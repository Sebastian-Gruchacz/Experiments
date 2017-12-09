namespace BusinessResult.SamplesAndTests
{
    using System;
    using System.Linq;

    using ObscureWare.BusinessResult;

    public class BusinessResultSamples
    {
        public void MainMethod()
        {
            var result = this.A();
            if (result.IsSuccess)
            {
                var r2 = this.B();
                if (r2.IsSuccess)
                {
                    Console.WriteLine(r2.Value);
                }

            }
        }

        public Result A()
        {
            return true;
        }

        public Result AM()
        {
            return "Invalid method execution";
        }

        public Result<int> B()
        {
            return 5;
        }

        public Result<int> C(Guid id)
        {
            if (id == Guid.Empty)
            {
                // Such errors should be thrown anyway
                throw new ArgumentException("Identificator cannot be empty.", nameof(id));
            }

            return id.ToByteArray().First();
        }

        public Result<Guid> TryConversion(string id)
        {
            if (Guid.TryParse(id, out var result))
            {
                return result;
            }

            return false;
        }

        public Result<Guid> TryConversionAlternate(string id)
        {
            try
            {
                return Guid.Parse(id);
            }
            catch (Exception ex)
            {
                return ex;
                // return Result<Guid>.Fail;
            }
        }

        public Result CastDown()
        {
            var r = this.B();
            if (r.HasFailed)
            {
                return r;
            }

            return true;
        }

        public Result<int> CastUp()
        {
            var r = this.A();
            if (r.HasFailed)
            {
                return r.Cast<int>();
            }

            return 0;
        }
    }
}
