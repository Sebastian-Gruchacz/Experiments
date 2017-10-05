namespace EfficiencyTests
{
    using System;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using FizzWare.NBuilder;
    using ModelProject;
    using ModelProject.Model;
    using Xunit;
    using FizzWare.NBuilder.Extensions;
    using Shouldly;
    using Xunit.Abstractions;

    public class EntityFrameworkEfficiencyTesting
    {
        private readonly ITestOutputHelper output;

        public EntityFrameworkEfficiencyTesting(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void time_of_using_shared_context_shall_be_less_than_pool_sharing()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this.Warmup();
            stopwatch.Stop();
            this.output.WriteLine(@"Warm-up time: " +  stopwatch.ElapsedTicks.ToString(@"N"));

            stopwatch.Restart();
            for (int i = 0; i < 100; i++)
            {
                this.RunDoubleContext();
            }
            stopwatch.Stop();
            var doubleTime = stopwatch.ElapsedTicks;


            stopwatch.Restart();
            for (int i = 0; i < 100; i++)
            {
                this.RunSingleContext();
            }
            stopwatch.Stop();
            var singleTime = stopwatch.ElapsedTicks;


            this.output.WriteLine(singleTime.ToString(@"N") + @" - " + doubleTime.ToString(@"N"));
            doubleTime.ShouldBeGreaterThan(singleTime);
        }

        private void Warmup()
        {
            this.RunDoubleContext();
            this.RunSingleContext();
        }

        private void RunDoubleContext()
        {
            var clientContext = new TestContext();
            var userContext = new TestContext();

            var client = this.NewClient();
            var user = this.NewUser();
            clientContext.Clients.Add(client);
            userContext.Users.Add(user);

            clientContext.SaveChanges();
            userContext.SaveChanges();
        }

        private void RunSingleContext()
        {
            var context = new TestContext();

            var client = this.NewClient();
            var user = this.NewUser();
            context.Clients.Add(client);
            context.Users.Add(user);

            context.SaveChanges();
        }

        private User NewUser()
        {
            return Builder<User>.CreateNew().Build();
        }

        private Client NewClient()
        {
            return Builder<Client>.CreateNew().Build();
        }
    }
}
