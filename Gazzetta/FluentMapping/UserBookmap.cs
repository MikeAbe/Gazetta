using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Gazzetta.Models;

namespace Gazzetta.FluentMapping
{
    public class UserBookmap: EntityTypeConfiguration<UserBook>
    {
        public UserBookmap()
        {
            //Keys
//            HasKey(q => new
//            {
//                q.AppliationUserId,
//                q.BookId
//
//            });
            HasKey(t => t.UserBookId);
            Property(t => t.UserBookId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //Relationships
            HasRequired(t => t.ApplicationUser)
                .WithMany(t => t.UserBooks)
                .HasForeignKey(t => t.AppliationUserId);

            HasRequired(t => t.Book)
                .WithMany(t => t.UserBooks)
                .HasForeignKey(t => t.BookId);

        }
    }
}