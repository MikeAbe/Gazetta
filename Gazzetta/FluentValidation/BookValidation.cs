using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Gazzetta.Models;

namespace Gazzetta.FluentValidation
{
    public class BookValidation : AbstractValidator<Book>
    {
        public BookValidation()
        {
            RuleFor(c => c.Publication).SetValidator(new PublicationValidation());
            RuleFor(s => s.Author).NotEmpty().Length(0, 50).WithMessage("Please enter author name");
            RuleFor(s => s.Blurb).Length(0, 280);
//            RuleFor(s => s.Publication.Publisher).NotEmpty().Length(0, 10);
//            RuleFor(s => s.Publication.Publisher).NotEmpty().Length(0, 10);


        }
    }
}