﻿// <auto-generated />
using System;
using DominosDriverHustleComp.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DominosDriverHustleComp.Server.Migrations
{
    [DbContext(typeof(HustleCompContext))]
    partial class HustleCompContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("DominosDriverHustleComp.Server.Models.Delivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DriverId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("HustleIn")
                        .HasColumnType("REAL");

                    b.Property<float>("HustleOut")
                        .HasColumnType("REAL");

                    b.Property<bool>("WasTracked")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.ToTable("Deliveries");
                });

            modelBuilder.Entity("DominosDriverHustleComp.Server.Models.DeliverySummary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("AvgHustleCombined")
                        .HasColumnType("REAL");

                    b.Property<float>("AvgHustleIn")
                        .HasColumnType("REAL");

                    b.Property<float>("AvgHustleOut")
                        .HasColumnType("REAL");

                    b.Property<int>("DriverId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumDels")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumOverspeeds")
                        .HasColumnType("INTEGER");

                    b.Property<float>("TrackedPercentage")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("WeekEnding")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.HasIndex("WeekEnding");

                    b.ToTable("DeliverySummaries");
                });

            modelBuilder.Entity("DominosDriverHustleComp.Server.Models.Driver", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsPermanentlyDisqualified")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Drivers");
                });

            modelBuilder.Entity("DominosDriverHustleComp.Server.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("HustleBenchmarkSeconds")
                        .HasColumnType("REAL");

                    b.Property<int>("MaxOverspeeds")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinDels")
                        .HasColumnType("INTEGER");

                    b.Property<float>("MinTrackedPercentage")
                        .HasColumnType("REAL");

                    b.Property<float>("OutlierSeconds")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("DominosDriverHustleComp.Server.Models.WeeklySummary", b =>
                {
                    b.Property<DateTime>("WeekEnding")
                        .HasColumnType("TEXT");

                    b.Property<float>("AvgHustleCombined")
                        .HasColumnType("REAL");

                    b.Property<float>("AvgHustleIn")
                        .HasColumnType("REAL");

                    b.Property<float>("AvgHustleOut")
                        .HasColumnType("REAL");

                    b.HasKey("WeekEnding");

                    b.ToTable("WeeklySummaries");
                });

            modelBuilder.Entity("DominosDriverHustleComp.Server.Models.Delivery", b =>
                {
                    b.HasOne("DominosDriverHustleComp.Server.Models.Driver", "Driver")
                        .WithMany("Deliveries")
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Driver");
                });

            modelBuilder.Entity("DominosDriverHustleComp.Server.Models.DeliverySummary", b =>
                {
                    b.HasOne("DominosDriverHustleComp.Server.Models.Driver", "Driver")
                        .WithMany("DeliverySummaries")
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DominosDriverHustleComp.Server.Models.WeeklySummary", null)
                        .WithMany("DeliverySummaries")
                        .HasForeignKey("WeekEnding")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Driver");
                });

            modelBuilder.Entity("DominosDriverHustleComp.Server.Models.Driver", b =>
                {
                    b.Navigation("Deliveries");

                    b.Navigation("DeliverySummaries");
                });

            modelBuilder.Entity("DominosDriverHustleComp.Server.Models.WeeklySummary", b =>
                {
                    b.Navigation("DeliverySummaries");
                });
#pragma warning restore 612, 618
        }
    }
}
