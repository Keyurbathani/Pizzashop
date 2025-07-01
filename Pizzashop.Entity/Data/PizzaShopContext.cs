using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Entity.Data;

public partial class PizzaShopContext : DbContext
{
    public PizzaShopContext(DbContextOptions<PizzaShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<MappingMenuItemWithModifier> MappingMenuItemWithModifiers { get; set; }

    public virtual DbSet<MenuCategory> MenuCategories { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Modifier> Modifiers { get; set; }

    public virtual DbSet<ModifierAndGroup> ModifierAndGroups { get; set; }

    public virtual DbSet<ModifierGroup> ModifierGroups { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderTaxMapping> OrderTaxMappings { get; set; }

    public virtual DbSet<OrderedItem> OrderedItems { get; set; }

    public virtual DbSet<OrderedItemModifierMapping> OrderedItemModifierMappings { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TableOrderMapping> TableOrderMappings { get; set; }

    public virtual DbSet<TaxAndFee> TaxAndFees { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WaitingToken> WaitingTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("accounts_pkey");

            entity.ToTable("accounts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsFirstLogin)
                .IsRequired()
                .HasDefaultValueSql("true")
                .HasColumnName("is_first_login");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.TokenExpiry)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("token_expiry");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AccountCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("accounts_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.AccountModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("accounts_modified_by_fkey");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("accounts_role_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.AccountUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("accounts_user_id_fkey");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("city_pkey");

            entity.ToTable("city");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
            entity.Property(e => e.StateId).HasColumnName("state_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CityCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("city_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.CityModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("city_modified_by_fkey");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("city_state_id_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("country_pkey");

            entity.ToTable("country");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CountryCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("country_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.CountryModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("country_modified_by_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_pkey");

            entity.ToTable("customer");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone).HasColumnName("phone");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CustomerCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.CustomerModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("modified_by_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("feedback_pkey");

            entity.ToTable("feedback");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ambience).HasColumnName("ambience");
            entity.Property(e => e.AvgRating).HasColumnName("avg_rating")
            .HasPrecision(18, 1);
            entity.Property(e => e.Comments)
                .HasMaxLength(100)
                .HasColumnName("comments");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Food).HasColumnName("food");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Service).HasColumnName("service");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.FeedbackCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("feedback_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.FeedbackModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("feedback_modified_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("feedback_order_id_fkey");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invoice_pkey");

            entity.ToTable("invoice");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.QuantityOfModifier).HasColumnName("quantity_of_modifier");
            entity.Property(e => e.RateOfModifier)
                .HasPrecision(18, 2)
                .HasColumnName("rate_of_modifier");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvoiceCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("invoice_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InvoiceModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("invoice_modified_by_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.ModifierId)
                .HasConstraintName("invoice_modifier_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoice_order_id_fkey");
        });

        modelBuilder.Entity<MappingMenuItemWithModifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mapping_menu_item_with_modifier_pkey");

            entity.ToTable("mapping_menu_item_with_modifier");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.MaxSelectionRequired).HasColumnName("max_selection_required");
            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.MinSelectionRequired).HasColumnName("min_selection_required");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifierGroupId).HasColumnName("modifier_group_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MappingMenuItemWithModifierCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("mapping_menu_item_with_modifier_created_by_fkey");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.MappingMenuItemWithModifiers)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("mapping_menu_item_with_modifier_menu_item_id_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.MappingMenuItemWithModifierModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("mapping_menu_item_with_modifier_modified_by_fkey");

            entity.HasOne(d => d.ModifierGroup).WithMany(p => p.MappingMenuItemWithModifiers)
                .HasForeignKey(d => d.ModifierGroupId)
                .HasConstraintName("mapping_menu_item_with_modifier_modifier_group_id_fkey");
        });

        modelBuilder.Entity<MenuCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("menu_category_pkey");

            entity.ToTable("menu_category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MenuCategoryCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("menu_category_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.MenuCategoryModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("menu_category_modified_by_fkey");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("menu_items_pkey");

            entity.ToTable("menu_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValueSql("true")
                .HasColumnName("is_available");
            entity.Property(e => e.IsDefaultTax)
                .HasDefaultValueSql("false")
                .HasColumnName("is_default_tax");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsFavourite)
                .HasDefaultValueSql("false")
                .HasColumnName("is_favourite");
            entity.Property(e => e.ItemType).HasColumnName("item_type");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.ProfileImage)
                .HasMaxLength(200)
                .HasColumnName("profile_image");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("1")
                .HasColumnName("quantity");
            entity.Property(e => e.Rate)
                .HasPrecision(18, 2)
                .HasColumnName("rate");
            entity.Property(e => e.ShortCode)
                .HasMaxLength(50)
                .HasColumnName("short_code");
            entity.Property(e => e.TaxPercentage)
                .HasPrecision(18, 2)
                .HasColumnName("tax_percentage");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.Category).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("menu_items_category_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MenuItemCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("menu_items_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.MenuItemModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("menu_items_modified_by_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("menu_items_unit_id_fkey");
        });

        modelBuilder.Entity<Modifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifier_pkey");

            entity.ToTable("modifier");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Rate)
                .HasPrecision(18, 2)
                .HasColumnName("rate");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifierCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("modifier_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ModifierModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("modifier_modified_by_fkey");

            entity.HasOne(d => d.Unit).WithMany(p => p.Modifiers)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modifier_unit_id_fkey");
        });

        modelBuilder.Entity<ModifierAndGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifier_and_group_pkey");

            entity.ToTable("modifier_and_group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.ModifiergroupId).HasColumnName("modifiergroup_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifierAndGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("modifier_and_group_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ModifierAndGroupModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("modifier_and_group_modified_by_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.ModifierAndGroups)
                .HasForeignKey(d => d.ModifierId)
                .HasConstraintName("modifier_and_group_modifier_id_fkey");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.ModifierAndGroups)
                .HasForeignKey(d => d.ModifiergroupId)
                .HasConstraintName("modifier_and_group_modifiergroup_id_fkey");
        });

        modelBuilder.Entity<ModifierGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modifier_group_pkey");

            entity.ToTable("modifier_group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModifierGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("modifier_group_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ModifierGroupModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("modifier_group_modified_by_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Discount)
                .HasPrecision(18, 2)
                .HasColumnName("discount");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsSgstSelected)
                .HasDefaultValueSql("false")
                .HasColumnName("is_sgst_selected");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.OrderNo).HasColumnName("order_no");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(50)
                .HasColumnName("order_status");
            entity.Property(e => e.PaidAmount)
                .HasPrecision(18, 2)
                .HasColumnName("paid_amount");
            entity.Property(e => e.SubTotal)
                .HasPrecision(18, 2)
                .HasColumnName("sub_total");
            entity.Property(e => e.Tax)
                .HasPrecision(18, 2)
                .HasColumnName("tax");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("orders_created_by_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_customer_id_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.OrderModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("orders_modified_by_fkey");
        });

        modelBuilder.Entity<OrderTaxMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_tax_mapping_pkey");

            entity.ToTable("order_tax_mapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TaxId).HasColumnName("tax_id");
            entity.Property(e => e.TaxName)
                .HasMaxLength(100)
                .HasColumnName("tax_name");
            entity.Property(e => e.TaxType).HasColumnName("tax_type");
            entity.Property(e => e.TaxValue)
                .HasPrecision(18, 2)
                .HasColumnName("tax_value");
            entity.Property(e => e.TotalTax)
                .HasPrecision(18, 2)
                .HasColumnName("total_tax");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderTaxMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("order_tax_mapping_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.OrderTaxMappingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("order_tax_mapping_modified_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTaxMappings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tax_mapping_order_id_fkey");

            entity.HasOne(d => d.Tax).WithMany(p => p.OrderTaxMappings)
                .HasForeignKey(d => d.TaxId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tax_mapping_tax_id_fkey");
        });

        modelBuilder.Entity<OrderedItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ordered_items_pkey");

            entity.ToTable("ordered_items");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Instruction).HasColumnName("instruction");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .HasColumnName("item_name");
            entity.Property(e => e.ItemRate)
                .HasPrecision(18, 2)
                .HasColumnName("item_rate");
            entity.Property(e => e.ItemTotal)
                .HasPrecision(18, 2)
                .HasColumnName("item_total");
            entity.Property(e => e.MenuItemId).HasColumnName("menu_item_id");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ReadyItemQuantity)
                .HasDefaultValueSql("0")
                .HasColumnName("ready_item_quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasColumnName("status");
            entity.Property(e => e.Tax)
                .HasPrecision(18, 2)
                .HasColumnName("tax");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.TotalModifierAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_modifier_amount");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderedItemCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("ordered_items_created_by_fkey");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.OrderedItems)
                .HasForeignKey(d => d.MenuItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_items_menu_item_id_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.OrderedItemModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("ordered_items_modified_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderedItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_items_order_id_fkey");
        });

        modelBuilder.Entity<OrderedItemModifierMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ordered_item_modifier_mapping_pkey");

            entity.ToTable("ordered_item_modifier_mapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifierId).HasColumnName("modifier_id");
            entity.Property(e => e.ModifierName)
                .HasMaxLength(100)
                .HasColumnName("modifier_name");
            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.QuantityOfModifier).HasColumnName("quantity_of_modifier");
            entity.Property(e => e.RateOfModifier)
                .HasPrecision(18, 2)
                .HasColumnName("rate_of_modifier");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrderedItemModifierMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("ordered_item_modifier_mapping_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.OrderedItemModifierMappingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("ordered_item_modifier_mapping_modified_by_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.OrderedItemModifierMappings)
                .HasForeignKey(d => d.ModifierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_item_modifier_mapping_modifier_id_fkey");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderedItemModifierMappings)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ordered_item_modifier_mapping_order_item_id_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PaymentCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("payments_created_by_fkey");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_invoice_id_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PaymentModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("payments_modified_by_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ControllerName)
                .HasMaxLength(80)
                .HasColumnName("controller_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PermissionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("permissions_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.PermissionModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("permissions_modified_by_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoleCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("role_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.RoleModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("role_modified_by_fkey");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_permissions_pkey");

            entity.ToTable("role_permissions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CanAdd)
                .HasDefaultValueSql("false")
                .HasColumnName("can_add");
            entity.Property(e => e.CanDelete)
                .HasDefaultValueSql("false")
                .HasColumnName("can_delete");
            entity.Property(e => e.CanView)
                .HasDefaultValueSql("false")
                .HasColumnName("can_view");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RolePermissionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("role_permissions_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.RolePermissionModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("role_permissions_modified_by_fkey");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_permissions_permission_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_permissions_role_id_fkey");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("section_pkey");

            entity.ToTable("section");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SectionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("section_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.SectionModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("section_modified_by_fkey");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("state_pkey");

            entity.ToTable("state");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("state_country_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StateCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("state_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.StateModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("state_modified_by_fkey");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValueSql("true")
                .HasColumnName("is_available");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.SectionId).HasColumnName("section_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TableCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tables_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TableModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tables_modified_by_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.Tables)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tables_section_id_fkey");
        });

        modelBuilder.Entity<TableOrderMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("table_order_mapping_pkey");

            entity.ToTable("table_order_mapping");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.NoOfPersons).HasColumnName("no_of_persons");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TableId).HasColumnName("table_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TableOrderMappingCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("table_order_mapping_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TableOrderMappingModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("table_order_mapping_modified_by_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.TableOrderMappings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("table_order_mapping_order_id_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.TableOrderMappings)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("table_order_mapping_table_id_fkey");
        });

        modelBuilder.Entity<TaxAndFee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tax_and_fees_pkey");

            entity.ToTable("tax_and_fees");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.FlatAmount)
                .HasPrecision(18, 2)
                .HasColumnName("flat_amount");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("false")
                .HasColumnName("is_active");
            entity.Property(e => e.IsDefault)
                .HasDefaultValueSql("false")
                .HasColumnName("is_default");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Percentage)
                .HasPrecision(18, 2)
                .HasColumnName("percentage");
            entity.Property(e => e.TaxType)
                .HasDefaultValueSql("true")
                .HasColumnName("tax_type");
            entity.Property(e => e.TaxValue)
                .HasPrecision(18, 2)
                .HasColumnName("tax_value");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TaxAndFeeCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("tax_and_fees_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TaxAndFeeModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("tax_and_fees_modified_by_fkey");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("units_pkey");

            entity.ToTable("units");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.ShortName)
                .HasMaxLength(50)
                .HasColumnName("short_name");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UnitCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("units_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.UnitModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("units_modified_by_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FisrtName)
                .HasMaxLength(50)
                .HasColumnName("fisrt_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("true")
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.IsFirstLogin)
                .HasDefaultValueSql("true")
                .HasColumnName("is_first_login");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .HasColumnName("password");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.ProfileImage)
                .HasMaxLength(200)
                .HasColumnName("profile_image");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.TokenExpiary)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("token_expiary");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.Zipcode).HasColumnName("zipcode");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("users_city_id_fkey");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("users_country_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("users_created_by_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InverseModifiedByNavigation)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("users_modified_by_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_role_id");

            entity.HasOne(d => d.State).WithMany(p => p.Users)
                .HasForeignKey(d => d.StateId)
                .HasConstraintName("users_state_id_fkey");
        });

        modelBuilder.Entity<WaitingToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("waiting_tokens_pkey");

            entity.ToTable("waiting_tokens");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsAssigned)
                .HasDefaultValueSql("false")
                .HasColumnName("is_assigned");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValueSql("false")
                .HasColumnName("is_deleted");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.NoOfPersons).HasColumnName("no_of_persons");
            entity.Property(e => e.SectionId).HasColumnName("section_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.WaitingTokenCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("waiting_tokens_created_by_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.WaitingTokens)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_tokens_customer_id_fkey");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.WaitingTokenModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("waiting_tokens_modified_by_fkey");

            entity.HasOne(d => d.Section).WithMany(p => p.WaitingTokens)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_tokens_section_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
