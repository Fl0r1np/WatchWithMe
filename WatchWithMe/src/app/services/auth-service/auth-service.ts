import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  
  private readonly TOKEN_KEY = 'auth_token';

  // Save token to local storage
  saveToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
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
    localStorage.removeItem(this.TOKEN_KEY);
  }


}
