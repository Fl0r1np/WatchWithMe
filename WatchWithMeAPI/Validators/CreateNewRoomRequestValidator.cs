using System.Data;
using FluentValidation;
using WatchWithMeAPI.DTOs.Room.Requests;
using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Validators;

public class CreateNewRoomRequestValidator : AbstractValidator<CreateNewRoomRequestDTO>
{

    public CreateNewRoomRequestValidator()
    {
        
        // Validation rule for DisplayName
        RuleFor(x => x.DisplayName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Display name is required!")
            .MaximumLength(30).WithMessage("Display name must be less than 30 characters!")
            .MinimumLength(6).WithMessage("Display name must be more than 6 characters!")
            .Matches(@"^[a-zA-Z0-9 ]+$").WithMessage("Display name can only contain letters, numbers and spaces!");
        
        // Validation rule for NumberOfMaxParticipants
        RuleFor(x => x.NumberOfMaxParticipants)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(2).WithMessage("Number of max participants must be greater than 2!")
            .LessThanOrEqualTo(20).WithMessage("Number of max participants must be less than or equal to 20!");

        // Validation rule for DefaultParticipantRole
        RuleFor(x => x.DefaultParticipantRole)
            .Cascade(CascadeMode.Stop)
            .IsEnumName(typeof(RoomParticipantRole)).WithMessage("Default participant role is not valid!");

    }

}