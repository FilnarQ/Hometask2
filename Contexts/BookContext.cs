using Hometask2.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Hometask2.Contexts
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; } = null;
        public DbSet<Rating> Ratings { get; set; } = null;
        public DbSet<Review> Reviews { get; set; } = null;
    }
}
