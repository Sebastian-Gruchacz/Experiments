namespace ModelProject.Model
{
    using System;

    public class User
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}