namespace ObscureWare.BusinessResult
{
    using System;

    public static class ResultExtensions
    {
        public static Result Simplify<T>(this Result<T> valueResult)
        {
            if (valueResult.Exception != null)
            {
                return new Result(valueResult.Exception);
            }
            else
            {
                return new Result(valueResult._state, valueResult.ErrorMessage);
            }
            
        }

        /// <summary>
        /// Casts inner operation failure into typed failure
        /// </summary>
        /// <param name="innerResult"></param>
        /// <returns></returns>
        public static Result<T> Cast<T>(this Result innerResult)
        {
            // TODO: discuss - not necessary - let it throw when Value is asked without checking for failure...
            // or no? There is no reason to cast not-failed Result actually...
            if (innerResult.IsSuccess)
            {
                throw new InvalidOperationException($"Cannot convert successful Result into Result<{nameof(T)}>.");
            }

            if (innerResult.Exception != null)
            {
                return new Result<T>(innerResult.Exception);
            }
            else
            {
                return new Result<T>(innerResult._state, innerResult.ErrorMessage);
            }
        }

    }
}