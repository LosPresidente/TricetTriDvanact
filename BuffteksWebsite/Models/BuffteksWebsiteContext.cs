using Microsoft.EntityFrameworkCore;

namespace BuffteksWebsite.Models
{
    public class BuffteksWebsiteContext : DbContext
    {
        public BuffteksWebsiteContext (DbContextOptions<BuffteksWebsiteContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectRoster> ProjectRoster { get; set; }

        protected override void  OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Member>().ToTable("Client");
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Project>().ToTable("Project");


/*
This one helps the database make its connections 
LINQ
project     project roster      project participants
0               -< 0 >-                 0
 */

            //establish the join through the keys
            modelBuilder.Entity<ProjectRoster>()
                .HasKey(pr => new {pr.ProjectID, pr.ProjectParticipantID});

            //set up the one to many map from Project to ProjectRoster
            modelBuilder.Entity<ProjectRoster>()
                .HasOne(pr => pr.Project)
                .WithMany(p => p.Participants)
                .HasForeignKey(pr => pr.ProjectID);

            //set up the one to many map from ProjectParticipant to ProjectRoster
            //by "map" he means the path one-to-many and one-to-many
            modelBuilder.Entity<ProjectRoster>()
                .HasOne(pr => pr.ProjectParticipant)
                .WithMany(pp => pp.Projects)
                .HasForeignKey(pr => pr.ProjectParticipantID);

        }
    }
}