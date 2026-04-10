using FluentValidation;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Validators;

public class DisplayStatusUpdateRequestValidator:AbstractValidator<DisplayStatusUpdateRequestDTO>
{
    
    public DisplayStatusUpdateRequestValidator()
    {

        // List of allowed statuses
        var allowedDisplayStatuses = new List<string>
        {
            nameof(UserStatus.Online),
            nameof(UserStatus.Offline),
            nameof(UserStatus.InCall),
            nameof(UserStatus.InRoom)
        };
        
        // Validation rules
        RuleFor(x => x.DisplayStatus)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Status is required!")
            .IsEnumName(typeof(UserStatus)).WithMessage("Status is not valid!")
            .Must(status => allowedDisplayStatuses.Contains(status)).WithMessage($"Status must be one of:{string.Join(", ", allowedDisplayStatuses)}.");


    }
    
}