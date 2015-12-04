---
layout: draft-notes
title: Some thinking
---

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