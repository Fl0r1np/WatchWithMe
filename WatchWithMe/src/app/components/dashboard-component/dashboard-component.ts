import { Component, LOCALE_ID, OnInit, ChangeDetectorRef, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@services/auth-service/auth-service';  
import { ReactiveFormsModule, FormGroup, FormControl, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { environment } from '@environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { User } from '@app/models/user';
import { UserStatus } from '@app/models/user-status';
import { authMethod } from '@app/models/auth-method';
import { UserService } from '@app/services/user-service/user-service';
import { ApiEndpoints } from '@app/models/api-endpoints';
import { UserAccountUtils } from '@app/utils/UserAccountUtils';
import { NotificationType } from '@app/models/notification-type';
import { DashboadTabType } from '@app/models/dashboard-tab-type';
import { AssetsPaths } from '@app/models/assets-paths';

// Custom validator to check if new passwords match
export const matchNewPasswordValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const newPassword = control.get('newPassword');
  const confirmNewPassword = control.get('confirmNewPassword');
  if (newPassword && confirmNewPassword && newPassword.value !== confirmNewPassword.value) {
    confirmNewPassword.setErrors({ passwordMismatch: true });
    return { passwordMismatch: true };
  }
  return null;
};

@Component({
  selector: 'app-dashboard-component',
  imports: [ReactiveFormsModule],
  templateUrl: './dashboard-component.html',
  styleUrl: './dashboard-component.css', 
})
export class DashboardComponent implements OnInit {

  // Preset for avatars
  presetAvatars: string[] = [];

  // User info
  currentUser: User = {
    username: 'Username',
    email: 'user@example.com',
    profilePicture: `${AssetsPaths.avatars}avatar-default.png`,
    status: UserStatus.Public,
    displayStatus: UserStatus.Online,
    authMethod: authMethod.BASIC,
    notifyBasic: true,
    notifyInvitations: true
  };

  // Tracks if we are looking at the list or the content on mobile
  mobileViewMode: 'list' | 'content' = 'list';
  isMobile: boolean = false;

  // Navigation State
  activeTab: DashboadTabType = DashboadTabType.Account;
  activeTabTypes = {
    Account: DashboadTabType.Account,
    Notifications: DashboadTabType.Notifications
  };

  // Notification Preferences
  notifyInvitations: boolean = true;
  notifyBasic: boolean = true;

  // Modal States
  isProfilePicModalOpen: boolean = false;
  isStatusModalOpen: boolean = false;
  isPasswordModalOpen: boolean = false;
  isUsernameModalOpen: boolean = false;
  isEmailModalOpen: boolean = false;

  // Temporary selections for modals (before hitting save)
  selectedProfilePicTemp: string = '';
  selectedStatusTemp: UserStatus = UserStatus.Online;

  // Status options for the status modal
  userStatusOptions = {
    Public: UserStatus.Public,
    Private: UserStatus.Private,
    DoNotDisturb: UserStatus.DoNotDisturb,
    Online: UserStatus.Online,
    Offline: UserStatus.Offline,
    InCall: UserStatus.InCall,
    InRoom: UserStatus.InRoom
  };

  // Notification types
  notificationTypes = {
    Basic: NotificationType.Basic,
    Invitations: NotificationType.Invitations
  };

  // Reactive forms
  usernameForm = new FormGroup({
    newUsername: new FormControl('', [Validators.required, Validators.minLength(6)])
  });

  emailForm = new FormGroup({
    newEmail: new FormControl('', [Validators.required, Validators.email])
  });

  passwordForm = new FormGroup({
    currentPassword: new FormControl('', [Validators.required]),
    newPassword: new FormControl('', [Validators.required, Validators.minLength(8)]),
    confirmNewPassword: new FormControl('', [Validators.required])
  }, { validators: matchNewPasswordValidator });

  // Loading state
  isLoading = true;

  constructor(
    private authService: AuthService,
    private router: Router, 
    private http: HttpClient,
    private cdr: ChangeDetectorRef,
    private userService: UserService
  ) {}

  ngOnInit(): void {

    this.checkScreenSize();

    // Load user data from backend when component initializes
    this.loadUserData();
    
    // Load the avatars
    this.loadAvatars();

    console.log(this.presetAvatars);

  }

  // Method to check screen size and set mobile view accordingly
  @HostListener('window:resize')
  onResize() {
    this.checkScreenSize();
  }

  checkScreenSize() {
    this.isMobile = window.innerWidth <= 768;
    // On desktop, we always want to show content
    if (!this.isMobile) {
      this.mobileViewMode = 'content';
    }
  }

  // Method to load user data from backend
  loadUserData(): void {

    // Set loading state to true before fetching data
    this.isLoading = true;

    // Grab the token from storage
    const token = localStorage.getItem('auth_token'); 

    // Create the headers
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    // Get the user info 
    this.http.get(`${ApiEndpoints.getUserInfo}`, { headers })
      .subscribe({
        next: (response: any) => {

          // Update the currentUser object with data from backend (using defaults if any field is missing)
          this.currentUser = {
            username: response.username ?? this.currentUser.username,
            email: response.email ?? this.currentUser.email,
            profilePicture: response.profilePicture ?? this.currentUser.profilePicture,
            status: response.status ?? this.currentUser.status,
            displayStatus: response.displayStatus ?? this.currentUser.displayStatus,
            authMethod: response.authMethod ?? this.currentUser.authMethod,
            notifyBasic: response.notifyBasic ?? this.currentUser.notifyBasic,
            notifyInvitations: response.notifyInvitations ?? this.currentUser.notifyInvitations
          };

          // Notify the observers with the updated user data
          this.userService.updateUser(this.currentUser);

          // Setting the options
          this.notifyBasic = this.currentUser.notifyBasic;
          this.notifyInvitations = this.currentUser.notifyInvitations;

          // Set loading state to false after data is loaded
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: (err: any) => {
          console.error(err);
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });

  }
  
  // Method to load all available avatars
  loadAvatars(): void{

    // Try to get a list with all available avatars
    this.http.get<string[]>(`${ApiEndpoints.getAvatarList}`)
      .subscribe({
        next: (filenames : any) => {
          this.presetAvatars = filenames;
        },
        error: (err: any) => {
          console.log(err);
        }
      });

  }

  // Tab switching logic
  switchTab(tab: DashboadTabType): void {
    this.activeTab = tab;


    if (this.isMobile) {
      this.mobileViewMode = 'content';
    }
  }

  // Back to list logic for mobile
  goBackToList() {
    this.mobileViewMode = 'list';
    this.closeAllModals();
    this.closeCurrentTab();
  }

  // Modals logic 

  openUsernameModal(): void {
    this.usernameForm.reset({ newUsername: this.currentUser.username });
    this.isUsernameModalOpen = true;
  }

  saveUsername(): void {
    if (this.usernameForm.valid) {

      // Grab the token from storage
      const token = localStorage.getItem('auth_token'); 

      // Create the headers
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });

      // Create the request body
      const requestBody = { userName: this.usernameForm.value.newUsername };

      // Try to update the username on the backend
      this.http.put(`${ApiEndpoints.updateUserUsername}`, requestBody, { headers })
        .subscribe({
          next: (response: any) => {
            
            // Inform the user about the successful update
            console.log(response.message);

            // Update the username in the currentUser object
            this.currentUser.username = this.usernameForm.value.newUsername ?? this.currentUser.username;

            // Notify the observers
            this.userService.updateUser(this.currentUser);

            // Refresh the Front-End 
            this.isUsernameModalOpen = false;
            this.loadUserData(); 

          },
          error: (err: any) => {
            // Inform the user about the error 
            console.error(err.error);
            alert(err.error);
          }
        });

    } else {
      this.usernameForm.markAllAsTouched();
    }
  }

  openEmailModal(): void {
    this.emailForm.reset({ newEmail: this.currentUser.email });
    this.isEmailModalOpen = true;
  }

  saveEmail(): void {
    if (this.emailForm.valid) {
    
      // Grab the token from storage
      const token = localStorage.getItem('auth_token');

      // Create the headers
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });

      // Create the request body
      const requestBody = { email: this.emailForm.value.newEmail };

      // Try to update the email on the backend
      this.http.put(`${ApiEndpoints.updateUserEmail}`, requestBody, { headers })
        .subscribe({
          next: (response: any) => {
            // Inform the user about the successful update
            console.log(response.message);

            alert('Email changed! Please log in again with your new email.');
            this.logout();
          },
          error: (err: any) => {
            // Inform the user about the error 
            console.error(err.error);
            alert(err.error);
          }
        });

    } else {
      this.emailForm.markAllAsTouched();
    }
  }

  openProfilePicModal(): void {
    this.selectedProfilePicTemp = this.currentUser.profilePicture;
    this.isProfilePicModalOpen = true;
  }

  saveProfilePic(): void {
    
    // Grab the token from storage
    const token = localStorage.getItem('auth_token');

    // Create the headers
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });

    // Create the request body
    const requestBody = { profilePictureFilename: this.selectedProfilePicTemp };

    // Try to update the profile picture on the backend
    this.http.put(`${ApiEndpoints.updateUserProfilePicture}`, requestBody, { headers })
      .subscribe({
        next: (response: any) => {
          // Inform the user about the successful update
          console.log(response.message);

          // Update the profile picture in the currentUser object
          this.currentUser.profilePicture = this.selectedProfilePicTemp ?? this.currentUser.profilePicture;
          
          // Notify the observers
          this.userService.updateUser(this.currentUser);

          // Refresh the Front-End 
          this.isProfilePicModalOpen = false;
          this.loadUserData();
        },
        error: (err: any) => {
          // Inform the user about the error 
          console.error(err.error);
          alert(err.error);
        }
      });
  }

  openStatusModal(): void {
    this.selectedStatusTemp = this.currentUser.status;
    this.isStatusModalOpen = true;
  }

  saveStatus(): void {

    // Grab the token from storage
    const token = localStorage.getItem('auth_token');

    // Create the headers
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    // Create the request body
    const requestBody = { status: this.selectedStatusTemp.toString() };

    // Try to update the status on the backend
    this.http.put(`${ApiEndpoints.updateUserStatus}`, requestBody, { headers })
      .subscribe({
        next: (response: any) => {
          // Inform the user about the successful update
          console.log(response.message);

          // Update the status in the currentUser object
          this.currentUser.status = this.selectedStatusTemp ?? this.currentUser.status;
          
          // Notify the observers
          this.userService.updateUser(this.currentUser);

          // Refresh the Front-End 
          this.isStatusModalOpen = false;
          this.loadUserData();
        },
        error: (err: any) => {
          // Inform the user about the error 
          console.error(err.error);
          alert(err.error);
        }
      });


    this.currentUser.status = this.selectedStatusTemp;
    this.isStatusModalOpen = false;
  }

  openPasswordModal(): void {
    this.passwordForm.reset();
    this.isPasswordModalOpen = true;
  }

  savePassword(): void {
    if (this.passwordForm.valid) {
      
      // Grab the token from storage
      const token = localStorage.getItem('auth_token');

      // Create the headers
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });

      // Create the request body
      const requestBody = { 
        currentPassword: this.passwordForm.value.currentPassword, 
        newPassword: this.passwordForm.value.newPassword 
      };

      // Try to update the password on the backend
      this.http.put(`${ApiEndpoints.updateUserPassword}`, requestBody, { headers })
        .subscribe({
          next: (response: any) => {
            // Inform the user about the successful update
            console.log(response.message);  

            // Refresh the Front-End
            this.isPasswordModalOpen = false;
            this.loadUserData();
          },
          error: (err: any) => {
            // Inform the user about the error 
            console.error(err);
            alert(err.error.errors.CurrentPassword + "\n" + err.error.errors.NewPassword);
          }
        });

     
    } else {

      // Inform the user about the form errors
      console.error(this.passwordForm.errors);
      alert('The passwords do not match.');
      this.passwordForm.markAllAsTouched();
    }
  }

  closeAllModals(): void {
    this.isProfilePicModalOpen = false;
    this.isStatusModalOpen = false;
    this.isPasswordModalOpen = false;
    this.isUsernameModalOpen = false;
    this.isEmailModalOpen = false;
  }

  closeCurrentTab(): void {
    this.activeTab = null as any;
  }

  // Toggle notification checkboxes
  toggleNotification(type: NotificationType): void {
    
    // Change the selected option
    if (type === NotificationType.Invitations) {
      this.notifyInvitations = !this.notifyInvitations;
    } else if (type === NotificationType.Basic) {
      this.notifyBasic = !this.notifyBasic;
    }

    // Save the options
    this.saveNotification();

  }

  // Method to save the selected notification options 
  saveNotification(){

    // Grab the token from storage
    const token = localStorage.getItem('auth_token');

    // Create the headers
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    console.log("request: " + this.notifyBasic + " " + this.notifyInvitations);

    // Create the request body
    const requestBody = { 
      notifyBasic: this.notifyBasic,
      notifyInvitations: this.notifyInvitations 
    };

    // Try to update the options
    this.http.put(`${ApiEndpoints.updateNotificationOptions}`, requestBody, {headers})
      .subscribe({
          next: (response: any) => {
            // Inform the user about the successful update
            console.log(response.message);  

            // Refresh the Front-End
            this.loadUserData();
          },
          error: (err: any) => {
            // Inform the user about the error 
            console.error(err);
            alert(err.error.errors.notifyBasic + "\n" + err.error.errors.notifyInvitations);
          }
        });

  }

  // Logout logic
  async logout(): Promise<void> {

    // Perform logout operations
    await this.authService.logout();
    const result = await this.router.navigate(['/login']);

    // Check if navigation was successful
    if (!result) {
      console.error('Navigation to login failed after logout.');
    }
  }

  
  // Method to get the Profile Picture for the connected user
  getProfilePicture(filename: string | null | undefined): string {

    if (!filename) {
      return AssetsPaths.avatars + 'avatar-default.png';
    }

    return AssetsPaths.avatars + filename;

  }

   convertDisplayStatusToString(status: UserStatus): string {
  
      return UserAccountUtils.convertDisplayStatusToString(status);
      
    }

}
