#MiserCollections
Garbage-free collections intended for game developers.

##Table of contents
- [How is it different from standard collections?](#How is it different from standard collections?)
  - [`MiserList<T>` vs `List<T>`](#`MiserList<T>` vs `List<T>`)

##How is it different from standard collections?
###`MiserList<T>` vs `List<T>`
When you exceed capacity of `List<T>`, it makes new inner array, copies old inner array content to new one, and then drops the old one, leaving it to GC.

[Here is how it looks in modern .NET](http://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs,9808f1f5ef16c436)

And here is what Unity 4.6 uses:
![Unity 4.6 List<T>.Capacity implementation](/Images/List'T.Capacity.png?raw=true "Unity 4.6 List<T>.Capacity implementation")

and

![Unity 4.6 Array.Resize() implementation](/Images/Array.Resize.png?raw=true "Unity 4.6 Array.Resize() implementation")

When you exceed capacity of `MiserList<T>`, it makes new inner array and "appends" it to what it already has, no copying, no deallocation, no garbage.
