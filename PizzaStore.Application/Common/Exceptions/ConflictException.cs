using System;

namespace PizzaStore.Application.Common.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}
