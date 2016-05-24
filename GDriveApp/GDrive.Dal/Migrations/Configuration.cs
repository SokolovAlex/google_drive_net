using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace GDrive.Dal.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<GDrive.Dal.dbEntities.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(dbEntities.AppContext context)
        {
        }
    }
}
