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

I'll need an implementation of [SynchronizationContext](https://msdn.microsoft.com/en-us/library/system.threading.synchronizationcontext(v=vs.110).aspx) to support requeuing when `await` complete.
Maybe, I should start with a single worker thread implementation?

In fact, a worker thread would drain an actor of any continuous and synchronous work
until some work is awaited. At which point the SynchronizationContext would simply requeue the actor.

So I only need one `ConcurrentQueue<Actor>` which will be written to by the first caller to add to the mailbox of an actor
and by the SynchronizationContext when awaited work inside an actor complete.

