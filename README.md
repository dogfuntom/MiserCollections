#MiserCollections
Garbage-free collections intended for game developers.

##How is it different from standard collections?
When you exceed capacity of List<T>, it makes new inner array, copies old inner array content to new one, and then drops the old one, leaving it to GC.

When you exceed capacity of, for example, MiserList<T>, it makes new inner array and "appends" it to what it already has, no copying, no deallocation, no garbage.
