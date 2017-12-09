namespace ObscureWare.BusinessResult
{
    using System;

    public sealed class Result<T> : Result
    {
        private static readonly Result<T> _fail = new Result<T>(ResultState.Failure, "");

        private readonly T _value;
        private const string BAD_IMPLEMENTATION = @"You cannot use Result<bool>. Use regular Result instead.";

        /// <inheritdoc />
        internal Result(ResultState state, string errorMessage = null) : base(state, errorMessage)
        {
            if (typeof(T) == typeof(bool))
            {
                throw new InvalidOperationException(BAD_IMPLEMENTATION);
            }
        }

        internal Result(T value) : base(ResultState.OK)
        {
            if (typeof(T) == typeof(bool))
            {
                throw new InvalidOperationException(BAD_IMPLEMENTATION);
            }

            this._value = value;
        }

        internal Result(Exception exception) : base(exception)
        {
            if (typeof(T) == typeof(bool))
            {
                throw new InvalidOperationException(BAD_IMPLEMENTATION);
            }
        }

        public T Value
        {
            get
            {
                if (this.HasFailed)
                {
                    throw new InvalidOperationException("Cannot read value from failed Result.", this.Exception); // will also use inner exception to propagate root cause
                }

                return this._value;
            }
        }

        public static implicit operator Result<T>(T value)
        {
            // TODO: eventually remove this check if does not work properly...
            if ((dynamic) default(T) == (dynamic) value)
            {
                return new Result<T>(ResultState.Failure, (string) null);
            }

            return new Result<T>(value);
        }

        public static implicit operator Result<T>(bool b)
        {
            if (b)
            {
                throw new InvalidOperationException("You cannot convert TRUE into successful Result<T>. Value is required when Result<T> is successful.");
            }

            return new Result<T>(ResultState.Failure);
        }

        public static implicit operator Result<T>(Exception ex)
        {
            return new Result<T>(ex);
        }

        public new static Result<T> Fail => _fail;

        public static Result<T> OK(T value)
        {
            return value;
        }
    }
}
