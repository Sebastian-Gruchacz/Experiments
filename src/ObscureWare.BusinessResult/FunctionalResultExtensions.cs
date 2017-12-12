namespace ObscureWare.BusinessResult
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://gist.github.com/vkhorikov/7852c7606f27c52bc288</remarks>
    public static class FunctionalResultExtensions
    {
        public static Result OnSuccess(this Result result, Func<Result> func)
        {
            if (result.HasFailed)
                return result;

            return func();
        }

        public static Result OnSuccess(this Result result, Action action)
        {
            if (result.HasFailed)
                return result;

            action();

            return Result.OK;
        }

        public static Result OnSuccess<T>(this Result<T> result, Action<T> action)
        {
            if (result.HasFailed)
                return result;

            action(result.Value);

            return Result.OK;
        }

        public static Result<T> OnSuccess<T>(this Result result, Func<T> func)
        {
            if (result.HasFailed)
                return Result.Fail<T>(result.Error);

            return Result.Ok(func());
        }

        public static Result<T> OnSuccess<T>(this Result result, Func<Result<T>> func)
        {
            if (result.HasFailed)
                return Result.Fail<T>(result.Error);

            return func();
        }

        public static Result OnSuccess<T>(this Result<T> result, Func<T, Result> func)
        {
            if (result.HasFailed)
                return result;

            return func(result.Value);
        }

        public static Result OnFailure(this Result result, Action action)
        {
            if (result.HasFailed)
            {
                action();
            }

            return result;
        }

        public static Result OnBoth(this Result result, Action<Result> action)
        {
            action(result);

            return result;
        }

        public static T OnBoth<T>(this Result result, Func<Result, T> func)
        {
            return func(result);
        }
    }
}