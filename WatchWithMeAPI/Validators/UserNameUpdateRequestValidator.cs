using FluentValidation;
using Microsoft.AspNetCore.Identity;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Validators;

public class UserNameUpdateRequestValidator : AbstractValidator<UserNameUpdateRequestDTO>
{
    
    // Necessary services
    private readonly UserManager<User> _userManager;

    public UserNameUpdateRequestValidator(UserManager<User> userManager)
    {
        
        _userManager = userManager;
        
        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Username is required!")
            .MinimumLength(6).WithMessage("Username must be between 3 and 20 characters long!")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters and numbers!")
            .MustAsync(async (userName, cancellationToken) => await _userManager.FindByNameAsync(userName) == null).WithMessage("Username is already taken!");
        
    }
    
}