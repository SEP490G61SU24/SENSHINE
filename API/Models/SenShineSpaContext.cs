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
        public virtual DbSet<Card> Cards { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Combo> Combos { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<Promotion> Promotions { get; set; } = null!;
        public virtual DbSet<Province> Provinces { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<Salary> Salaries { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<Spa> Spas { get; set; } = null!;
        public virtual DbSet<SystemSetting> SystemSettings { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;
        public virtual DbSet<WorkSchedule> WorkSchedules { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("server=DESKTOP-SF3JEMS\\SQLEXPRESS;database=SenShineSpa;uid=sa;pwd=1;TrustServerCertificate=true;Trusted_Connection=True;");
//            }
//        }

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

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.AppointmentCustomers)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__Custo__6D0D32F4");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.AppointmentEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__Emplo__6E01572D");

                entity.HasMany(d => d.Products)
                    .WithMany(p => p.Appointments)
                    .UsingEntity<Dictionary<string, object>>(
                        "AppointmentProduct",
                        l => l.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Appointme__Produ__75A278F5"),
                        r => r.HasOne<Appointment>().WithMany().HasForeignKey("AppointmentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Appointme__Appoi__74AE54BC"),
                        j =>
                        {
                            j.HasKey("AppointmentId", "ProductId").HasName("PK__Appointm__458D30AE75CC4618");

                            j.ToTable("Appointment_Product");
                        });

                entity.HasMany(d => d.Services)
                    .WithMany(p => p.Appointments)
                    .UsingEntity<Dictionary<string, object>>(
                        "AppointmentService",
                        l => l.HasOne<Service>().WithMany().HasForeignKey("ServiceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Appointme__Servi__71D1E811"),
                        r => r.HasOne<Appointment>().WithMany().HasForeignKey("AppointmentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Appointme__Appoi__70DDC3D8"),
                        j =>
                        {
                            j.HasKey("AppointmentId", "ServiceId").HasName("PK__Appointm__329C47C21E1B1639");

                            j.ToTable("Appointment_Service");
                        });
            });

            modelBuilder.Entity<Bed>(entity =>
            {
                entity.ToTable("Bed");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BedNumber).HasMaxLength(50);
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.ToTable("Card");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CardNumber).HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(15, 2)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Cards)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Card__CustomerId__45F365D3");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryName).HasMaxLength(100);
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

                entity.Property(e => e.SalePrice).HasColumnType("decimal(15, 2)");

                entity.HasMany(d => d.Cards)
                    .WithMany(p => p.Combos)
                    .UsingEntity<Dictionary<string, object>>(
                        "CardCombo",
                        l => l.HasOne<Card>().WithMany().HasForeignKey("CardId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Card_Comb__CardI__4D94879B"),
                        r => r.HasOne<Combo>().WithMany().HasForeignKey("ComboId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Card_Comb__Combo__4CA06362"),
                        j =>
                        {
                            j.HasKey("ComboId", "CardId").HasName("PK__Card_Com__281DB4F404ED62D8");

                            j.ToTable("Card_Combo");
                        });

                entity.HasMany(d => d.Services)
                    .WithMany(p => p.Combos)
                    .UsingEntity<Dictionary<string, object>>(
                        "ComboService",
                        l => l.HasOne<Service>().WithMany().HasForeignKey("ServiceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Combo_Ser__Servi__49C3F6B7"),
                        r => r.HasOne<Combo>().WithMany().HasForeignKey("ComboId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Combo_Ser__Combo__48CFD27E"),
                        j =>
                        {
                            j.HasKey("ComboId", "ServiceId").HasName("PK__Combo_Se__6113E32E0F34A797");

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

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Invoice__Custome__08B54D69");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.PromotionId)
                    .HasConstraintName("FK__Invoice__Promoti__09A971A2");

                entity.HasOne(d => d.Spa)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.SpaId)
                    .HasConstraintName("FK__Invoice__SpaId__07C12930");

                entity.HasMany(d => d.Cards)
                    .WithMany(p => p.Invoices)
                    .UsingEntity<Dictionary<string, object>>(
                        "InvoiceCard",
                        l => l.HasOne<Card>().WithMany().HasForeignKey("CardId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Invoice_C__CardI__114A936A"),
                        r => r.HasOne<Invoice>().WithMany().HasForeignKey("InvoiceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Invoice_C__Invoi__10566F31"),
                        j =>
                        {
                            j.HasKey("InvoiceId", "CardId").HasName("PK__Invoice___22C9466F49C63A8D");

                            j.ToTable("Invoice_Card");
                        });

                entity.HasMany(d => d.Services)
                    .WithMany(p => p.Invoices)
                    .UsingEntity<Dictionary<string, object>>(
                        "InvoiceService",
                        l => l.HasOne<Service>().WithMany().HasForeignKey("ServiceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Invoice_S__Servi__0D7A0286"),
                        r => r.HasOne<Invoice>().WithMany().HasForeignKey("InvoiceId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Invoice_S__Invoi__0C85DE4D"),
                        j =>
                        {
                            j.HasKey("InvoiceId", "ServiceId").HasName("PK__Invoice___6BC711B5BC4C1036");

                            j.ToTable("Invoice_Service");
                        });
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
                    .HasConstraintName("FK__Notificat__UserI__01142BA1");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductName).HasMaxLength(100);

                entity.HasMany(d => d.Categories)
                    .WithMany(p => p.Products)
                    .UsingEntity<Dictionary<string, object>>(
                        "ProductCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ProductCa__Categ__656C112C"),
                        r => r.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__ProductCa__Produ__6477ECF3"),
                        j =>
                        {
                            j.HasKey("ProductId", "CategoryId").HasName("PK__ProductC__159C556DAB72EC3C");

                            j.ToTable("ProductCategories");
                        });
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ProductImage");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(1000)
                    .HasColumnName("ImageURL");
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
                    .HasConstraintName("FK__Promotion__SpaId__03F0984C");
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
                    .HasConstraintName("FK__Reviews__Custome__797309D9");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK__Reviews__Service__7A672E12");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RoomName).HasMaxLength(100);

                entity.HasMany(d => d.Beds)
                    .WithMany(p => p.Rooms)
                    .UsingEntity<Dictionary<string, object>>(
                        "RoomBed",
                        l => l.HasOne<Bed>().WithMany().HasForeignKey("BedId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Room_Bed__BedId__5AEE82B9"),
                        r => r.HasOne<Room>().WithMany().HasForeignKey("RoomId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Room_Bed__RoomId__59FA5E80"),
                        j =>
                        {
                            j.HasKey("RoomId", "BedId").HasName("PK__Room_Bed__280C483D5D07DC55");

                            j.ToTable("Room_Bed");
                        });
            });

            modelBuilder.Entity<Salary>(entity =>
            {
                entity.ToTable("Salary");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BaseSalary).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TotalSalary).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Salaries)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Salary__Employee__3F466844");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(15, 2)");

                entity.Property(e => e.ServiceName).HasMaxLength(100);
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

                entity.HasMany(d => d.Rooms)
                    .WithMany(p => p.Spas)
                    .UsingEntity<Dictionary<string, object>>(
                        "SpaRoom",
                        l => l.HasOne<Room>().WithMany().HasForeignKey("RoomId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Spa_Room__RoomId__5535A963"),
                        r => r.HasOne<Spa>().WithMany().HasForeignKey("SpaId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__Spa_Room__SpaId__5441852A"),
                        j =>
                        {
                            j.HasKey("SpaId", "RoomId").HasName("PK__Spa_Room__40685EAB55E2E877");

                            j.ToTable("Spa_Room");
                        });
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

                entity.Property(e => e.DistrictCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("district_code");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MidName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("province_code");

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
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__User_Role__RoleI__3C69FB99"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__User_Role__UserI__3B75D760"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PK__User_Rol__AF2760AD96E069FB");

                            j.ToTable("User_Role");
                        });
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

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.WorkSchedules)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK__WorkSched__Emplo__7D439ABD");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
