using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Mvc;
using Gazzetta.Models;

namespace Gazzetta.FluentValidation
{
    public class PublicationValidation : AbstractValidator<Publication>
    {
        public PublicationValidation()
        {
            //RuleFor(p => p.Category).NotEmpty().Length(0, 50);
            RuleFor(p => p.Name).NotEmpty().MaximumLength(50);
            RuleFor(p => p.Description).NotEmpty().MaximumLength(280);
            RuleFor(p => p.Language).NotEmpty().Length(0, 20);
            RuleFor(p => p.Price).NotEmpty().GreaterThanOrEqualTo(0);
            RuleFor(p => p.Publisher).NotEmpty().Length(0,50);
            RuleFor(p => p.Tags).Length(0, 20);
            //RuleFor(p => p.Content).NotEmpty().NotNull();

            // RuleFor(p => p.Content).NotNull();


        }
    }
}