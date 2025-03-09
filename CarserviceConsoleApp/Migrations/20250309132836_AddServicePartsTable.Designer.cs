﻿// <auto-generated />
using System;
using CarserviceConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarserviceConsoleApp.Migrations
{
    [DbContext(typeof(CarserviceContext))]
    [Migration("20250309132836_AddServicePartsTable")]
    partial class AddServicePartsTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CarserviceConsoleApp.Models.Car", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("brand");

                    b.Property<int>("ClientId")
                        .HasColumnType("int")
                        .HasColumnName("client_id");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("model");

                    b.Property<string>("Vin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("vin");

                    b.Property<DateOnly>("Year")
                        .HasColumnType("date")
                        .HasColumnName("year");

                    b.HasKey("Id")
                        .HasName("PK__Cars__3213E83FDD63F329");

                    b.HasIndex("ClientId");

                    b.HasIndex(new[] { "Id" }, "UQ__Cars__3213E83E9B763F19")
                        .IsUnique();

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("phone");

                    b.HasKey("Id")
                        .HasName("PK__Clients__3213E83FFE56F497");

                    b.HasIndex(new[] { "Id" }, "UQ__Clients__3213E83EF96C9753")
                        .IsUnique();

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("phone");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("position");

                    b.HasKey("Id")
                        .HasName("PK__Employee__3213E83FBEDFDAE0");

                    b.HasIndex(new[] { "Id" }, "UQ__Employee__3213E83E3CB6D2C8")
                        .IsUnique();

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Inventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("PartId")
                        .HasColumnType("int")
                        .HasColumnName("part_id");

                    b.Property<int>("Stock")
                        .HasColumnType("int")
                        .HasColumnName("stock");

                    b.HasKey("Id")
                        .HasName("PK__Inventor__3213E83FAD1B48CF");

                    b.HasIndex("PartId");

                    b.HasIndex(new[] { "Id" }, "UQ__Inventor__3213E83EB1F51875")
                        .IsUnique();

                    b.ToTable("Inventory", (string)null);
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CarId")
                        .HasColumnType("int")
                        .HasColumnName("car_id");

                    b.Property<int>("ClientId")
                        .HasColumnType("int")
                        .HasColumnName("client_id");

                    b.Property<DateTime>("CompletedAt")
                        .HasColumnType("datetime")
                        .HasColumnName("completed_at");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime")
                        .HasColumnName("created_at");

                    b.HasKey("Id")
                        .HasName("PK__Orders__3213E83F6F81E7F8");

                    b.HasIndex("CarId");

                    b.HasIndex("ClientId");

                    b.HasIndex(new[] { "Id" }, "UQ__Orders__3213E83ECFC3255A")
                        .IsUnique();

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.OrderAssignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int")
                        .HasColumnName("employee_id");

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("order_id");

                    b.HasKey("Id")
                        .HasName("PK__Order_as__3213E83F49F0FD4F");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("OrderId");

                    b.HasIndex(new[] { "Id" }, "UQ__Order_as__3213E83E85CB8FA2")
                        .IsUnique();

                    b.ToTable("Order_assignments", (string)null);
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.OrderPart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("order_id");

                    b.Property<int>("PartId")
                        .HasColumnType("int")
                        .HasColumnName("part_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.HasKey("Id")
                        .HasName("PK__Order_pa__3213E83F78E3B22F");

                    b.HasIndex("OrderId");

                    b.HasIndex("PartId");

                    b.HasIndex(new[] { "Id" }, "UQ__Order_pa__3213E83E5FF29685")
                        .IsUnique();

                    b.ToTable("Order_parts", (string)null);
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.OrderService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("order_id");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int")
                        .HasColumnName("service_id");

                    b.HasKey("Id")
                        .HasName("PK__Order_se__3213E83F6BACFC8E");

                    b.HasIndex("OrderId");

                    b.HasIndex("ServiceId");

                    b.HasIndex(new[] { "Id" }, "UQ__Order_se__3213E83E3286ED1D")
                        .IsUnique();

                    b.ToTable("Order_services", (string)null);
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Part", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 0)")
                        .HasColumnName("price");

                    b.HasKey("Id")
                        .HasName("PK__Parts__3213E83F93B0D7B3");

                    b.HasIndex(new[] { "Id" }, "UQ__Parts__3213E83E554EC61E")
                        .IsUnique();

                    b.ToTable("Parts");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 0)")
                        .HasColumnName("price");

                    b.HasKey("Id")
                        .HasName("PK__Services__3213E83FE45EA95B");

                    b.HasIndex(new[] { "Id" }, "UQ__Services__3213E83E04F93EE2")
                        .IsUnique();

                    b.ToTable("Services");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.ServicePart", b =>
                {
                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<int>("PartId")
                        .HasColumnType("int");

                    b.HasKey("ServiceId", "PartId");

                    b.HasIndex("PartId");

                    b.ToTable("ServiceParts");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Car", b =>
                {
                    b.HasOne("CarserviceConsoleApp.Models.Client", "Client")
                        .WithMany("Cars")
                        .HasForeignKey("ClientId")
                        .IsRequired()
                        .HasConstraintName("Cars_fk1");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Inventory", b =>
                {
                    b.HasOne("CarserviceConsoleApp.Models.Part", "Part")
                        .WithMany("Inventories")
                        .HasForeignKey("PartId")
                        .IsRequired()
                        .HasConstraintName("Inventory_fk1");

                    b.Navigation("Part");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Order", b =>
                {
                    b.HasOne("CarserviceConsoleApp.Models.Car", "Car")
                        .WithMany("Orders")
                        .HasForeignKey("CarId")
                        .IsRequired()
                        .HasConstraintName("Orders_fk1");

                    b.HasOne("CarserviceConsoleApp.Models.Client", "Client")
                        .WithMany("Orders")
                        .HasForeignKey("ClientId")
                        .IsRequired()
                        .HasConstraintName("Orders_fk2");

                    b.Navigation("Car");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.OrderAssignment", b =>
                {
                    b.HasOne("CarserviceConsoleApp.Models.Employee", "Employee")
                        .WithMany("OrderAssignments")
                        .HasForeignKey("EmployeeId")
                        .IsRequired()
                        .HasConstraintName("Order_assignments_fk2");

                    b.HasOne("CarserviceConsoleApp.Models.Order", "Order")
                        .WithMany("OrderAssignments")
                        .HasForeignKey("OrderId")
                        .IsRequired()
                        .HasConstraintName("Order_assignments_fk1");

                    b.Navigation("Employee");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.OrderPart", b =>
                {
                    b.HasOne("CarserviceConsoleApp.Models.Order", "Order")
                        .WithMany("OrderParts")
                        .HasForeignKey("OrderId")
                        .IsRequired()
                        .HasConstraintName("Order_parts_fk1");

                    b.HasOne("CarserviceConsoleApp.Models.Part", "Part")
                        .WithMany("OrderParts")
                        .HasForeignKey("PartId")
                        .IsRequired()
                        .HasConstraintName("Order_parts_fk2");

                    b.Navigation("Order");

                    b.Navigation("Part");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.OrderService", b =>
                {
                    b.HasOne("CarserviceConsoleApp.Models.Order", "Order")
                        .WithMany("OrderServices")
                        .HasForeignKey("OrderId")
                        .IsRequired()
                        .HasConstraintName("Order_services_fk1");

                    b.HasOne("CarserviceConsoleApp.Models.Service", "Service")
                        .WithMany("OrderServices")
                        .HasForeignKey("ServiceId")
                        .IsRequired()
                        .HasConstraintName("Order_services_fk2");

                    b.Navigation("Order");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.ServicePart", b =>
                {
                    b.HasOne("CarserviceConsoleApp.Models.Part", "Part")
                        .WithMany("ServiceParts")
                        .HasForeignKey("PartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarserviceConsoleApp.Models.Service", "Service")
                        .WithMany("ServiceParts")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Part");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Car", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Client", b =>
                {
                    b.Navigation("Cars");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Employee", b =>
                {
                    b.Navigation("OrderAssignments");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Order", b =>
                {
                    b.Navigation("OrderAssignments");

                    b.Navigation("OrderParts");

                    b.Navigation("OrderServices");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Part", b =>
                {
                    b.Navigation("Inventories");

                    b.Navigation("OrderParts");

                    b.Navigation("ServiceParts");
                });

            modelBuilder.Entity("CarserviceConsoleApp.Models.Service", b =>
                {
                    b.Navigation("OrderServices");

                    b.Navigation("ServiceParts");
                });
#pragma warning restore 612, 618
        }
    }
}
