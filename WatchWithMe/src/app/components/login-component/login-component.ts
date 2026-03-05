import { AuthService } from '@services/auth-service/auth-service';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment.development';

@Component({
  selector: 'app-login-component',
  imports: [RouterLink, ReactiveFormsModule],
  template: `
    <div class="login-page">
      <div class="login-card">
        <h1 class="login-card-title">Login</h1>

        <form class="login-form" [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          
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
                @if (loginForm.get('email')?.hasError('required')) {
                  <span>Email is required.</span>
                }
                @if (loginForm.get('email')?.hasError('email') && !loginForm.get('email')?.hasError('required')) {
                  <span>Please enter a valid email address.</span>
                }
              </div>
            }
          </div>

          <div class="form-group">
            <div class="form-label-group">
              <label for="password" class="form-label">Password</label>
              <a routerLink="/forgot-password" class="forgot-link">Forgot password?</a>
            </div>
            
            <div class="password-wrapper">
              <input 
                [type]="isPasswordHidden ? 'password' : 'text'" 
                id="password" 
                formControlName="password"
                class="form-input" 
                [class.input-error]="isInvalid('password')"
                placeholder="Enter your password">
                
              <button type="button" class="password-toggle" (click)="togglePasswordVisibility()">
                {{ isPasswordHidden ? 'Show' : 'Hide' }}
              </button>
            </div>

            @if (isInvalid('password')) {
              <div class="error-text">
                @if (loginForm.get('password')?.hasError('required')) {
                  <span>Password is required.</span>
                }
                @if( loginForm.get('password')?.hasError('minlength') && !loginForm.get('password')?.hasError('required')) {
                  <span>Password must be at least 8 characters long.</span>
                }
              </div>
            }
          </div>

          <button type="submit" class="btn btn--primary btn--full">Login</button>

          <div class="form-footer">
            <span class="options-text">Don't have an account? 
              <a routerLink="/register" class="text-link">Sign up</a>
            </span>
          </div>
        </form>

        <div class="divider">
          <span class="divider-line"></span>
          <span class="divider-text">OR</span>
          <span class="divider-line"></span>
        </div>

        <div class="social-login">
          <button type="button" class="btn btn--google btn--full" (click)="loginWithGoogle()">
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
  styleUrl: './login-component.css',
})
export class LoginComponent {

  // Variable that decide if the password is visible or not
  isPasswordHidden: boolean = true;

  // Check if the form is submitted
  isSubmitted: boolean = false;

  // Variable containing the error message
  serverErrorMessage: string | null = null;

  // Defing the form group for the login form
  loginForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)])
  });

  // Backend API URL
  private apiUrl: string = environment.apiURL;

  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router, private cdr: ChangeDetectorRef, private authService: AuthService) {}

  // Checking if there is a query parameter indicating a error from the backend
  ngOnInit(): void {
  
    // Listen for query parameters
    // Especially for Google Auth
    this.route.queryParams.subscribe(params => {

      const errorCode = params['error'];

      if (errorCode) {
        
        // Map the error codes to user-friendly messages

        switch (errorCode) {
          
          case 'google_auth_failed':
            this.serverErrorMessage = 'Google authentication was cancelled or failed. Please try again.';
            break;
          case 'registration_failed':
            this.serverErrorMessage = 'We received your Google info, but could not create an account in our database. Please contact support.';
            break;
          case 'unknown_error':
            this.serverErrorMessage = 'An unexpected error occurred during login. Please try again later.';
            break;
          default:
            this.serverErrorMessage = 'Something went wrong. Please try logging in again.';
            break;

        }

      }

    });
    

  }

  // Function to toggle the visibility of the password
  togglePasswordVisibility(): void {
    this.isPasswordHidden = !this.isPasswordHidden;
  }

  // Function to verify if an input field has an error
  isInvalid(controlName: string): boolean {
    const control = this.loginForm.get(controlName);
    return !!(control && control.invalid && (control.touched || this.isSubmitted));
  }

  // Function to handle the form submission
  onSubmit(): void {
    
    this.isSubmitted = true;
    this.serverErrorMessage = null;

    if (this.loginForm.valid) {

      console.log('Form is valid! Data:', this.loginForm.value);

      const loginData = {
        email: this.loginForm.get('email')?.value || '',
        password: this.loginForm.get('password')?.value || ''
       };

        // Send a POST request to the new backend endpoint
      this.http.post(`${this.apiUrl}/api/auth/login`, loginData)
        .subscribe({
          next: (response: any) => {

            console.log('Login successful! Response:', response);

            // Setting the token
            this.authService.saveToken(response.accessToken);

            // Redirecting to the dashboard
            this.router.navigate(['/dashboard']);

          },
          error: (err) => {
            // If the backend returns a BadRequest, catch the errors here
            console.error(err);

            if (err.error && Array.isArray(err.error)) {
          
                // 2. Map over the array and extract just the "description" strings
                const errorMessages = err.error.map((e: any) => e.description);
                
                // 3. Join them together into one string (separated by a space or a new line)
                this.serverErrorMessage = errorMessages.join(' '); 
                
              } else {
                // 4. Fallback for other types of errors (like standard 500 server errors)
                this.serverErrorMessage = err.error?.message || 'Login failed. Please check your credentials and try again.';
              }

              // Force Angular to update the HTML immediately
              this.cdr.detectChanges();
            
          }
        });
    }
    else{
      // If the is invalid, we show the error messages for all the fields
      console.log('Form is invalid. Please correct the errors and try again.');
      this.loginForm.markAllAsTouched();
    }

  }

  loginWithGoogle(): void {
    window.location.href = `${this.apiUrl}/api/auth/login-google?provider=Google`;
  }

}
