using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataBase.DataBase;

public partial class CourierServiceContext : DbContext
{
    public CourierServiceContext()
    {
    }

    public CourierServiceContext(DbContextOptions<CourierServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Courier> Couriers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderAcceptance> OrderAcceptances { get; set; }

    public virtual DbSet<OrderHistory> OrderHistories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=CourierService;Username=postgres;Password=123456");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("clients_pkey");

            entity.ToTable("clients");

            entity.HasIndex(e => e.Email, "clients_email_key").IsUnique();

            entity.HasIndex(e => e.TelephoneNumber, "clients_telephone_number_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.TelephoneNumber)
                .HasMaxLength(20)
                .HasColumnName("telephone_number");
        });

        modelBuilder.Entity<Courier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("couriers_pkey");

            entity.ToTable("couriers");

            entity.HasIndex(e => e.TelephoneNumber, "couriers_telephone_number_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.OrderPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("order_percentage");
            entity.Property(e => e.TelephoneNumber)
                .HasMaxLength(20)
                .HasColumnName("telephone_number");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DeparturePoint).HasColumnName("departure_point");
            entity.Property(e => e.DestinationPoint).HasColumnName("destination_point");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("fk_orders_client");

            entity.HasOne(d => d.DeparturePointNavigation).WithMany(p => p.OrderDeparturePointNavigations)
                .HasForeignKey(d => d.DeparturePoint)
                .HasConstraintName("fk_orders_departure");

            entity.HasOne(d => d.DestinationPointNavigation).WithMany(p => p.OrderDestinationPointNavigations)
                .HasForeignKey(d => d.DestinationPoint)
                .HasConstraintName("fk_orders_destination");
        });

        modelBuilder.Entity<OrderAcceptance>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("order_acceptance_pkey");

            entity.ToTable("order_acceptance");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.CourierId).HasColumnName("courier_id");
            entity.Property(e => e.Date).HasColumnName("date");

            entity.HasOne(d => d.Courier).WithMany(p => p.OrderAcceptances)
                .HasForeignKey(d => d.CourierId)
                .HasConstraintName("fk_order_acceptance_courier");

            entity.HasOne(d => d.Order).WithOne(p => p.OrderAcceptance)
                .HasForeignKey<OrderAcceptance>(d => d.OrderId)
                .HasConstraintName("fk_order_acceptance_order");
        });

        modelBuilder.Entity<OrderHistory>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.Date }).HasName("order_history_pkey");

            entity.ToTable("order_history");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderHistories)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_order_history_order");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Order).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.OrderId)
                .HasConstraintName("fk_payments_order");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("routes_pkey");

            entity.ToTable("routes");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_routes_parent");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
