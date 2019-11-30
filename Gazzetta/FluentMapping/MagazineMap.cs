using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Gazzetta.Models;

namespace Gazzetta.FluentMapping
{
    public class MagazineMap : EntityTypeConfiguration<Magazine>
    {
        public MagazineMap()
        {
            //Table Key
            HasKey(x => x.Id);

            //Fields
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Publication.Category).IsRequired().HasMaxLength(50);
            Property(x => x.Publication.Description).IsRequired().HasMaxLength(280);
            Property(x => x.Publication.Language).IsRequired().HasMaxLength(20);
            Property(x => x.Publication.Name).IsRequired().HasMaxLength(50);
            Property(x => x.Publication.Price).IsRequired();
            Property(x => x.Publication.Publisher).IsRequired().HasMaxLength(50);
            Property(x => x.Publication.Content).IsRequired();
            Property(x => x.IssueNumber).IsRequired().HasColumnType("date");
           
          
            //Relationship
//            HasMany(x => x.Customers).WithMany(c => c.Magazines)
//                .Map(x => x.ToTable("CustomersToMagazines")
//                    .MapLeftKey("CustomerId")
//                    .MapRightKey("MagazineId")
//
//                );

        }
    }
}