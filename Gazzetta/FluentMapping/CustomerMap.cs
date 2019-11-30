using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Gazzetta.Models;

namespace Gazzetta.FluentMapping
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            //Table Key
            HasKey(x => x.Id);


            //Property
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Email).IsRequired().HasMaxLength(30);
            Property(x => x.Name).IsRequired().HasMaxLength(20);
            //Property(x => x.PhoneNumber).IsRequired();
            

        }
    }
}