using FluentValidation;
using Skii.Constants;
using Skii.DTOs;
using Skii.Models;

namespace Skii.Validators;

public class AnswerValidator : AbstractValidator<Answer>
{
    public AnswerValidator()
    {
        RuleFor(x => x.MinLength)
            .GreaterThanOrEqualTo(AnswerConstants.MinLength)
            .WithMessage($"Maximum length must be larger than {AnswerConstants.MinLength}");
        
        RuleFor(x => x.MaxLength)
            .LessThanOrEqualTo(AnswerConstants.MaxLength)
            .WithMessage($"Maximum length must be smaller than {AnswerConstants.MaxLength}");
        
        RuleFor(x => x.SkiiOnFirstDay)
            .NotNull()
            .WithMessage("Must have a valid selection");
        
        RuleFor(x => x.PreferWeekends)
            .NotNull()
            .WithMessage("Must have a valid selection");
        
        RuleFor(x => x.AvailableDates)
            .NotEmpty()
            .WithMessage("Must have a valid selection");
        
        RuleForEach(x => x.AvailableDates)
            .SetValidator(new DateSelectionValidator());

    }
}