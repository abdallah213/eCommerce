using eCommerceApp.Application.DTOs.Identity;
using FluentValidation;

namespace eCommerceApp.Application.Validations.Authentication
{
    public class CreateUserValidator: AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FullName)
               .NotEmpty().WithMessage("FullName is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");


            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password Must Contain At Least 8 Characters Long")
                .Matches(@"[A-Z]").WithMessage("Password Must Contain At Least One Uppercase Letter.")
                .Matches(@"[a-z]").WithMessage("Password Must Contain At Least One Lowercase Letter.")
                .Matches(@"\d").WithMessage("Password Must Contain At Least One Number")
                .Matches(@"\w").WithMessage("Password Must Contain At Least One Spectial Character");

            RuleFor(x=>x.ConfirmPassword)
                .Equal(x=>x.Password).WithMessage("Passwords do not match");
        }
    }
}
