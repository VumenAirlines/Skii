using FluentValidation;
using Skii.Constants;
using Skii.DTOs;
using Skii.Enums;
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
            .Must(HaveValidDateRange)
            .WithMessage("Must have a valid selection");
        
        RuleForEach(x => x.AvailableDates)
            .SetValidator(new DateSelectionValidator());
        

    }
    private bool HaveValidDateRange(List<DateSelection> availability)
    {
        var startDate = DateOnly.Parse( AnswerConstants.StartDate);
        var endDate = DateOnly.Parse( AnswerConstants.EndDate);
        
        var dateSet = availability.ToDictionary(a => a.Date, a => a.Choice);

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (!dateSet.TryGetValue(date, out var choice))
                return false;

            if (choice == null || !Enum.IsDefined(typeof(DateChoices), choice))
                return false;
        }
        return true;
    }
}