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

            var session_1_1 = context.Sessions.FirstOrDefault(s => s.Title == ".NET Cross Platform alla prova dei fatti");
            if (session_1_1 == null)
            {
                session_1_1 = new Session
                {
                    Title = ".NET Cross Platform alla prova dei fatti",
                    Description = "In questo talk mostreremo com’è possibile creare una code base unica C# / .NET che possa girare egualmente bene sia su piattaforme desktop (da Windows XP fino a Win10) sia su mobile iOS, Android e Windows. Parleremo di Portable Classes, Shared project, #ifdef, tecniche di Inversion of Control e cenni di Xamarin. Nel corso del talk condivideremo alcuni progetti open source che abbiamo realizzato utilizzando le tecniche suddette: un client REST, una object cache locale, e altri ancora.",
                    ProposedAt = new DateTimeOffset(2016, 3, 18, 11, 30, 0, TimeSpan.FromHours(2)),
                    CancelledAt = default(DateTimeOffset),
                    Proponent = marioRossi,
                    Moderator = francoVerdi,
                    SessionState = SessionState.Done,
                    Meetup = firstMeetup,
                    Votes = null
                };
                context.Sessions.Add(session_1_1);
            }

            var session_1_2 = context.Sessions.FirstOrDefault(s => s.Title == "Microsoft e OSS: La strana coppia :-)");
            if (session_1_2 == null)
            {
                session_1_2 = new Session
                {
                    Title = "Microsoft e OSS: La strana coppia :-)",
                    Description = "In questa sessione vedremo come si pone la “nuova” Microsoft nei confronti di Linux e di moltissime altre tecnologie Open Source. Parleremo di Linux su Azure, Sql Server su Linux, di Bash e tutta la parte usermode di Ubuntu Linux in Windows 10 e di tanti altri argomenti, come ad esempio tutti i nuovi servizi e le nuove API Microsoft in un contesto aperto e multipiattaforma.",
                    ProposedAt = new DateTimeOffset(2016, 3, 18, 11, 00, 0, TimeSpan.FromHours(2)),
                    CancelledAt = default(DateTimeOffset),
                    Proponent = marioRossi,
                    Moderator = francoVerdi,
                    SessionState = SessionState.Done,
                    Meetup = firstMeetup,
                    Votes = null
                };
                context.Sessions.Add(session_1_2);
            }

            var session_1_3 = context.Sessions.FirstOrDefault(s => s.Title == "Creare applicazioni che rispondano ai comandi in linguaggio naturale");
            if (session_1_3 == null)
            {
                session_1_3 = new Session
                {
                    Title = "Creare applicazioni che rispondano ai comandi in linguaggio naturale",
                    Description = "Prima della presentazione di Microsoft Cognitive Services, le possibilità per gli sviluppatori di sperimentare le tecnologie di intelligenza artificiale e machine learning erano limitate dalla necessità di robuste basi teoriche o dell’accesso a prodotti proprietari di aziende specializzate e con costi importanti. LUIS, uno dei servizi di intelligenza artificiale recentemente presentati da Microsoft, consente di istruire le applicazioni a rispondere a dei comandi espressi in linguaggio naturale, come si farebbe con una persona. Poiché questi algoritmi sono esposti attraverso servizi HTTP REST, è facile integrare queste funzionalità all'interno di applicazioni basate su architetture e piattaforme differenti.",
                    ProposedAt = new DateTimeOffset(2016, 3, 20, 9, 30, 0, TimeSpan.FromHours(2)),
                    CancelledAt = default(DateTimeOffset),
                    Proponent = marioRossi,
                    Moderator = francoVerdi,
                    SessionState = SessionState.Done,
                    Meetup = firstMeetup,
                    Votes = null
                };
                context.Sessions.Add(session_1_3);
            }

            var session_2_1 = context.Sessions.FirstOrDefault(s => s.Title == "EventSourcing & EventStorming: 'It...could...works!'");
            if (session_2_1 == null)
            {
                session_2_1 = new Session
                {
                    Title = "EventSourcing & EventStorming: 'It...could...works!'",
                    Description = "La storia poco romanzata e molto tecnica di come una manciata di sviluppatori ha trasformato un 'pet project' (JARVIS) in un prodotto in grado di competere con i leader di mercato. Vedremo come Pattern e Metodologie (DDD, Eventsourcing, opensource, eventstorming, kanban, noSql, TDD, BDD, SOA) permettono di passare velocemente dal problema al prototipo in grado di evolvere nel tempo mantenendo basso il costo di implementazione.",
                    ProposedAt = new DateTimeOffset(2016, 7, 20, 15, 30, 0, TimeSpan.FromHours(2)),
                    CancelledAt = default(DateTimeOffset),
                    Proponent = marioRossi,
                    Moderator = francoVerdi,
                    SessionState = SessionState.Done,
                    Meetup = firstMeetup,
                    Votes = null
                };
                context.Sessions.Add(session_2_1);
            }

            var session_A = context.Sessions.FirstOrDefault(s => s.Title == "IOT Security: Il punto della sitazione");
            if (session_A == null)
            {
                session_A = new Session
                {
                    Title = "IOT Security: Il punto della sitazione",
                    Description = "Una panoramica dello stato dell'arte della sicurezza nel mondo dell'IoT.",
                    ProposedAt = new DateTimeOffset(2016, 10, 3, 18, 30, 0, TimeSpan.FromHours(2)),
                    CancelledAt = default(DateTimeOffset),
                    Proponent = francoVerdi,
                    Moderator = francoVerdi,
                    SessionState = SessionState.Proposed,
                    Meetup = firstMeetup,
                    Votes = null
                };
                context.Sessions.Add(session_A);
            }

        }
    }
}
