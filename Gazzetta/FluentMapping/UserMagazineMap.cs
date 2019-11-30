using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Gazzetta.Models;

namespace Gazzetta.FluentMapping
{
    public class UserMagazineMap:EntityTypeConfiguration<UserMagazine>
    {
        public UserMagazineMap()
        {
            //Key
//            HasKey(q => new
//            {
//                q.ApplicationUserId,
//                q.MagazineId
//
//            });
            HasKey(t => t.UserMagazineId);
            Property(t => t.UserMagazineId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            //Relationship
            HasRequired(t => t.ApplicationUser)
                .WithMany(t =>t.UserMagazines)
                .HasForeignKey(t => t.ApplicationUserId);

            HasRequired(t => t.Magazine)
                .WithMany(t => t.UserMagazines)
                .HasForeignKey(t => t.MagazineId);
        }
    }
}