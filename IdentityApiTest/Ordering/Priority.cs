using System;

namespace IdentityApiTest.Ordering;

[AttributeUsage(AttributeTargets.Method)]
public class Priority : Attribute
{
    public Priority(int priority)
    {
        _priority = priority;
    }

    public int _priority { get; }
}
