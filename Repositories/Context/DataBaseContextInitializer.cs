using System.Data.Entity;

namespace Repositories.Context
{
    public class DataBaseContextInitializer : DropCreateDatabaseIfModelChanges<DataContext>    
    {
        protected override void Seed(DataContext context)
        {
            // TODO  add default user here
        }
    }
}