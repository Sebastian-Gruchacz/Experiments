namespace ModelProject
{
    using System.Data.Entity;
    using Model;

    public class TestContext : DbContext
    {
        public TestContext() : base ("name=TestDBConnectionString")
        {
            
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
