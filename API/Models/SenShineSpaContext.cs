﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Models
{
    public partial class SenShineSpaContext : DbContext
    {
        public SenShineSpaContext()
        {
        }

        public SenShineSpaContext(DbContextOptions<SenShineSpaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdministrativeRegion> AdministrativeRegions { get; set; } = null!;
        public virtual DbSet<AdministrativeUnit> AdministrativeUnits { get; set; } = null!;
        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<Bed> Beds { get; set; } = null!;
        public virtual DbSet<BedSlot> BedSlots { get; set; } = null!;
        public virtual DbSet<Card> Cards { get; set; } = null!;
        public virtual DbSet<CardCombo> CardCombos { get; set; } = null!;
        public virtual DbSet<CardInvoice> CardInvoices { get; set; } = null!;
        public virtual DbSet<Combo> Combos { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<InvoiceCombo> InvoiceCombos { get; set; } = null!;
        public virtual DbSet<InvoiceService> InvoiceServices { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Promotion> Promotions { get; set; } = null!;
        public virtual DbSet<Province> Provinces { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<Rule> Rules { get; set; } = null!;
        public virtual DbSet<Salary> Salaries { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<Slot> Slots { get; set; } = null!;
        public virtual DbSet<Spa> Spas { get; set; } = null!;
        public virtual DbSet<SystemSetting> SystemSettings { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserSlot> UserSlots { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;
        public virtual DbSet<WorkSchedule> WorkSchedules { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=SenShineSpa;User Id=sa;Password=12345678;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdministrativeRegion>(entity =>
            {
                entity.ToTable("administrative_regions");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.CodeNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("code_name_en");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");
            });

            modelBuilder.Entity<AdministrativeUnit>(entity =>
            {
                entity.ToTable("administrative_units");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.CodeNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("code_name_en");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_en");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(255)
                    .HasColumnName("short_name");

                entity.Property(e => e.ShortNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("short_name_en");
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppointmentDate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('PENDING')");

                entity.HasOne(d => d.Bed)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.BedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__BedId__3F073C79");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.AppointmentCustomers)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__Custo__3D1EF407");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.AppointmentEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__Emplo__3E131840");

                entity.HasOne(d => d.Slot)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.SlotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__SlotI__3FFB60B2");

                entity.HasMany(d => d.Combos)
                    .WithMany(p => p.Appointments)
                    .UsingEntity<Dictionary<string, object>>(
                        "AppointmentCombo",
                        l => l.HasOne<Combo>().WithMany().HasForeignKey("ComboId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Appointme__Combo__479C827A"),
                        r => r.HasOne<Appointment>().WithMany().HasForeignKey("AppointmentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Appointme__Appoi__46A85E41"),
                        j =>
                        {
                            j.HasKey("AppointmentId", "ComboId").HasName("PK__Appointm__7319D94032FF361B");

                            j.ToTable("Appointment_Combo");
                        });

                entity.HasMany(d => d.Services)
                    .WithMany(p => p.Appointments)
                    .UsingEntity<Dictionary<string, object>>(
                        "AppointmentService",
                        l => l.HasOne<Service>().WithMany().HasForeignKey("ServiceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Appointme__Servi__43CBF196"),
                        r => r.HasOne<Appointment>().WithMany().HasForeignKey("AppointmentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Appointme__Appoi__42D7CD5D"),
                        j =>
                        {
                            j.HasKey("AppointmentId", "ServiceId").HasName("PK__Appointm__329C47C211C72467");

                            j.ToTable("Appointment_Service");
                        });
            });

            modelBuilder.Entity<Bed>(entity =>
            {
                entity.ToTable("Bed");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BedNumber).HasMaxLength(50);

                entity.Property(e => e.StatusWorking)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('INACTIVE')");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Beds)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bed__RoomId__1DA648AE");
            });

            modelBuilder.Entity<BedSlot>(entity =>
            {
                entity.ToTable("BedSlot");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SlotDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Bed)
                    .WithMany(p => p.BedSlots)
                    .HasForeignKey(d => d.BedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BedSlot__BedId__3489AE06");

                entity.HasOne(d => d.Slot)
                    .WithMany(p => p.BedSlots)
                    .HasForeignKey(d => d.SlotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BedSlot__SlotId__357DD23F");
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.ToTable("Card");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CardNumber).HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Cards)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Card__BranchId__2176D992");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Cards)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Card__CustomerId__2082B559");
            });

            modelBuilder.Entity<CardCombo>(entity =>
            {
                entity.ToTable("Card_Combo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasOne(d => d.Card)
                    .WithMany(p => p.CardCombos)
                    .HasForeignKey(d => d.CardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Card_Comb__CardI__2823D721");

                entity.HasOne(d => d.Combo)
                    .WithMany(p => p.CardCombos)
                    .HasForeignKey(d => d.ComboId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Card_Comb__Combo__2917FB5A");
            });

            modelBuilder.Entity<CardInvoice>(entity =>
            {
                entity.ToTable("Card_Invoice");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasOne(d => d.Card)
                    .WithMany(p => p.CardInvoices)
                    .HasForeignKey(d => d.CardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Card_Invo__CardI__6438C128");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.CardInvoices)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Card_Invo__Invoi__652CE561");
            });

            modelBuilder.Entity<Combo>(entity =>
            {
                entity.ToTable("Combo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Discount)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("discount");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Note)
                    .HasMaxLength(50)
                    .HasColumnName("note");

                entity.Property(e => e.Price).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SalePrice).HasColumnType("decimal(15, 2)");

                entity.HasMany(d => d.Services)
                    .WithMany(p => p.Combos)
                    .UsingEntity<Dictionary<string, object>>(
                        "ComboService",
                        l => l.HasOne<Service>().WithMany().HasForeignKey("ServiceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Combo_Ser__Servi__25476A76"),
                        r => r.HasOne<Combo>().WithMany().HasForeignKey("ComboId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Combo_Ser__Combo__2453463D"),
                        j =>
                        {
                            j.HasKey("ComboId", "ServiceId").HasName("PK__Combo_Se__6113E32EC98DEFB0");

                            j.ToTable("Combo_Service");
                        });
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("districts_pkey");

                entity.ToTable("districts");

                entity.HasIndex(e => e.ProvinceCode, "idx_districts_province");

                entity.HasIndex(e => e.AdministrativeUnitId, "idx_districts_unit");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .HasColumnName("code");

                entity.Property(e => e.AdministrativeUnitId).HasColumnName("administrative_unit_id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_en");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.ProvinceCode)
                    .HasMaxLength(20)
                    .HasColumnName("province_code");

                entity.HasOne(d => d.AdministrativeUnit)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.AdministrativeUnitId)
                    .HasConstraintName("districts_administrative_unit_id_fkey");

                entity.HasOne(d => d.ProvinceCodeNavigation)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.ProvinceCode)
                    .HasConstraintName("districts_province_code_fkey");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImagePath).HasMaxLength(1000);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(1000)
                    .HasColumnName("ImageURL");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoice");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.InvoiceDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('Pending')");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Invoice__Custome__5BA37B27");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.PromotionId)
                    .HasConstraintName("FK__Invoice__Promoti__5C979F60");

                entity.HasOne(d => d.Spa)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.SpaId)
                    .HasConstraintName("FK__Invoice__SpaId__5AAF56EE");
            });

            modelBuilder.Entity<InvoiceCombo>(entity =>
            {
                entity.HasKey(e => new { e.InvoiceId, e.ComboId })
                    .HasName("PK__Invoice___2A428F37087BE7F3");

                entity.ToTable("Invoice_Combo");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Combo)
                    .WithMany(p => p.InvoiceCombos)
                    .HasForeignKey(d => d.ComboId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice_C__Combo__69F19A7E");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceCombos)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice_C__Invoi__68FD7645");
            });

            modelBuilder.Entity<InvoiceService>(entity =>
            {
                entity.HasKey(e => new { e.InvoiceId, e.ServiceId })
                    .HasName("PK__Invoice___6BC711B5235EE7D2");

                entity.ToTable("Invoice_Service");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 0)")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceServices)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice_S__Invoi__60683044");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.InvoiceServices)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice_S__Servi__615C547D");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cover).HasMaxLength(1000);

                entity.Property(e => e.PublishedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title).HasMaxLength(1000);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsRead).HasColumnName("Is_Read");

                entity.Property(e => e.NotificationDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Notificat__UserI__530E3526");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.PromotionName).HasMaxLength(100);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Spa)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.SpaId)
                    .HasConstraintName("FK__Promotion__SpaId__55EAA1D1");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("provinces_pkey");

                entity.ToTable("provinces");

                entity.HasIndex(e => e.AdministrativeRegionId, "idx_provinces_region");

                entity.HasIndex(e => e.AdministrativeUnitId, "idx_provinces_unit");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .HasColumnName("code");

                entity.Property(e => e.AdministrativeRegionId).HasColumnName("administrative_region_id");

                entity.Property(e => e.AdministrativeUnitId).HasColumnName("administrative_unit_id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_en");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.HasOne(d => d.AdministrativeRegion)
                    .WithMany(p => p.Provinces)
                    .HasForeignKey(d => d.AdministrativeRegionId)
                    .HasConstraintName("provinces_administrative_region_id_fkey");

                entity.HasOne(d => d.AdministrativeUnit)
                    .WithMany(p => p.Provinces)
                    .HasForeignKey(d => d.AdministrativeUnitId)
                    .HasConstraintName("provinces_administrative_unit_id_fkey");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ReviewDate).HasColumnType("date");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Reviews__Custome__4B6D135E");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK__Reviews__Service__4C613797");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RoleName).HasMaxLength(50);

                entity.Property(e => e.Rules).HasMaxLength(1000);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RoomName).HasMaxLength(100);

                entity.HasOne(d => d.Spa)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.SpaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Room__SpaId__19D5B7CA");
            });

            modelBuilder.Entity<Rule>(entity =>
            {
                entity.ToTable("Rule");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Condition)
                    .HasMaxLength(50)
                    .HasColumnName("condition");

                entity.Property(e => e.Icon)
                    .HasMaxLength(50)
                    .HasColumnName("icon");

                entity.Property(e => e.Ismenu)
                    .HasColumnName("ismenu")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.Path)
                    .HasMaxLength(50)
                    .HasColumnName("path");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.Property(e => e.Remark)
                    .HasMaxLength(50)
                    .HasColumnName("remark");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");

                entity.Property(e => e.Url)
                    .HasMaxLength(50)
                    .HasColumnName("url");
            });

            modelBuilder.Entity<Salary>(entity =>
            {
                entity.ToTable("Salary");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Allowances).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BaseSalary).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Bonus).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Deductions).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TotalSalary).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Salaries)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Salary__Employee__114071C9");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.ServiceName).HasMaxLength(100);
            });

            modelBuilder.Entity<Slot>(entity =>
            {
                entity.ToTable("Slot");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SlotName).HasMaxLength(100);

                entity.Property(e => e.TimeFrom).HasColumnType("time(0)");

                entity.Property(e => e.TimeTo).HasColumnType("time(0)");
            });

            modelBuilder.Entity<Spa>(entity =>
            {
                entity.ToTable("Spa");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DistrictCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("district_code");

                entity.Property(e => e.ProvinceCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("province_code");

                entity.Property(e => e.SpaName).HasMaxLength(100);

                entity.Property(e => e.WardCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ward_code");
            });

            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");

                entity.Property(e => e.Key)
                    .HasMaxLength(255)
                    .HasColumnName("key");

                entity.Property(e => e.Value)
                    .HasMaxLength(1000)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DistrictCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("district_code");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.MidName).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("province_code");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('ACTIVE')");

                entity.Property(e => e.StatusWorking)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('AVAILABLE')");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WardCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ward_code");

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__User_Role__RoleI__0B879873"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__User_Role__UserI__0A93743A"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PK__User_Rol__AF2760ADBCBE49C4");

                            j.ToTable("User_Role");
                        });
            });

            modelBuilder.Entity<UserSlot>(entity =>
            {
                entity.ToTable("UserSlot");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.SlotDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Slot)
                    .WithMany(p => p.UserSlots)
                    .HasForeignKey(d => d.SlotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserSlot__SlotId__394E6323");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSlots)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserSlot__UserId__385A3EEA");
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("wards_pkey");

                entity.ToTable("wards");

                entity.HasIndex(e => e.DistrictCode, "idx_wards_district");

                entity.HasIndex(e => e.AdministrativeUnitId, "idx_wards_unit");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .HasColumnName("code");

                entity.Property(e => e.AdministrativeUnitId).HasColumnName("administrative_unit_id");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(255)
                    .HasColumnName("code_name");

                entity.Property(e => e.DistrictCode)
                    .HasMaxLength(20)
                    .HasColumnName("district_code");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEn)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_en");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.HasOne(d => d.AdministrativeUnit)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.AdministrativeUnitId)
                    .HasConstraintName("wards_administrative_unit_id_fkey");

                entity.HasOne(d => d.DistrictCodeNavigation)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.DistrictCode)
                    .HasConstraintName("wards_district_code_fkey");
            });

            modelBuilder.Entity<WorkSchedule>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DayOfWeek).HasMaxLength(20);

                entity.Property(e => e.EndDateTime).HasColumnType("datetime");

                entity.Property(e => e.StartDateTime).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.WorkSchedules)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK__WorkSched__Emplo__4F3DA442");
            });

            OnModelCreatingPartial(modelBuilder);
        }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}