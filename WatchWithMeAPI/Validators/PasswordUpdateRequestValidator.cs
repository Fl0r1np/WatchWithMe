using FluentValidation;
using WatchWithMeAPI.DTO;

namespace WatchWithMeAPI.Validators;

public class PasswordUpdateRequestValidator : AbstractValidator<PasswordUpdateRequestDTO>
{

    public PasswordUpdateRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Current password is required!")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$").WithMessage(
                "Current Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number and one alphanumeric character!");
        
        RuleFor(x => x.NewPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("New password is required!")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$").WithMessage("New Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number and one alphanumeric character!")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password cannot be the same as the current one!");
    }
    
}