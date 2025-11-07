using FluentValidation;
using UserAuth.DTO.V1.Requests;

namespace UserAuth.Validators;

public class QueryUserValidator: AbstractValidator<AuthUserRequest>
{
    public QueryUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}