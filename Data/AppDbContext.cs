using Microsoft.EntityFrameworkCore;
using ZALaw.Api.Models;

namespace ZALaw.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaxApplication> Applications => Set<TaxApplication>();
    public DbSet<ChecklistResponse> ChecklistResponses => Set<ChecklistResponse>();
    public DbSet<DocumentFile> Documents => Set<DocumentFile>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Service> Services => Set<Service>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<TaxApplication>(entity =>
        {
            entity.HasIndex(a => a.ApplicationNumber).IsUnique();

            entity.HasOne(a => a.Client)
                  .WithMany(u => u.ApplicationsAsClient)
                  .HasForeignKey(a => a.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.AssignedConsultant)
                  .WithMany(u => u.ApplicationsAsConsultant)
                  .HasForeignKey(a => a.AssignedConsultantId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ChecklistResponse>(entity =>
        {
            entity.HasIndex(c => new { c.ApplicationId, c.SectionKey, c.FieldKey }).IsUnique();
            entity.HasOne(c => c.Application)
                  .WithMany(a => a.ChecklistResponses)
                  .HasForeignKey(c => c.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DocumentFile>(entity =>
        {
            entity.HasOne(d => d.Application)
                  .WithMany(a => a.Documents)
                  .HasForeignKey(d => d.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.ReviewedBy)
                  .WithMany()
                  .HasForeignKey(d => d.ReviewedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasOne(m => m.Application)
                  .WithMany(a => a.Messages)
                  .HasForeignKey(m => m.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Sender)
                  .WithMany()
                  .HasForeignKey(m => m.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne(n => n.User)
                  .WithMany()
                  .HasForeignKey(n => n.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasOne(ap => ap.Client)
                  .WithMany()
                  .HasForeignKey(ap => ap.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ap => ap.Consultant)
                  .WithMany()
                  .HasForeignKey(ap => ap.ConsultantId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasIndex(s => s.Slug).IsUnique();
        });

        // Seed service catalog — matches the public site's services page.
        modelBuilder.Entity<Service>().HasData(
            new Service { Id = Guid.Parse("11111111-1111-1111-1111-111111111101"), Slug = "individual-tax-filing", Name = "Individual Tax Filing", ShortDescription = "Guided FBR return filing for salaried persons and freelancers.", Icon = "FileText" },
            new Service { Id = Guid.Parse("11111111-1111-1111-1111-111111111102"), Slug = "corporate-tax-compliance", Name = "Corporate Tax & Compliance", ShortDescription = "End-to-end corporate return filing and regulatory compliance.", Icon = "Building2" },
            new Service { Id = Guid.Parse("11111111-1111-1111-1111-111111111103"), Slug = "legal-advisory", Name = "Legal Advisory", ShortDescription = "Business and personal legal advisory and documentation support.", Icon = "Scale" },
            new Service { Id = Guid.Parse("11111111-1111-1111-1111-111111111104"), Slug = "bookkeeping-accounting", Name = "Bookkeeping & Accounting", ShortDescription = "Ongoing bookkeeping and financial record-keeping for businesses.", Icon = "BookOpenCheck" },
            new Service { Id = Guid.Parse("11111111-1111-1111-1111-111111111105"), Slug = "financial-planning-advisory", Name = "Financial Planning & Advisory", ShortDescription = "Strategic financial planning for individuals and businesses.", Icon = "TrendingUp" },
            new Service { Id = Guid.Parse("11111111-1111-1111-1111-111111111106"), Slug = "ntn-strn-registration", Name = "NTN / STRN & Registration", ShortDescription = "Tax registration for individuals, freelancers and businesses.", Icon = "BadgeCheck" }
        );
    }
}
