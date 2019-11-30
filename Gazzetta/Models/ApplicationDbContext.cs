using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Gazzetta.FluentMapping;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Gazzetta.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers  { get; set; }

        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<UserMagazine> UserMagazines { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //FluentAPI mapping goes here
            modelBuilder.ComplexType<Publication>();

            modelBuilder.Configurations.Add(new BookMap());
            modelBuilder.Configurations.Add(new MagazineMap());
//            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new UserMagazineMap());
            modelBuilder.Configurations.Add(new UserBookmap());



 
        }


        

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}