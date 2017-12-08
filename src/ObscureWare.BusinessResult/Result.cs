namespace ObscureWare.BusinessResult
{
    using System;
    using System.CodeDom;

    // TODO: non-nullable class (must be class because of inheritance for Result<T>...)

    public class Result : IEquatable<Result>
    {
        private static Result _ok = new Result(ResultState.OK);

        private static Result _fail = new Result(ResultState.Failure, "");

        internal readonly ResultState _state;
        private readonly string _errorMessage;
        private readonly Exception _exception = null;

        protected internal Result(ResultState state, string errorMessage = null)
        {
            this._state = state;
            this._errorMessage = errorMessage;
        }

        protected internal Result(Exception exception, string message = null) : this(ResultState.Exception, message ?? exception.Message)
        {
            this._exception = exception;
        }

        public bool IsSuccess => this._state == ResultState.OK;

        public bool HasFailed => this._state != ResultState.OK;

        public string ErrorMessage => this._errorMessage ?? this._exception?.Message;

        public Exception Exception => this._exception;

        /// <inheritdoc />
        public bool Equals(Result other)
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

            return Equals((Result) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int) this._state;
        }

        public static Result OK => _ok;

        public static Result Fail => _fail;


        public static implicit operator bool (Result r)
        {
            return r.IsSuccess;
        }

        public static implicit operator Result (bool b)
        {
            return (b) ? OK : Fail;
        }

        public static implicit operator Result(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("message", nameof(message));
            }

            return new Result(ResultState.Failure, message);
        }

        public static Result FromException(Exception ex, string message = null)
        {
            return new Result(ex, message);
        }
    }
}
