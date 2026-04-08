import { Component, ChangeDetectorRef} from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ApiEndpoints } from '@app/utils/apiEndpoints';

// Custom validator to check if passwords match
export const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');

  // If both exist and their values don't match, we set an error on the confirmPassword control
  if (password && confirmPassword && password.value !== confirmPassword.value) {
    confirmPassword.setErrors({ passwordMismatch: true });
    return { passwordMismatch: true };
  }
  
  return null;
};

@Component({
  selector: 'app-register-component',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './register-component.html',
  styleUrl: './register-component.css',
})
export class RegisterComponent {
  
  isPasswordHidden: boolean = true;
  isConfirmPasswordHidden: boolean = true;
  isSubmitted: boolean = false;
  
  serverErrorMessage: string | null = null;

  // Initialize the form with all required inputs
  registerForm = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.minLength(6)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
    confirmPassword: new FormControl('', [Validators.required, Validators.minLength(8)])
  }, { validators: passwordMatchValidator }); // Attach the custom validator to the whole form

  constructor(private http: HttpClient, private router: Router, private cdr: ChangeDetectorRef) {}

  togglePasswordVisibility(): void {
    this.isPasswordHidden = !this.isPasswordHidden;
  }

  toggleConfirmPasswordVisibility(): void {
    this.isConfirmPasswordHidden = !this.isConfirmPasswordHidden;
  }

  // Helper method to check if an input is invalid
  isInvalid(controlName: string): boolean {
    const control = this.registerForm.get(controlName);
    return !!(control && control.invalid && (control.touched || this.isSubmitted));
  }

  // Method called when the user clicks the Register button
  onSubmit(): void {
    this.isSubmitted = true;
    this.serverErrorMessage = null;

    if (this.registerForm.valid) {
      console.log('Register Form is valid! Data:', this.registerForm.value);
      
      const registerData = {
        username: this.registerForm.get('username')?.value || '',
        email: this.registerForm.get('email')?.value || '',
        password: this.registerForm.get('password')?.value || '',
        confirmPassword: this.registerForm.get('confirmPassword')?.value || ''
      };

      // Send a POST request to the new backend endpoint
      this.http.post(`${ApiEndpoints.register}`, registerData)
        .subscribe({
          next: (response: any) => {
            // Redirect to the login page
            alert('Registration successful! Please log in.');
            this.router.navigate(['/login']);
          },
          error: (err) => {
              // If the backend returns a BadRequest, catch the errors
              console.error(err);

              if (err.error && Array.isArray(err.error)) {
          
                // Map over the array and extract just the "description" strings
                const errorMessages = err.error.map((e: any) => e.description);
                
                // Join them together into one string
                this.serverErrorMessage = errorMessages.join(' '); 
                
              } else {
                // Fallback for other types of errors (like standard 500 server errors)
                this.serverErrorMessage = err.error?.message || 'Registration failed. Please check your information and try again.';
              }

              // Force Angular to update the HTML immediately
              this.cdr.detectChanges(); 
            }
        });

    } else {
      this.registerForm.markAllAsTouched();
    }
  }

  loginWithGoogle(): void {
    window.location.href = `${ApiEndpoints.loginGoogle}?provider=Google`;
  }

}
