---
layout: draft-notes
title: Some learning
---

I was so wrong.

- `Task.Run` with `TaskCreationOptions.LongRunning` will
 [simply create a new thread](http://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/ThreadPoolTaskScheduler.cs,33cd274e06874569,references)
- The default `SynchronizationContext` use `ThreadPool.QueueUserWorkItem` which I'm not sure what it does…
- But Task.Run use `ThreadPool.UnsafeQueueCustomWorkItem` which **very rarely** lock

It was naively ambitious to [try to do better](http://blogs.msdn.com/b/ericeil/archive/2009/04/23/clr-4-0-threadpool-improvements-part-1.aspx).


OK, so I've learn a lot about .Net threading utilities… Mostly that I should thrust it.
And that I should concentrate on making sure that each actor individually
and efficiently process its messages sequentially in a non-reentrant way.


I decided to completely change were I was going with the project.
I'll make a [Fody](https://github.com/Fody/Fody) Addin that will make any class thread-safe.


Made a first implementation of the `ActorCore` mixin that will be included in each actor class.
Next step will be to do the IL Weaving.
