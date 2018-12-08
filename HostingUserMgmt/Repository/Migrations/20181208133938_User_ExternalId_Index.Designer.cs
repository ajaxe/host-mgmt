﻿// <auto-generated />
using System;
using HostingUserMgmt.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HostingUserMgmt.Repository.Migrations
{
    [DbContext(typeof(HostingManagementDbContext))]
    [Migration("20181208133938_User_ExternalId_Index")]
    partial class User_ExternalId_Index
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("HostingUserMgmt.Domain.EntityModels.ApiCredential", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAtUtc");

                    b.Property<DateTime?>("ModifiedAtUtc");

                    b.Property<short>("Type");

                    b.Property<int>("UserId");

                    b.Property<string>("UserSecret");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("ApiCredentials");
                });

            modelBuilder.Entity("HostingUserMgmt.Domain.EntityModels.Entitlement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAtUtc");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("ModifiedAtUtc");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Entitlements");
                });

            modelBuilder.Entity("HostingUserMgmt.Domain.EntityModels.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAtUtc");

                    b.Property<DateTime?>("ModifiedAtUtc");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("HostingUserMgmt.Domain.EntityModels.RoleEntitlement", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<int>("EntitlementId");

                    b.HasKey("RoleId", "EntitlementId");

                    b.HasIndex("EntitlementId");

                    b.ToTable("RoleEntitlement");
                });

            modelBuilder.Entity("HostingUserMgmt.Domain.EntityModels.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAtUtc");

                    b.Property<string>("EmailAddress");

                    b.Property<string>("ExternalId");

                    b.Property<DateTime?>("ModifiedAtUtc");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HostingUserMgmt.Domain.EntityModels.ApiCredential", b =>
                {
                    b.HasOne("HostingUserMgmt.Domain.EntityModels.User", "User")
                        .WithMany("Credentials")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HostingUserMgmt.Domain.EntityModels.RoleEntitlement", b =>
                {
                    b.HasOne("HostingUserMgmt.Domain.EntityModels.Entitlement", "Entitlement")
                        .WithMany("Roles")
                        .HasForeignKey("EntitlementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HostingUserMgmt.Domain.EntityModels.Role", "Role")
                        .WithMany("Entitlements")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
