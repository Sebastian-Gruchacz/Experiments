using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureWare.BusinessResult
{
    public struct ResultS<T> : IEquatable<ResultS<T>>
    {
        private static readonly ResultS<T> _fail = new ResultS<T>(ResultState.Failure, "");

        private readonly T _value;
        private const string BAD_IMPLEMENTATION_WITH_BOOL = @"You cannot use Result<bool>. Use regular Result instead.";

        /// <inheritdoc />
        internal ResultS(ResultState state, string errorMessage = null) 
        {
            if (typeof(T) == typeof(bool))
            {
                throw new InvalidOperationException(BAD_IMPLEMENTATION_WITH_BOOL);
            }
        }

        internal ResultS(T value)
        {
            if (typeof(T) == typeof(bool))
            {
                throw new InvalidOperationException(BAD_IMPLEMENTATION_WITH_BOOL);
            }

            this._value = value;
        }

        internal ResultS(Exception exception)
        {
            if (typeof(T) == typeof(bool))
            {
                throw new InvalidOperationException(BAD_IMPLEMENTATION_WITH_BOOL);
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

        public static implicit operator ResultS<T>(T value)
        {
            // TODO: eventually remove this check if does not work properly...
            if ((dynamic)default(T) == (dynamic)value)
            {
                return new Result<T>(ResultState.Failure, (string)null);
            }

            return new Result<T>(value);
        }

        public static implicit operator ResultS<T>(bool b)
        {
            if (b)
            {
                throw new InvalidOperationException("You cannot convert TRUE into successful Result<T>. Value is required when Result<T> is successful.");
            }

            return new ResultS<T>(ResultState.Failure);
        }

        public static implicit operator ResultS<T>(Exception ex)
        {
            return new ResultS<T>(ex);
        }

        public new static ResultS<T> Fail => _fail;

        public static ResultS<T> OK(T value)
        {
            return value;
        }



        private static ResultS _ok = new ResultS(ResultState.OK);

        private static ResultS<T> _fail = new ResultS<T>(ResultState.Failure, "");

        internal readonly ResultState _state;
        private readonly string _errorMessage;
        private readonly Exception _exception;

        internal ResultS(ResultState state, string errorMessage = null)
        {
            this._state = state;
            this._errorMessage = errorMessage;
            this._exception = null;
        }

        internal ResultS(Exception exception, string message = null) : this(ResultState.Exception, message ?? exception.Message)
        {
            this._exception = exception;
        }

        public bool IsSuccess => this._state == ResultState.OK;

        public bool HasFailed => this._state != ResultState.OK;

        public string ErrorMessage => this._errorMessage ?? this._exception?.Message;

        public Exception Exception => this._exception;

        /// <inheritdoc />
        public bool Equals(ResultS other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this._state == other._state;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((ResultS)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int)this._state;
        }

        public static ResultS OK => _ok;

        public static ResultS Fail => _fail;


        public static implicit operator bool(ResultS r)
        {
            return r.IsSuccess;
        }

        public static implicit operator ResultS(bool b)
        {
            return (b) ? OK : Fail;
        }

        public static implicit operator ResultS(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("message", nameof(message));
            }

            return new ResultS(ResultState.Failure, message);
        }

        public static ResultS FromException(Exception ex, string message = null)
        {
            return new ResultS(ex, message);
        }
    }
}
