using System;

namespace FlyingPizza.Drone
{
    public struct Option<T>
    {
        public bool HasValue { get; private set; }
        private T value;
        public T Value
        {
            get
            {
                if (HasValue)
                    return value;
                throw new InvalidOperationException();
            }
        }

        public Option(T value)
        {
            this.value = value;
            HasValue = true;
        }

        public static explicit operator T(Option<T> option)
        {
            return option.Value;
        }
        public static implicit operator Option<T>(T value)
        {
            return new(value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Option<T>)
                return this.Equals((Option<T>)obj);
            return false;
        }
        public bool Equals(Option<T> other)
        {
            if (HasValue && other.HasValue)
                return object.Equals(value, other.value);
            return HasValue == other.HasValue;
        }
    }
}
