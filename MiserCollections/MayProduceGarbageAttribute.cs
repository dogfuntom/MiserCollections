using System;

[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor,
    Inherited = true,
    AllowMultiple = false)]
sealed class MayProduceGarbageAttribute : Attribute
{
    public MayProduceGarbageAttribute()
    {
    }
}