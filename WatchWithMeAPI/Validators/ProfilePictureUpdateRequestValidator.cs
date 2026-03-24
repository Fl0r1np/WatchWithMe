using WatchWithMeAPI.DTO;

namespace WatchWithMeAPI.Validators;
using FluentValidation;

public class ProfilePictureUpdateRequestValidator : AbstractValidator<ProfilePictureUpdateRequestDTO>
{
    
    public ProfilePictureUpdateRequestValidator()
    {
        RuleFor(x => x.ProfilePictureFilename)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Avatar filename is required!")
            .Matches(@"^avatar-(default|[1-9]\d*)\.png$")
            .WithMessage("Avatar filename must be in the format 'avatar-default.png' or 'avatar-[number >= 1].png'!");
    }
    
}