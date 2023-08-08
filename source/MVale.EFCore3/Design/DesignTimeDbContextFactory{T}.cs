using System;
using System.CommandLine;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace MVale.EntityFramework.Design
{
    public abstract class DesignTimeDbContextFactoryBase< T> : IDesignTimeDbContextFactory<T>
        where T : DbContext
    {
        public virtual bool IsInteractive => true;

        private string GetConnectionStringFromCommandLine(string connectionString)
        {
            bool isAccepted = false;
            while (!isAccepted)
            {
                if (connectionString == null)
                {
                    Console.WriteLine("Enter a connection string...");
                    connectionString = Console.ReadLine();
                }

                Console.WriteLine($"The current connection string is: '{connectionString}', Confirm? (Yes)");
                isAccepted = Console.ReadLine().Equals("Yes", StringComparison.InvariantCultureIgnoreCase);
            }

            return connectionString;
        }

        protected abstract T CreateDbContext(DbContextOptions options);

        protected virtual DbContextOptionsBuilder CreateDbContextOptionsBuilder()
        {
            return new DbContextOptionsBuilder<T>();
        }

        protected virtual void Configure(DbContextOptionsBuilder builder, string[] args)
        {
            string connectionString = Environment.GetEnvironmentVariable("connection");

            var connectionStringOption = new Option<string>(
                new string[]
                {
                    "-c",
                    "-C",
                    "--connection",
                    "--Connection",
                    "--connection-string",
                    "--Connection-String"
                },
                () => connectionString,
                "Specify a connection string to a server for the db context.")
            {
                Name = "--ConnectionString",
                IsRequired = true,
            };

            var rootCommand = new RootCommand($"Create an instance of the {typeof(T)} db context.")
            {
                connectionStringOption
            };

            rootCommand.SetHandler(
                (string connection) =>
                {
                    Console.WriteLine($"Creating an instance of: {typeof(T)}.");

                    if (this.IsInteractive)
                    {
                        connection = this.GetConnectionStringFromCommandLine(connection);
                    }

                    foreach (var relationalOptionsExtension in builder.Options.Extensions.OfType<RelationalOptionsExtension>())
                    {
                        ((IDbContextOptionsBuilderInfrastructure) builder).AddOrUpdateExtension(
                            relationalOptionsExtension.WithConnectionString(connection));
                    }

/*#pragma warning disable EF1001
                    if (builder.Options.FindExtension<NpgsqlOptionsExtension>() is {} existing)
                    {
                        ((IDbContextOptionsBuilderInfrastructure) builder).AddOrUpdateExtension(
                            (NpgsqlOptionsExtension) existing.WithConnectionString(connection));
                    }
#pragma warning restore EF1001*/
                },
                connectionStringOption);

            rootCommand.Invoke(args);
        }

        public T CreateDbContext(string[] args)
        {
            var builder = this.CreateDbContextOptionsBuilder();
            this.Configure(builder, args);
            return this.CreateDbContext(builder.Options);
        }
    }
}