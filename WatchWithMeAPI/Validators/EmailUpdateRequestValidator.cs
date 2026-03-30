using FluentValidation;
using Microsoft.AspNetCore.Identity;
using WatchWithMeAPI.DTO;
using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Validators;

public class EmailUpdateRequestValidator : AbstractValidator<EmailUpdateRequestDTO>
{
    
    // Necessary services
    private readonly UserManager<User> _userManager;
    
    /// <summary>
    /// Constructor of the Validator
    /// </summary>
    /// <param name="userManager">
    /// Class containing all the logic for users repository management
    /// </param>
    public EmailUpdateRequestValidator( UserManager<User> userManager )
    {
        
        _userManager = userManager;
        
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required!")
            .EmailAddress().WithMessage("Email is not valid!")
            .MustAsync(IsEmailUnique).WithMessage("Email is already in use!");

    }

    /// <summary>
    /// Checks if the email is already in use
    /// </summary>
    /// <param name="email">
    /// A string containing the email to check
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token
    /// </param>
    /// <returns>
    /// True if the email is unique, false otherwise
    /// </returns>
    private async Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken)
    {
        // Check if the email is already in use
        var existingUser = await _userManager.FindByEmailAsync(email);
        
        // If the email is not in use, return true
        return existingUser == null;
    }

}