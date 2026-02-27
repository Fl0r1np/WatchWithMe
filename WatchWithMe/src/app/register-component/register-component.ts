import { Component, ChangeDetectorRef} from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';

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
  template: `
    <div class="register-page">
      <div class="register-card">
        <h1 class="register-card-title">Register</h1>

        <form class="register-form" [formGroup]="registerForm" (ngSubmit)="onSubmit()">
          
          <div class="form-group">
            <label for="username" class="form-label">Username</label>
            <input 
              type="text" 
              id="username" 
              formControlName="username"
              class="form-input" 
              [class.input-error]="isInvalid('username')"
              placeholder="Enter your username">
            
            @if (isInvalid('username')) {
              <div class="error-text">
                @if (registerForm.get('username')?.hasError('required')) {
                  <span>Username is required.</span>
                }
                @if (registerForm.get('username')?.hasError('minlength') && !registerForm.get('username')?.hasError('required')) {
                  <span>Username must be at least 6 characters.</span>
                }
              </div>
            }
          </div>

          <div class="form-group">
            <label for="email" class="form-label">Email</label>
            <input 
              type="email" 
              id="email" 
              formControlName="email"
              class="form-input" 
              [class.input-error]="isInvalid('email')"
              placeholder="Enter your email">
            
            @if (isInvalid('email')) {
              <div class="error-text">
                @if (registerForm.get('email')?.hasError('required')) {
                  <span>Email is required.</span>
                }
                @if (registerForm.get('email')?.hasError('email') && !registerForm.get('email')?.hasError('required')) {
                  <span>Please enter a valid email address.</span>
                }
              </div>
            }
          </div>

          <div class="form-group">
            <label for="password" class="form-label">Password</label>
            
            <div class="password-wrapper">
              <input 
                [type]="isPasswordHidden ? 'password' : 'text'" 
                id="password" 
                formControlName="password"
                class="form-input" 
                [class.input-error]="isInvalid('password')"
                placeholder="Create a password">
                
              <button type="button" class="password-toggle" (click)="togglePasswordVisibility()">
                {{ isPasswordHidden ? 'Show' : 'Hide' }}
              </button>
            </div>

            @if (isInvalid('password')) {
              <div class="error-text">
                @if (registerForm.get('password')?.hasError('required')) {
                  <span>Password is required.</span>
                }
                @if (registerForm.get('password')?.hasError('minlength') && !registerForm.get('password')?.hasError('required')) {
                  <span>Password must be at least 8 characters.</span>
                }
              </div>
            }
          </div>

          <div class="form-group">
            <label for="confirmPassword" class="form-label">Confirm Password</label>
            
            <div class="password-wrapper">
              <input 
                [type]="isConfirmPasswordHidden ? 'password' : 'text'" 
                id="confirmPassword" 
                formControlName="confirmPassword"
                class="form-input" 
                [class.input-error]="isInvalid('confirmPassword')"
                placeholder="Rewrite your password">
                
              <button type="button" class="password-toggle" (click)="toggleConfirmPasswordVisibility()">
                {{ isConfirmPasswordHidden ? 'Show' : 'Hide' }}
              </button>
            </div>

            @if (isInvalid('confirmPassword')) {
              <div class="error-text">
                @if (registerForm.get('confirmPassword')?.hasError('required')) {
                  <span>Please confirm your password.</span>
                }
                @if (registerForm.get('confirmPassword')?.hasError('passwordMismatch') && !registerForm.get('confirmPassword')?.hasError('required')) {
                  <span>Passwords do not match.</span>
                }
              </div>
            }
          </div>

          <button type="submit" class="btn btn-primary btn-full" (click)="onSubmit()">Register</button>

          <div class="form-footer">
            <span class="options-text">Already have an account?
              <a routerLink="/login" class="text-link">Sing in</a>
            </span>
          </div>
        </form>

        <div class="divider">
          <span class="divider-line"></span>
          <span class="divider-text">OR</span>
          <span class="divider-line"></span>
        </div>

        <div class="social-login">
          <button type="button" class="btn btn-google btn-full" (click)="loginWithGoogle()">
            <img src="/assets/icons/google-icon.svg" alt="Google Logo" class="google-icon">
            Continue with Google
          </button>
        </div>

        @if (serverErrorMessage) {
          <div class="global-error-box">
            {{ serverErrorMessage }}
          </div>
        }

      </div>
    </div>
  `,
  styleUrl: './register-component.css',
})
export class RegisterComponent {
  
  isPasswordHidden: boolean = true;
  isConfirmPasswordHidden: boolean = true;
  isSubmitted: boolean = false;
  
  serverErrorMessage: string | null = null;

  private apiUrl: string = environment.apiURL;

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
      this.http.post(`${this.apiUrl}/api/auth/register`, registerData)
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
    window.location.href = `${this.apiUrl}/api/auth/login-google?provider=Google`;
  }

}
