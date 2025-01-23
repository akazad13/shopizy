﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shopizy.Infrastructure.Common.Persistence;

#nullable disable

namespace shopizy.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240401075111_UpdateCartTables")]
    partial class UpdateCartTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Shopizy.Domain.Carts.Cart", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Carts", (string)null);
                });

            modelBuilder.Entity("Shopizy.Domain.Categories.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("Shopizy.Domain.Customers.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Customers", (string)null);
                });

            modelBuilder.Entity("Shopizy.Domain.Orders.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("int");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("PromoCode")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orders", (string)null);
                });

            modelBuilder.Entity("Shopizy.Domain.Payments.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("PaymentStatus")
                        .HasColumnType("int");

                    b.Property<string>("TransactionId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Payments", (string)null);
                });

            modelBuilder.Entity("Shopizy.Domain.ProductReviews.ProductReview", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("ProductReviews", (string)null);
                });

            modelBuilder.Entity("Shopizy.Domain.Products.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Barcode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Brand")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("BreadCrums")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<decimal?>("Discount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("StockQuantity")
                        .HasColumnType("int");

                    b.Property<string>("Tags")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("Shopizy.Domain.PromoCodes.PromoCode", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("Discount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsPerchantage")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<int>("NumOfTimeUsed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("Code");

                    b.ToTable("PromoCodes", (string)null);
                });

            modelBuilder.Entity("Shopizy.Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("smalldatetime");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.HasIndex("Phone");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("Shopizy.Domain.Carts.Cart", b =>
                {
                    b.OwnsMany("Shopizy.Domain.Carts.Entities.LineItem", "LineItems", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("CartId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("ProductId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Quantity")
                                .HasColumnType("int");

                            b1.HasKey("Id", "CartId", "ProductId");

                            b1.HasIndex("CartId");

                            b1.HasIndex("ProductId");

                            b1.ToTable("LineItems", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("CartId");

                            b1.HasOne("Shopizy.Domain.Products.Product", "Product")
                                .WithMany()
                                .HasForeignKey("ProductId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.Navigation("Product");
                        });

                    b.Navigation("LineItems");
                });

            modelBuilder.Entity("Shopizy.Domain.Customers.Customer", b =>
                {
                    b.HasOne("Shopizy.Domain.Users.User", "User")
                        .WithOne()
                        .HasForeignKey("Shopizy.Domain.Customers.Customer", "UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.OwnsOne("Shopizy.Domain.Orders.ValueObjects.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("CustomerId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("City")
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.Property<string>("Country")
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.Property<string>("Line")
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("State")
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.Property<string>("ZipCode")
                                .HasMaxLength(10)
                                .HasColumnType("nvarchar(10)");

                            b1.HasKey("CustomerId");

                            b1.ToTable("Customers");

                            b1.WithOwner()
                                .HasForeignKey("CustomerId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Shopizy.Domain.Orders.Order", b =>
                {
                    b.HasOne("Shopizy.Domain.Users.User", null)
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Shopizy.Domain.Orders.Entities.OrderItem", "OrderItems", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("Discount")
                                .HasPrecision(18, 2)
                                .HasColumnType("decimal(18,2)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("PictureUrl")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Quantity")
                                .HasColumnType("int");

                            b1.HasKey("Id", "OrderId");

                            b1.HasIndex("OrderId");

                            b1.ToTable("OrderItems", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("OrderId");

                            b1.OwnsOne("Shopizy.Domain.Common.ValueObjects.Price", "UnitPrice", b2 =>
                                {
                                    b2.Property<Guid>("OrderItemId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<Guid>("OrderItemOrderId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<decimal>("Amount")
                                        .HasPrecision(18, 2)
                                        .HasColumnType("decimal(18,2)");

                                    b2.Property<int>("Currency")
                                        .HasColumnType("int");

                                    b2.HasKey("OrderItemId", "OrderItemOrderId");

                                    b2.ToTable("OrderItems");

                                    b2.WithOwner()
                                        .HasForeignKey("OrderItemId", "OrderItemOrderId");
                                });

                            b1.Navigation("UnitPrice")
                                .IsRequired();
                        });

                    b.OwnsOne("Shopizy.Domain.Common.ValueObjects.Price", "DeliveryCharge", b1 =>
                        {
                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("Amount")
                                .HasPrecision(18, 2)
                                .HasColumnType("decimal(18,2)");

                            b1.Property<int>("Currency")
                                .HasColumnType("int");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.OwnsOne("Shopizy.Domain.Orders.ValueObjects.Address", "ShippingAddress", b1 =>
                        {
                            b1.Property<Guid>("OrderId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.Property<string>("Line")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("State")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("nvarchar(10)");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.Navigation("DeliveryCharge")
                        .IsRequired();

                    b.Navigation("OrderItems");

                    b.Navigation("ShippingAddress")
                        .IsRequired();
                });

            modelBuilder.Entity("Shopizy.Domain.Payments.Payment", b =>
                {
                    b.HasOne("Shopizy.Domain.Orders.Order", "Order")
                        .WithOne()
                        .HasForeignKey("Shopizy.Domain.Payments.Payment", "OrderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Shopizy.Domain.Users.User", "User")
                        .WithOne()
                        .HasForeignKey("Shopizy.Domain.Payments.Payment", "UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.OwnsOne("Shopizy.Domain.Orders.ValueObjects.Address", "BillingAddress", b1 =>
                        {
                            b1.Property<Guid>("PaymentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.Property<string>("Line")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("State")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("nvarchar(30)");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("nvarchar(10)");

                            b1.HasKey("PaymentId");

                            b1.ToTable("Payments");

                            b1.WithOwner()
                                .HasForeignKey("PaymentId");
                        });

                    b.OwnsOne("Shopizy.Domain.Common.ValueObjects.Price", "Total", b1 =>
                        {
                            b1.Property<Guid>("PaymentId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("Amount")
                                .HasPrecision(18, 2)
                                .HasColumnType("decimal(18,2)");

                            b1.Property<int>("Currency")
                                .HasColumnType("int");

                            b1.HasKey("PaymentId");

                            b1.ToTable("Payments");

                            b1.WithOwner()
                                .HasForeignKey("PaymentId");
                        });

                    b.Navigation("BillingAddress")
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Total")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Shopizy.Domain.ProductReviews.ProductReview", b =>
                {
                    b.HasOne("Shopizy.Domain.Products.Product", null)
                        .WithMany("ProductReviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Shopizy.Domain.Users.User", null)
                        .WithMany("ProductReviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Shopizy.Domain.Common.ValueObjects.Rating", "Rating", b1 =>
                        {
                            b1.Property<Guid>("ProductReviewId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<double>("Value")
                                .HasPrecision(18, 2)
                                .HasColumnType("float(18)");

                            b1.HasKey("ProductReviewId");

                            b1.ToTable("ProductReviews");

                            b1.WithOwner()
                                .HasForeignKey("ProductReviewId");
                        });

                    b.Navigation("Rating")
                        .IsRequired();
                });

            modelBuilder.Entity("Shopizy.Domain.Products.Product", b =>
                {
                    b.HasOne("Shopizy.Domain.Categories.Category", null)
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.OwnsOne("Shopizy.Domain.Common.ValueObjects.AverageRating", "AverageRating", b1 =>
                        {
                            b1.Property<Guid>("ProductId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("NumRatings")
                                .HasColumnType("int");

                            b1.Property<double>("Value")
                                .HasPrecision(18, 2)
                                .HasColumnType("float(18)");

                            b1.HasKey("ProductId");

                            b1.ToTable("Products");

                            b1.WithOwner()
                                .HasForeignKey("ProductId");
                        });

                    b.OwnsMany("Shopizy.Domain.Products.Entities.ProductImage", "ProductImages", b1 =>
                        {
                            b1.Property<Guid>("ProductId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImageUrl")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PublicId")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<int>("Seq")
                                .HasColumnType("int");

                            b1.HasKey("ProductId", "Id");

                            b1.ToTable("ProductImages", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("ProductId");
                        });

                    b.OwnsOne("Shopizy.Domain.Common.ValueObjects.Price", "UnitPrice", b1 =>
                        {
                            b1.Property<Guid>("ProductId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<decimal>("Amount")
                                .HasPrecision(18, 2)
                                .HasColumnType("decimal(18,2)");

                            b1.Property<int>("Currency")
                                .HasColumnType("int");

                            b1.HasKey("ProductId");

                            b1.ToTable("Products");

                            b1.WithOwner()
                                .HasForeignKey("ProductId");
                        });

                    b.Navigation("AverageRating")
                        .IsRequired();

                    b.Navigation("ProductImages");

                    b.Navigation("UnitPrice")
                        .IsRequired();
                });

            modelBuilder.Entity("Shopizy.Domain.Categories.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Shopizy.Domain.Products.Product", b =>
                {
                    b.Navigation("ProductReviews");
                });

            modelBuilder.Entity("Shopizy.Domain.Users.User", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("ProductReviews");
                });
#pragma warning restore 612, 618
        }
    }
}
