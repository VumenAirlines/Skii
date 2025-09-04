using FluentValidation;
using Skii.Constants;
using Skii.DTOs;

namespace Skii.Validators;

public class UpdateAnswerValidator: AbstractValidator<UpdateAnswerParsedDto>
{
    public UpdateAnswerValidator()
    {
        RuleFor(x => x.MinLength)
            .GreaterThanOrEqualTo(AnswerConstants.MinLength)
            .When(x => x.MinLength.HasValue)
            .WithMessage($"Maximum length must be larger than {AnswerConstants.MinLength}");
        
        RuleFor(x => x.MaxLength)
            .LessThanOrEqualTo(AnswerConstants.MaxLength)
            .When(x => x.MaxLength.HasValue)
            .WithMessage($"Maximum length must be smaller than {AnswerConstants.MaxLength}");
        
        RuleForEach(x => x.AvailableDates)
            .SetValidator(new DateSelectionValidator())
            .When(x => x.AvailableDates is not null);;
    }
}