﻿// <auto-generated />
using System;
using DevCenterGallery.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevCenterGallery.Web.Migrations
{
    [DbContext(typeof(DevCenterContext))]
    partial class DevCenterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7");

            modelBuilder.Entity("DevCenterGallary.Common.Models.Asset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AssetType")
                        .HasColumnType("TEXT");

                    b.Property<string>("PackageId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PackageId");

                    b.ToTable("Asset");
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.FileInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AssetId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileName")
                        .HasColumnType("TEXT");

                    b.Property<string>("SasUrl")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AssetId")
                        .IsUnique();

                    b.ToTable("FileInfo");
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.Package", b =>
                {
                    b.Property<string>("PackageId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Architecture")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PackageVersion")
                        .HasColumnType("TEXT");

                    b.Property<string>("SubmissionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PackageId");

                    b.HasIndex("SubmissionId");

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.Product", b =>
                {
                    b.Property<string>("BigId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LogoUri")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("BigId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.Submission", b =>
                {
                    b.Property<string>("SubmissionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProductBigId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PublishedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("ReleaseRank")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("SubmissionId");

                    b.HasIndex("ProductBigId");

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.TargetPlatform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("MinVersion")
                        .HasColumnType("TEXT");

                    b.Property<string>("PackageId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PlatformName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PackageId");

                    b.ToTable("TargetPlatform");
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.Asset", b =>
                {
                    b.HasOne("DevCenterGallary.Common.Models.Package", "Package")
                        .WithMany("Assets")
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.FileInfo", b =>
                {
                    b.HasOne("DevCenterGallary.Common.Models.Asset", "Asset")
                        .WithOne("FileInfo")
                        .HasForeignKey("DevCenterGallary.Common.Models.FileInfo", "AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.Package", b =>
                {
                    b.HasOne("DevCenterGallary.Common.Models.Submission", "Submission")
                        .WithMany("Packages")
                        .HasForeignKey("SubmissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.Submission", b =>
                {
                    b.HasOne("DevCenterGallary.Common.Models.Product", "Product")
                        .WithMany("Submissions")
                        .HasForeignKey("ProductBigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DevCenterGallary.Common.Models.TargetPlatform", b =>
                {
                    b.HasOne("DevCenterGallary.Common.Models.Package", "Package")
                        .WithMany("RuntimeTargetPlatforms")
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}