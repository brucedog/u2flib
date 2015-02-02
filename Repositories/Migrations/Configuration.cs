using System.Data.Entity.Migrations;
using Repositories.Context;

namespace Repositories.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Repository.DataContext";
        }

        protected override void Seed(DataContext context)
        {
        }
    }
}
