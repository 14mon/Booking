
using booking_system.Models;
using Microsoft.EntityFrameworkCore;

namespace booking_system.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Country Relationship
            modelBuilder.Entity<Country>()
                        .HasMany(u => u.Users)
                        .WithOne(h => h.Country)
                        .HasForeignKey(h => h.CountryId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Country>()
                        .HasMany(u => u.PaymentGateways)
                        .WithOne(h => h.Country)
                        .HasForeignKey(h => h.CountryId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Country>()
                        .HasMany(u => u.Classes)
                        .WithOne(h => h.Country)
                        .HasForeignKey(h => h.CountryId)
                        .OnDelete(DeleteBehavior.Cascade);

            //user Relationship

            // modelBuilder.Entity<User>()
            //             .HasMany(u => u.UserCreditHistories)
            //             .WithOne(h => h.User)
            //             .HasForeignKey(h => h.UserId)
            //             .OnDelete(DeleteBehavior.Cascade);       //if mobile need to query expand UserCreditHistory , will connect

            modelBuilder.Entity<User>()
                        .HasMany(u => u.Transactions)
                        .WithOne(h => h.User)
                        .HasForeignKey(h => h.UserId)
                        .OnDelete(DeleteBehavior.Cascade);


            //Payment GateWay
            modelBuilder.Entity<PaymentGateway>()
                        .HasMany(u => u.Packages)
                        .WithOne(h => h.PaymentGateway)
                        .HasForeignKey(h => h.GatewayId)
                        .OnDelete(DeleteBehavior.Cascade);

            //Package
            modelBuilder.Entity<Package>()
                        .HasMany(p => p.Transactions)
                        .WithOne(t => t.Package)
                        .HasForeignKey(p => p.PackageId)
                        .OnDelete(DeleteBehavior.Cascade);

            //GateWayRawEvent
            modelBuilder.Entity<Transaction>()
                        .HasOne(g => g.GatewayRawEvent)
                        .WithOne(t => t.Transaction)
                        .HasForeignKey<Transaction>(t => t.GateWayRawEventId)
                        .OnDelete(DeleteBehavior.Cascade);

            //Class
            modelBuilder.Entity<Class>()
                        .HasMany(c => c.ClassBookings)
                        .WithOne(h => h.Class)
                        .HasForeignKey(h => h.ClassId)
                        .OnDelete(DeleteBehavior.Cascade);


            //ClassCreditHistory
            modelBuilder.Entity<UserCreditHistory>()
                        .HasMany(g => g.ClassBookings)
                        .WithOne(t => t.UserCreditHistory)
                        .HasForeignKey(t => t.UserCreditId)
                        .OnDelete(DeleteBehavior.Cascade);


            //ClassBooking
            modelBuilder.Entity<UserCreditHistory>()
                       .HasMany(g => g.ClassBookings)
                       .WithOne(t => t.UserCreditHistory)
                       .HasForeignKey(t => t.UserCreditId)
                       .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassBooking>()
                      .HasOne(g => g.Refund)
                      .WithOne(t => t.ClassBooking)
                      .HasForeignKey<Refund>(t => t.BookingId)
                      .OnDelete(DeleteBehavior.Cascade);

            //Refund
            modelBuilder.Entity<Refund>()
                     .HasOne(g => g.UserCreditHistory)
                     .WithOne(t => t.Refund)
                     .HasForeignKey<UserCreditHistory>(t => t.RefundId)
                     .OnDelete(DeleteBehavior.Cascade);


        }
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Class> Classes { get; set; } = default!;
        public DbSet<ClassBooking> ClassBookings { get; set; } = default!;
        public DbSet<Country> Countries { get; set; } = default!;
        public DbSet<GatewayRawEvent> GatewayRawEvents { get; set; } = default!;
        public DbSet<Package> Packages { get; set; } = default!;
        public DbSet<PaymentGateway> PaymentGateways { get; set; } = default!;
        public DbSet<Refund> Refunds { get; set; } = default!;
        public DbSet<Transaction> Transactions { get; set; } = default!;
        public DbSet<UserCreditHistory> UserCreditHistories { get; set; } = default!;


    }
}