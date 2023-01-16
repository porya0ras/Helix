using Duende.IdentityServer.EntityFramework.Options;
using helix.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace helix.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {


        }

        public  DbSet<SObject> SObjects { get; set; }
        public  DbSet<Filter> Filters { get; set; }
        public  DbSet<Detector> Detectors { get; set; }
        public  DbSet<Telescope> Telescopes { get; set; }
        public  DbSet<ObservationSubmission> ObservationSubmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        { 
            base.OnModelCreating(builder);
        }

    }
}