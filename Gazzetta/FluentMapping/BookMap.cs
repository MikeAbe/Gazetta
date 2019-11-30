using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;
using Gazzetta.Models;

namespace Gazzetta.FluentMapping
{
    public class BookMap : EntityTypeConfiguration<Book>
    {
        public BookMap()
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
            Property(x => x.Author).IsRequired().HasMaxLength(50);
            Property(x => x.Blurb).HasMaxLength(280).IsOptional();
            Property(x => x.Publication.Content).IsRequired();


            //Relationship
           // HasRequired(x => x.Owner).WithMany(t => t.UserBooks).Map(m => m.MapKey("OwnerId"));

//            HasMany(x => x.UserBooks).WithMany(c => c.UserBooks)
//                .Map(x => x.ToTable("CustomersToBooks")
//                    .MapLeftKey("CustomerId")
//                    .MapRightKey("BookId")
//
//                );
            

        }

    }

}






/*
modelBuilder.Entity<Publication>()
    .HasKey(x => x.Id);

modelBuilder.Entity<Publication>()
    .Property(x => x.Id)
    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

modelBuilder.Entity<Publication>()
    .Property(x => x.Name)
    .IsRequired()
    .HasMaxLength(30);

modelBuilder.Entity<Publication>()
    .Property(x => x.Price)
    .IsRequired();
    

modelBuilder.Entity<Publication>()
    .Property(x => x.Publisher)
    .IsRequired()
    .HasMaxLength(50);

modelBuilder.Entity<Publication>()
    .Property(x => x.Category)
    .IsRequired()
    .HasMaxLength(30);

modelBuilder.Entity<Publication>()
    .Property(x => x.Description)
    .IsRequired()
    .HasMaxLength(280);

modelBuilder.Entity<Publication>()
    .Property(x => x.Language)
    .IsRequired()
    .HasMaxLength(20);

}
}
}*/