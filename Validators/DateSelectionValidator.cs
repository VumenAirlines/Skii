using FluentValidation;
using Skii.Constants;
using Skii.Models;

namespace Skii.Validators;

public class DateSelectionValidator: AbstractValidator<DateSelection>
{
    public DateSelectionValidator()
    {
        RuleFor(x => x.Choice)
            .NotNull()
            . WithMessage("Must have a valid choice");
        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(DateOnly.Parse(AnswerConstants.StartDate))
            .LessThanOrEqualTo(DateOnly.Parse(AnswerConstants.EndDate))
            .WithMessage($"Date must be between {AnswerConstants.StartDate} and {AnswerConstants.EndDate}");
    }
}