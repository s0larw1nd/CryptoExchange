using FluentValidation;
using UserAuth.DTO.V1.Requests;

namespace UserAuth.Validators;

public class ValidateTokenValidator: AbstractValidator<ValidateTokenRequest>
{
    public ValidateTokenValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}