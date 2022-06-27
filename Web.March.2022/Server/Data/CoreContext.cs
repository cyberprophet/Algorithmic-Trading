using Duende.IdentityServer.EntityFramework.Extensions;
using Duende.IdentityServer.EntityFramework.Options;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using ShareInvest.Models;
using ShareInvest.Models.OpenAPI;
using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Data
{
    public class CoreContext : ApiAuthorizationDbContext<CoreUser>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Stock>(o =>
            {
                o.ToTable(nameof(Stock));
                o.HasKey(o => o.Code);
                o.HasOne(o => o.Overview).WithOne().OnDelete(DeleteBehavior.Cascade).HasForeignKey<CompanyOverview>(o => o.Code);
            });
            builder.Entity<CompanyOverview>(o =>
            {
                o.ToTable(nameof(CompanyOverview));
                o.HasKey(o => o.Code);
            });
            builder.Entity<Securities>(o =>
            {
                o.HasKey(o => new
                {
                    o.Key,
                    o.Company
                });
                o.HasMany(o => o.Messages).WithOne().OnDelete(DeleteBehavior.Cascade).HasForeignKey(o => new
                {
                    o.Key,
                    o.Company
                });
                o.HasMany(o => o.Accounts).WithOne().OnDelete(DeleteBehavior.Cascade).HasForeignKey(o => new
                {
                    o.Key,
                    o.Company
                });
                o.ToTable(nameof(Securities));
            });
            builder.Entity<Message>(o =>
            {
                o.HasKey(o => new
                {
                    o.Key,
                    o.Lookup,
                    o.Company
                });
                o.ToTable(nameof(Message));
            });
            builder.Entity<Account>(o =>
            {
                o.HasKey(o => new
                {
                    o.Key,
                    o.Date,
                    o.AccNo,
                    o.Company
                });
                o.HasMany(o => o.Balances).WithOne().OnDelete(DeleteBehavior.Cascade).HasForeignKey(o => new
                {
                    o.Key,
                    o.Date,
                    o.AccNo,
                    o.Company
                });
                o.ToTable(nameof(Account));
            });
            builder.Entity<Balance>(o =>
            {
                o.HasKey(o => new
                {
                    o.Key,
                    o.Code,
                    o.Date,
                    o.AccNo,
                    o.Company
                });
                o.ToTable(nameof(Balance));
            });
            builder.ConfigurePersistedGrantContext(store.Value);
        }
        public DbSet<CompanyOverview>? Overviews
        {
            get; set;
        }
        public DbSet<Message>? Messages
        {
            get; set;
        }
        public DbSet<Balance>? Balances
        {
            get; set;
        }
        public DbSet<Account>? Accounts
        {
            get; set;
        }
        public DbSet<Securities>? Securities
        {
            get; set;
        }
        public DbSet<Stock>? Stocks
        {
            get; set;
        }
        public CoreContext(DbContextOptions options, IOptions<OperationalStoreOptions> store) : base(options, store) => this.store = store;
        readonly IOptions<OperationalStoreOptions> store;
    }
}