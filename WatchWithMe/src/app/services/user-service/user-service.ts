import { Injectable, signal} from '@angular/core';
import { environment } from '@environments/environment.development';
import { BehaviorSubject } from 'rxjs';
import { User } from '@models/user';
import { authMethod } from '@app/models/auth-method';
import { UserStatus } from '@app/models/user-status';

  
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

  // Method to update the data
  updateUser(newData: any) {
    this.userSource.next(newData);
  }

}
