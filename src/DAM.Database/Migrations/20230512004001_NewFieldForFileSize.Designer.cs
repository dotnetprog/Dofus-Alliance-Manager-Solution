﻿// <auto-generated />
using System;
using DAM.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAM.Database.Migrations
{
    [DbContext(typeof(AllianceContext))]
    [Migration("20230512004001_NewFieldForFileSize")]
    partial class NewFieldForFileSize
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DAM.Domain.Entities.Alliance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Alias")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("AllianceConfigurationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DiscordGuildId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegisteredOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AllianceConfigurationId");

                    b.ToTable("Alliances");
                });

            modelBuilder.Entity("DAM.Domain.Entities.AllianceConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("AtkScreen_DiscordChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("DefScreen_DiscordChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.ToTable("AllianceConfigurations");
                });

            modelBuilder.Entity("DAM.Domain.Entities.AllianceMember", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Alias")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("AllianceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DiscordId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AllianceId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("DAM.Domain.Entities.AllianceMember_ScreenPost", b =>
                {
                    b.Property<Guid>("ScreenPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AllianceMemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("CharacterCount")
                        .HasColumnType("int");

                    b.HasKey("ScreenPostId", "AllianceMemberId");

                    b.HasIndex("AllianceMemberId");

                    b.ToTable("Member_ScreenPosts");
                });

            modelBuilder.Entity("DAM.Domain.Entities.ScreenPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Base64Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CreatedByMemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EnemyCount")
                        .HasColumnType("int");

                    b.Property<int?>("Filesize")
                        .HasColumnType("int");

                    b.Property<bool?>("HasOtherWithSameSize")
                        .HasColumnType("bit");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Target")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByMemberId");

                    b.ToTable("ScreenPosts");
                });

            modelBuilder.Entity("DAM.Domain.Entities.ScreenValidation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OCRScreenText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProcessingState")
                        .HasColumnType("int");

                    b.Property<int?>("ResultState")
                        .HasColumnType("int");

                    b.Property<Guid>("ScreenPostId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ScreenPostId");

                    b.ToTable("ScreenValidations");
                });

            modelBuilder.Entity("DAM.Domain.Entities.Alliance", b =>
                {
                    b.HasOne("DAM.Domain.Entities.AllianceConfiguration", "AllianceConfiguration")
                        .WithMany()
                        .HasForeignKey("AllianceConfigurationId");

                    b.Navigation("AllianceConfiguration");
                });

            modelBuilder.Entity("DAM.Domain.Entities.AllianceMember", b =>
                {
                    b.HasOne("DAM.Domain.Entities.Alliance", "Alliance")
                        .WithMany("Members")
                        .HasForeignKey("AllianceId");

                    b.Navigation("Alliance");
                });

            modelBuilder.Entity("DAM.Domain.Entities.AllianceMember_ScreenPost", b =>
                {
                    b.HasOne("DAM.Domain.Entities.AllianceMember", "AllianceMember")
                        .WithMany("ScreenPosts")
                        .HasForeignKey("AllianceMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAM.Domain.Entities.ScreenPost", "ScreenPost")
                        .WithMany("Members")
                        .HasForeignKey("ScreenPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AllianceMember");

                    b.Navigation("ScreenPost");
                });

            modelBuilder.Entity("DAM.Domain.Entities.ScreenPost", b =>
                {
                    b.HasOne("DAM.Domain.Entities.AllianceMember", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedByMemberId");

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("DAM.Domain.Entities.ScreenValidation", b =>
                {
                    b.HasOne("DAM.Domain.Entities.ScreenPost", "ScreenPost")
                        .WithMany("ScreenValidations")
                        .HasForeignKey("ScreenPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScreenPost");
                });

            modelBuilder.Entity("DAM.Domain.Entities.Alliance", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("DAM.Domain.Entities.AllianceMember", b =>
                {
                    b.Navigation("ScreenPosts");
                });

            modelBuilder.Entity("DAM.Domain.Entities.ScreenPost", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("ScreenValidations");
                });
#pragma warning restore 612, 618
        }
    }
}
