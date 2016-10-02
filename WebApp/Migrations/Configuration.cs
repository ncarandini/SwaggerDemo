namespace SwaggerDemo.WebApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var marioRossi = context.Users.FirstOrDefault(u => u.UserName == "mario.rossi@opendomus.it");
            if (marioRossi == null)
            {
                marioRossi = new ApplicationUser
                {
                    UserName = "mario.rossi@opendomus.it",
                    FirstName = "Mario",
                    LastName = "Rossi",
                    Email = "mario.rossi@opendomus.it"
                };
                userManager.Create(marioRossi, "Mario:123");
            }

            var francoVerdi = context.Users.FirstOrDefault(u => u.UserName == "franco.verdi@opendomus.it");
            if (francoVerdi == null)
            {
                francoVerdi = new ApplicationUser
                {
                    UserName = "franco.verdi@opendomus.it",
                    FirstName = "Franco",
                    LastName = "Verdi",
                    Email = "franco.verdi@opendomus.it"
                };
                userManager.Create(francoVerdi, "Franco:123");
            }

            var firstMeetup = context.Meetups.FirstOrDefault(m => m.Title == "OpenDomus first meetup!");
            if (firstMeetup == null)
            {
                firstMeetup = new Meetup
                {
                    Title = "OpenDomus first meetup!",
                    Description = "Come primo meetup del gruppo ci incontriamo per parlare di open software in casa Microsoft, per rompere i vecchi schemi ancorati ai ricordi del passato",
                    StartAt = new DateTimeOffset(2016, 4, 30, 9, 30, 0, TimeSpan.FromHours(2)),
                    EndAt = new DateTimeOffset(2016, 4, 30, 13, 0, 0, TimeSpan.FromHours(2))
                };
                context.Meetups.Add(firstMeetup);
            }
            var secondMeetup = context.Meetups.FirstOrDefault(m => m.Title == "OpenDomus Meetup: EventStorming");
            if (secondMeetup == null)
            {
                secondMeetup = new Meetup
                {
                    Title = "OpenDomus Meetup: EventStorming",
                    Description = "Nella splendida sede di Luiss Enlabs, incontriamo Andrea Balducci che ci racconta di EventStorming e molto altro ancora.",
                    StartAt = new DateTimeOffset(2016, 9, 30, 18, 0, 0, TimeSpan.FromHours(2)),
                    EndAt = new DateTimeOffset(2016, 9, 30, 20, 0, 0, TimeSpan.FromHours(2))
                };
                context.Meetups.Add(secondMeetup);
            }
        }
    }
}
