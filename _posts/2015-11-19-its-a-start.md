---
layout: draft-notes
title: It's a start
---

Started Comedian. The goal is to create a general purpose and ligh-tweight Actor library.

I made the assumption that I need a pool of worked threads which will receive work units.
The `Scene` will choose a queue to dispatch a work unit to and Enqueue the item.
This process must be thread safe and idealy lock-free.

I was uncertain about the phrasing of some .Net Framework documentation pages
Made unit tests to confirm my understanding of the API.

Started experimenting with code generation in a separate assembly.
Reflection.Emit doesn't exist for PCL.
**Comedian.Generator** will serve as post-build event for PCL projects or as live generator for normal projects.