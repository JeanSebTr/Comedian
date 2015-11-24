# Journal of implementation

Thought process and learnings made during the implementation of **Comedian**

## Nov. 19, 2015

Started Comedian. The goal is to create a general purpose and ligh-tweight Actor library.

I made the assumption that I need a pool of worked threads which will receive work units.
The `Scene` will choose a queue to dispatch a work unit to and Enqueue the item.
This process must be thread safe and idealy lock-free.

I was uncertain about the phrasing of some .Net Framework documentation pages
Made unit tests to confirm my understanding of the API.

Started experimenting with code generation in a separate assembly.
Reflection.Emit doesn't exist for PCL.
**Comedian.Generator** will serve as post-build event for PCL projects or as live generator for normal projects.

## Nov. 20, 2015

Do some more reading on lock-free queues.

While thinking about the goal to be non-reentrant when a method implementation use await,
I realize why the concept of a mailbox is attached to an actor.

My concept of multiple dispatch queues won't work.

I'll need an implementation of [SynchronizationContext](https://msdn.microsoft.com/en-us/library/system.threading.synchronizationcontext%28v=vs.110%29.aspx) to support requeuing when `await` complete.
Maybe, I should start with a single worker thread implementation?

In fact, a worker thread would drain an actor of any continuous and synchronous work
until some work is awaited. At which point the SynchronizationContext would simply requeue the actor.

So I only need one `ConcurrentQueue<Actor>` which will be written to by the first caller to add to the mailbox of an actor
and by the SynchronizationContext when awaited work inside an actor complete.


## Nov. 21, 2015

I've read some interesting articles:

- [About SynchronizationContext](http://blogs.msdn.com/b/pfxteam/archive/2012/06/15/executioncontext-vs-synchronizationcontext.aspx)
- [How the compiler transform async methods](https://weblogs.asp.net/dixin/understanding-c-sharp-async-await-1-compilation)

This led me to read a lot of code on [Microsoft's Reference Source](http://referencesource.microsoft.com/)
of the classes used in async operations. Most of thoses operations seem lock-free:

- awaiting on task
- executing the continuation when not switching SynchronizationContext

The default SynchronizationContext use the ThreadPool which lock
on what seem to be an old implementation of a task queue when posting jobs to it.
The Task class will definitely lock when a thread wait for completion synchronously,
but that's irrelevant. I don't think it will lock for simple use of TaskCompletionSource.
It's frightening how much they optimised the hell out of Tasks.

So the ThreadPool doesn't respect my lock-free prerequisite but anyway I was begening to think
that using threads from the ThreadPool as long running worker threads is not a great idea.
And since we don't have direct access to the `Thread` class in PCL
I'm divided between a few choices:

- Let the user provide the Thread implementation (seems overcomplicated for the user)
- Build a different assembly for each platform (seems a pain)
- Build an addon assembly for each platform (worst?)
- Il-Merge a small module for each platform (hacky, I like transparent hacks)


## Nov. 23, 2015

I was so wrong.

- `Task.Run` with `TaskCreationOptions.LongRunning` will
 [simply create a new thread](http://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/ThreadPoolTaskScheduler.cs,33cd274e06874569,references)
- The default `SynchronizationContext` use `ThreadPool.QueueUserWorkItem` which I'm not sure what it does…
- But Task.Run use `ThreadPool.UnsafeQueueCustomWorkItem` which **very rarely** lock

It was naively ambitious to [try to do better](http://blogs.msdn.com/b/ericeil/archive/2009/04/23/clr-4-0-threadpool-improvements-part-1.aspx).


OK, so I've learn a lot about .Net threading utilities… Mostly that I should thrust it.
And that I should concentrate on making sure that each actor individually
and efficiently process its messages sequentially in a non-reentrant way.

…

I decided to completely change were I was going with the project.
I'll make a [Fody](https://github.com/Fody/Fody) Addin that will make any class thread-safe.


Made a first implementation of the `ActorCore` mixin that will be included in each actor class.
Next step will be to do the IL Weaving.
