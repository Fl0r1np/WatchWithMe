import { Injectable, signal} from '@angular/core';
import { environment } from '@environments/environment.development';
import { BehaviorSubject } from 'rxjs';
import { User } from '@models/user';
import { authMethod } from '@app/models/auth-method';
import { UserStatus } from '@app/models/user-status';
import { ApiEndpoints } from '@app/models/apiEndpoints';
import { HttpClient, HttpHeaders } from '@angular/common/http';

  
@Injectable({
  providedIn: 'root',
})
export class UserService {

  // Initial user data, can be updated when the user logs in or updates their profile
  private userSource = new BehaviorSubject<User>({
    username: 'Username',
    email: 'user@example.com',
    profilePicture: `avatar-default.png`,
    status: UserStatus.Public,
    displayStatus: UserStatus.Online,
    authMethod: authMethod.BASIC
  });

  // Expose the user data as an observable for components to subscribe to
  currentUser$ = this.userSource.asObservable();

  constructor(
    private http: HttpClient
  ){}

  // Method to update the data
  updateUser(newData: User) {
    this.userSource.next(newData);
  }

  // Method to update the User Display Status
  updateUserDisplayStatus(currentUser: User | null, newDisplayStatus: UserStatus){

    // Grab the token from storage
    const token = localStorage.getItem('auth_token'); 

    // Create the headers
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    // Create the request
    var requestBody = { displayStatus: newDisplayStatus.toString() };

    // Try to update the display status on the backend
    this.http.put(`${ApiEndpoints.updateUserDisplayStatus}`, requestBody, { headers })
      .subscribe({
        next: (response: any) => {
          // Inform the user about the successful update
          console.log(response.message);

          // Update the user source if a user is given 
          if (currentUser !== null){  
            // Update the status in the currentUser object
            currentUser.displayStatus = newDisplayStatus || currentUser.status;
          
            // Notify the observers
            this.updateUser(currentUser);
          }
          
        },
        error: (err: any) => {
          // Inform the user about the error 
          console.error(err.error);
        }
      });

  }

}
