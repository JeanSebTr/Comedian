---
layout: draft-notes
title: Some reading
---

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