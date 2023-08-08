using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MVale.EntityFramework.Test.Design
{
    public class DesignTimeDbContextFactory_Test
    {
        public class TestDbContext : DbContext
        {
            public DbContextOptions<TestDbContext> Options { get; }

            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
            {
                Options = options;
            }
        }

        public class TestDbContextDesignTimeFactory :
            MVale.EntityFramework.Design.DesignTimeDbContextFactoryBase<TestDbContext>
        {
            public override bool IsInteractive => false;

            protected override TestDbContext CreateDbContext(DbContextOptions options)
            {
                return new TestDbContext((DbContextOptions<TestDbContext>) options);
            }
        }

        [Test]
        public void Test()
        {
            const string connectionString = "Test-Connection-String";

            var factory = new TestDbContextDesignTimeFactory();

            var args = new string[]
            {
                "--connection",
                connectionString
            };

            var dbContext = factory.CreateDbContext(args);
            Assert.NotNull(dbContext);

            Assert.NotNull(dbContext.Options);
            foreach (var extension in dbContext.Options.Extensions.OfType<Microsoft.EntityFrameworkCore.Infrastructure.RelationalOptionsExtension>())
            {
                Assert.AreEqual(connectionString, extension.ConnectionString);
            }
        }
    }
}