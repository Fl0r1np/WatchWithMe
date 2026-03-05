import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '@services/auth-service/auth-service';

@Component({
  selector: 'app-header-component',
  imports: [RouterLink],
  template: `
    <header class="main-header">
      
    <div class="main-header-brand" routerLink="/">
        <img src="/assets/brand/logo.png" alt="Logo WatchWithMe" class="main-header-logo">
        <h1 class="main-header-title">Watch <br> With Me</h1>
      </div>

      <div class="main-header-actions">
        
        @if (this.authService.isAuthenticated()){
          
          <div class="user-menu">

            <div class="user-menu-trigger">
              <img src="" alt="User Avatar" class="user-menu-avatar">
              <div class="user-menu-info">
                <span class="user-menu-name">{{username}}</span>
                <span class="user-menu-status">Status</span>
              </div>
            </div>

            <div class="user-menu-dropdown">
              <ul class="user-menu-list">
                <li class="user-menu-item"><a href="">Settings</a></li>
                <li class="user-menu-item"><a href="">New Room</a></li>
                <div class="container-notifications">
                  <span>Notifications</span>
                </div>
                <li class="user-menu-item"><button (click)="logout()" class="logout-button">Logout</button></li>
                
              </ul>
            </div>

          </div>

        }
        @else{

          <a routerLink="/login" class="login-button">Login</a>

        }

      </div>

    </header>
  `,
  styleUrl: './header-component.css',
})
export class HeaderComponent implements OnInit {

  username: string = 'User';

  constructor(public authService: AuthService) {}

  ngOnInit(): void {

    // Unpack the token to get the user info
    var tokenPayload = this.authService.getDecodedToken();

    // Setting the user info
    if( tokenPayload ){
      this.username = tokenPayload.given_name || 'User';
    }

  }

  logout() {
    this.authService.logout();
  }

}
