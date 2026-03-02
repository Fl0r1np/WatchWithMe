import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth-service';  


@Component({
  selector: 'app-dashboard-component',
  imports: [],
  template: `
    <div style="text-align: center;margin-top: 50px;">
      <h1>Welcome to the Dashboard, {{ userName }}!</h1>
      <p>Your email is: {{ userEmail }}</p>
      <button (click)="logout()">Logout</button>
    </div>
  `,
  styleUrl: './dashboard-component.css', 
})
export class DashboardComponent implements OnInit {

  // User info
  userName: string = '';
  userEmail: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {

    // Unpack the token
    var tokenPayload = this.authService.getDecodedToken();

    // Setting the user info
    if( tokenPayload ){

      this.userName = tokenPayload.given_name || 'None';
      this.userEmail = tokenPayload.name || 'None';

    }

  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

}
