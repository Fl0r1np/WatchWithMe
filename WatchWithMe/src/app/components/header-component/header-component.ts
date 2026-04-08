import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit, ChangeDetectorRef} from '@angular/core';
import { RouterLink } from '@angular/router';
import { environment } from '@environments/environment';
import { AuthService } from '@services/auth-service/auth-service';
import { UserService } from '@services/user-service/user-service';
import { AsyncPipe } from '@angular/common';
import { User } from '@app/models/user';
import { Observable } from 'rxjs';
import { ApiEndpoints } from '@app/models/apiEndpoints';
import { UserAccountUtils } from '@app/utils/UserAccountUtils';
import { UserStatus } from '@app/models/user-status';

@Component({
  selector: 'app-header-component',
  imports: [RouterLink, AsyncPipe],
  templateUrl: './header-component.html',
  styleUrl: './header-component.css',
})
export class HeaderComponent implements OnInit {

  // Observable for user data
  currentUser$: Observable<User>;

  // Active Tab State for the Inbox
  activeInboxTab: 'notifications' | 'invites' = 'notifications';

  // Profile Pictures location
  profilePicturesPath: string = environment.profilePicturesPath;

  constructor(
    public authService: AuthService, 
    private http: HttpClient, 
    private cdr: ChangeDetectorRef, 
    private userService: UserService
  ) {
     // We inject the service and grab the observable
    this.currentUser$ = this.userService.currentUser$;
  }

  ngOnInit(): void {

    // Load the current user data when the component initializes
    this.loadCurrentUser();

  }

  loadCurrentUser() {

    // Gret the token from the AuthService
    const token = this.authService.getToken();

    // Create headers with the token
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    // Try to get the current user data from the backend
    this.http.get<User>(`${ApiEndpoints.getUserInfo}`, { headers }).subscribe({
      next: (userData) => {

        // Set the user data in the UserService
        this.userService.updateUser(userData);

      },
      error: (err) => {
        console.error('Error fetching current user data:', err);
      } 
    });

  }

  logout() {
    this.authService.logout();
  }

  // Method to switch between states of Inbox
  switchInboxTab(tab: 'notifications' | 'invites'): void {
    this.activeInboxTab = tab;
  }

  // Method to get the Profile Picture for the connected user
  getProfilePicture(filename: string | null | undefined): string {

    if (!filename) {
      return this.profilePicturesPath + 'avatar-default.png';
    }

    return this.profilePicturesPath + filename;

  }

  convertDisplayStatusToString(status: UserStatus): string {

    return UserAccountUtils.convertDisplayStatusToString(status);
    
  }

}
