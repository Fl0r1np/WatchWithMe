using System.Data;
using FluentValidation;
using WatchWithMeAPI.DTO;

namespace WatchWithMeAPI.Validators;

public class NotificationOptionsRequestValidator: AbstractValidator<NotificationOptionsUpdateRequestDTO>
{
    
    public NotificationOptionsRequestValidator()
    {
    }
    
}