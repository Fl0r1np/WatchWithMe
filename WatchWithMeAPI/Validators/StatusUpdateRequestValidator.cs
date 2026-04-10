using FluentValidation;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Validators;

public class StatusUpdateRequestValidator : AbstractValidator<StatusUpdateRequestDTO>
{
    public StatusUpdateRequestValidator()
    {
        // List of allowed statuses
        var allowedStatuses = new List<string> {
            nameof(UserStatus.Public),
            nameof(UserStatus.Private),
            nameof(UserStatus.DoNotDisturb)
        };
        
        // Validation rules
        RuleFor(x => x.Status)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Status is required!")
            .IsEnumName(typeof(UserStatus)).WithMessage("Status is not valid!")
            .Must(status => allowedStatuses.Contains(status)).WithMessage($"Status must be one of:{string.Join(", ", allowedStatuses)}.");
    }
}