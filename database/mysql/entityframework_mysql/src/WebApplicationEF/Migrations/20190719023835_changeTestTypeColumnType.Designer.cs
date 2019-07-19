﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplicationEF.Data;

namespace WebApplicationEF.Migrations
{
    [DbContext(typeof(BloggingContext))]
    [Migration("20190719023835_changeTestTypeColumnType")]
    partial class changeTestTypeColumnType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("WebApplicationEF.Data.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Url");

                    b.HasKey("BlogId");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("WebApplicationEF.Data.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BlogId");

                    b.Property<string>("Content");

                    b.Property<string>("Title");

                    b.HasKey("PostId");

                    b.HasIndex("BlogId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("WebApplicationEF.Data.TestTable", b =>
                {
                    b.Property<int>("Number")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(50)");

                    b.Property<string>("Url")
                        .HasColumnType("VARCHAR(255)");

                    b.HasKey("Number");

                    b.ToTable("TestTable");
                });

            modelBuilder.Entity("WebApplicationEF.Data.TestType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Bool");

                    b.Property<byte>("Byte");

                    b.Property<DateTime>("Datetime");

                    b.Property<DateTimeOffset>("DatetimeOffset");

                    b.Property<double>("Double");

                    b.Property<float>("Float");

                    b.Property<int>("Int");

                    b.Property<long>("Long");

                    b.Property<sbyte>("Sbyte");

                    b.Property<short>("Short");

                    b.Property<string>("String");

                    b.Property<string>("String2")
                        .HasColumnType("VARCHAR(50)");

                    b.Property<uint>("Uint");

                    b.Property<long>("Ulong")
                        .HasColumnType("BigInt");

                    b.Property<ushort>("Ushort");

                    b.HasKey("Id");

                    b.ToTable("TestType");
                });

            modelBuilder.Entity("WebApplicationEF.Data.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("Created");

                    b.Property<string>("Name");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApplicationEF.Data.Post", b =>
                {
                    b.HasOne("WebApplicationEF.Data.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
