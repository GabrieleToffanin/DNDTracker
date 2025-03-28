﻿// <auto-generated />
using System;
using DNDTracker.Outbounx.PostgresDb.Database.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DNDTracker.Outbounx.PostgresDb.Migrations
{
    [DbContext(typeof(DNDTrackerPostgresDbContext))]
    [Migration("20250301222851_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DNDTracker.Domain.Campaigns.Campaign", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CampaignDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CampaignImage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CampaignName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Campaign");
                });

            modelBuilder.Entity("DNDTracker.Domain.Heroes.Hero", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("Alignment")
                        .HasColumnType("integer");

                    b.Property<string>("CampaignId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Class")
                        .HasColumnType("integer");

                    b.Property<int>("Experience")
                        .HasColumnType("integer");

                    b.Property<int>("HitDice")
                        .HasColumnType("integer");

                    b.Property<int>("HitPoints")
                        .HasColumnType("integer");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Race")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.ToTable("Hero");
                });

            modelBuilder.Entity("DNDTracker.SharedKernel.Primitives.DomainEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CampaignId")
                        .HasColumnType("text");

                    b.Property<string>("HeroId")
                        .HasColumnType("text");

                    b.Property<DateTime>("OccuredOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CampaignId");

                    b.HasIndex("HeroId");

                    b.ToTable("DomainEvent");
                });

            modelBuilder.Entity("DNDTracker.Vocabulary.ValueObjects.Spell", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CastingTime")
                        .HasColumnType("text");

                    b.Property<string>("Components")
                        .HasColumnType("text");

                    b.Property<string>("Concentration")
                        .HasColumnType("text");

                    b.Property<string>("Damage")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Duration")
                        .HasColumnType("text");

                    b.Property<string>("HeroId")
                        .HasColumnType("text");

                    b.Property<bool>("IsRitual")
                        .HasColumnType("boolean");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Material")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Range")
                        .HasColumnType("text");

                    b.Property<string>("Save")
                        .HasColumnType("text");

                    b.Property<string>("School")
                        .HasColumnType("text");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<string>("Time")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("HeroId");

                    b.ToTable("Spell");
                });

            modelBuilder.Entity("DNDTracker.Domain.Heroes.Hero", b =>
                {
                    b.HasOne("DNDTracker.Domain.Campaigns.Campaign", "Campaign")
                        .WithMany("Heroes")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("DNDTracker.SharedKernel.Primitives.DomainEvent", b =>
                {
                    b.HasOne("DNDTracker.Domain.Campaigns.Campaign", null)
                        .WithMany("DomainEvents")
                        .HasForeignKey("CampaignId");

                    b.HasOne("DNDTracker.Domain.Heroes.Hero", null)
                        .WithMany("DomainEvents")
                        .HasForeignKey("HeroId");
                });

            modelBuilder.Entity("DNDTracker.Vocabulary.ValueObjects.Spell", b =>
                {
                    b.HasOne("DNDTracker.Domain.Heroes.Hero", null)
                        .WithMany("Spells")
                        .HasForeignKey("HeroId");
                });

            modelBuilder.Entity("DNDTracker.Domain.Campaigns.Campaign", b =>
                {
                    b.Navigation("DomainEvents");

                    b.Navigation("Heroes");
                });

            modelBuilder.Entity("DNDTracker.Domain.Heroes.Hero", b =>
                {
                    b.Navigation("DomainEvents");

                    b.Navigation("Spells");
                });
#pragma warning restore 612, 618
        }
    }
}
