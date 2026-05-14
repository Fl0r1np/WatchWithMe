using FluentValidation;
using WatchWithMeAPI.DTOs.Room.Requests;

namespace WatchWithMeAPI.Validators;

public class JoinRoomWithShareCodeRequestValidator : AbstractValidator<JoinRoomWithShareCodeRequestDTO>
{
    
    public JoinRoomWithShareCodeRequestValidator()
    {
        RuleFor(x => x.ShareCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Share code is required!")
            .Matches(@"^[A-Z0-9]{4}-[A-Z0-9]{4}$").WithMessage("Share code is not valid!");
    }
    
}