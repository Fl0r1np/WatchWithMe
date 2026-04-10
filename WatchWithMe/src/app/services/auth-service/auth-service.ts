import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { UserService } from '../user-service/user-service';
import { UserStatus } from '@app/models/user-status';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  
  private readonly TOKEN_KEY = 'auth_token';

  constructor(
    private userService: UserService
  ){}

  // Save token to local storage
  saveToken(token: string): void {

    // Save the token
    localStorage.setItem(this.TOKEN_KEY, token);

    // Change user display status
    this.userService.updateUserDisplayStatus(null, UserStatus.Online);

  }

  // Get token from local storage
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  // Decode the token to get user information
  getDecodedToken(): any {

    const token = this.getToken();

    if (token) {

      try {
        return jwtDecode(token);
      } 
      catch (error) {
        return null;
      }

    }
    return null;

  }

  // Check if the user is authenticated and the token is not expired
  isAuthenticated(): boolean {

    const decodedToken = this.getDecodedToken();
    if( !decodedToken ) { 
      return false;
    }

    // Get the expiration time from the token and transform it to milliseconds
    const expirationTime = decodedToken.exp * 1000;
    const isExpired = Date.now() > expirationTime;

    if( isExpired ) {
      this.logout(); // Clear token if expired
      return false;
    }

    return true;

  }

  // Logout
  logout(): void {

    // Update the user display status
    this.userService.updateUserDisplayStatus(null, UserStatus.Offline);

    // Removing the token from local storage
    localStorage.removeItem(this.TOKEN_KEY);

  }


}
