using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using Gazzetta.Models;

namespace Gazzetta.FluentValidation
{
    public class MagazineValidation : AbstractValidator<Magazine>
    {
        public MagazineValidation()
        {
            RuleFor(c => c.Publication).SetValidator(new PublicationValidation());
            RuleFor(m => m.IssueNumber).NotEmpty();
        }
    }
}