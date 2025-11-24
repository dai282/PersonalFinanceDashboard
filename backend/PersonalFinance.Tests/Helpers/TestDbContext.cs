using Microsoft.EntityFrameworkCore;
using PersonalFinance.Infrastructure.Data;

namespace PersonalFinance.Tests.Helpers {
    public static class TestDbContext
    {
        //Creating a "fake" database in memory instead of connecting to a database engine (SQL server)
        public static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            //Unique database name for each test by creating a new
                            //Global Unique Identifier (GUID)
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;
            var context = new ApplicationDbContext(options);
            return context;
        }
    }

}


