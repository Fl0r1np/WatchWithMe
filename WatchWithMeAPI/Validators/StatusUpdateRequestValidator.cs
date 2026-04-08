using FluentValidation;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Validators;

public class StatusUpdateRequestValidator : AbstractValidator<StatusUpdateRequestDTO>
{
    public StatusUpdateRequestValidator()
    {
        RuleFor(x => x.Status)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Status is required!")
            .IsEnumName(typeof(UserStatus)).WithMessage("Status is not valid!");

    }
}