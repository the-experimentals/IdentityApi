using System;
namespace IdentityApiTest.Ordering
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Priority : Attribute
    {
        public Priority(int priority)
        {
            _priority = priority;
        }

        public int _priority { get; private set; }
    }
}
