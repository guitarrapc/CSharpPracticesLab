using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationEF.Data
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TestType> TestType { get; set; }
        public DbSet<TestTable> TestTable { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // custom type conversion: https://docs.microsoft.com/ja-jp/ef/core/modeling/value-conversions
            // DateTimeOffset(clr) should map to DateTime(mysql).
            // offset is always 0.(UTC)
            var datetimeOffsetConverter = new ValueConverter<DateTimeOffset, DateTime>(datetimeoffset => datetimeoffset.DateTime, value => new DateTimeOffset(value, TimeSpan.Zero));
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.DatetimeOffset2)
                .HasConversion(datetimeOffsetConverter);

            modelBuilder
                .Entity<TestType>()
                .Property(e => e.String3)
                .HasColumnType("VARCHAR(255)");
            // do not
            //.HasColumnType("VARCHAR")
            //.HasMaxLength(255);

            // InvalidCastException: Unable to cast object of type 'System.SByte' to type 'System.Int16'.
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Bool)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Bool2)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Bool3)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Sbyte)
                .HasColumnType("SMALLINT(6)");
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Ushort)
                .HasColumnType("INT(11)");
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Uint)
                .HasColumnType("BIGINT(20)");
            modelBuilder
                .Entity<TestType>()
                .Property(e => e.Char)
                .HasColumnType("INT(11)");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
    }

    public class TestType
    {
        // INT(11)
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }
        // SMALLINT(6)
        public sbyte Sbyte { get; set; }
        // TINYINT(4)
        public byte Byte { get; set; }
        // VARBINARY(3000)
        [MaxLength(3000)]
        public byte[] ByteArray { get; set; }
        // SMALLINT(6)
        public short Short { get; set; }
        // INT(11) UNSIGNED
        public ushort Ushort { get; set; }
        // INT(11)
        public int Int { get; set; }
        // BIGINT(20)
        public uint Uint { get; set; }
        // BIGINT(20)
        public long Long { get; set; }
        // BIGINT(20)
        // cannot map ulong to premitive, specify type.
        [Column(TypeName = "BigInt")]
        public ulong Ulong { get; set; }
        // FLOAT
        public float Float { get; set; }
        // DOUBLE
        public double Double { get; set; }
        // DECIMAL
        public decimal Decimal { get; set; }
        // SMALLINT(6)
        public bool Bool { get; set; }
        // TINYINT(1)
        [Column(TypeName = "TinyInt(1)")]
        public bool Bool2 { get; set; }
        // BIT(1)
        [Column(TypeName = "BIT(1)")]
        public bool Bool3 { get; set; }
        // INT(11)
        public char Char { get; set; }
        // TEXT
        public string String { get; set; }
        // VARCHAR(255)
        [Column(TypeName = "VARCHAR(255)")]
        public string String2 { get; set; }
        // VARCHAR
        public string String3 { get; set; }
        // DATETIME
        public DateTime Datetime { get; set; }
        // TIMESTAMP
        public DateTimeOffset DatetimeOffset { get; set; }
        // DATETIME
        public DateTimeOffset DatetimeOffset2 { get; set; }
    }

    public class TestTable
    {
        [Key]
        public int Number { get; set; }
        [Column(TypeName = "VARCHAR(50)")]
        public string Name { get; set; }
        [Column(TypeName = "VARCHAR(255)")]
        public string Url { get; set; }
    }
}
