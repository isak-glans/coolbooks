namespace CoolBooks.Migrations
{
    using CoolBooks.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CoolBooks.Models.CoolBooksDataModel>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CoolBooks.Models.CoolBooksDataModel context)
        {
            var entityContext = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(entityContext);
            var manager = new UserManager<ApplicationUser>(store);
            string adminId;

            context.Users.AddOrUpdate(p => p.DisplayName,
                new Models.Users
                {
                    UserId = "f21da5e9-7bc6-4736-b6a2-e411bcf2019f",
                    FirstName = "admin",
                    LastName = "admin",
                    Email = "admin@admin.com",
                    DisplayName = "admin",
                    IsDeleted = false,
                    Created = DateTime.Now
                });
        }
    }
}
