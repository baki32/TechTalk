using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWeb.Models
{
    public class FirstModel: DbContext
    {
        public FirstModel(DbContextOptions<FirstModel> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .Property(b => b.InsertedDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Event>()
                .Property(b => b.LastUpdatedDate)
                .HasDefaultValueSql("getdate()");
        }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Event> Events { get; set; }

    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class Event
    {
        [Key]
        public int EventId { get; set; }
        
        public DateTime InsertedDate { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public string Data { get; set; }
    }
}

