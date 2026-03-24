using FluentValidation;
using WatchWithMeAPI.DTO;

namespace WatchWithMeAPI.Validators;

public class StatusUpdateRequestValidator : AbstractValidator<StatusUpdateRequestDTO>
{
    public StatusUpdateRequestValidator()
    {
        
    }
}