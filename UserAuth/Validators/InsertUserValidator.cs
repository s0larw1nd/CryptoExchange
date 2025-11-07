using FluentValidation;
using UserAuth.DTO.V1.Requests;

namespace UserAuth.Validators;

public class InsertUserValidator: AbstractValidator<InsertUserRequest>
{
    public InsertUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(20);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(40);
    }
}