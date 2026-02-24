import { Component } from '@angular/core';

@Component({
  selector: 'app-header-component',
  imports: [],
  template: `
    <header class="main-header">
      
    <div class="main-header-brand">
        <img src="/assets/brand/logo.png" alt="Logo WatchWithMe" class="main-header-logo">
        <h1 class="main-header-title">Watch <br> With Me</h1>
      </div>

      <div class="main-header-actions">
        
        @if (false){
          
          <div class="user-menu">

            <div class="user-menu-trigger">
              <img src="" alt="User Avatar" class="user-menu-avatar">
              <div class="user-menu-info">
                <span class="user-menu-name">Username</span>
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
                <li class="user-menu-item"><a href="">Logout</a></li>
                
              </ul>
            </div>

          </div>

        }
        @else{

          <button class="login-button">Login</button>

        }

      </div>

    </header>
  `,
  styleUrl: './header-component.css',
})
export class HeaderComponent {

}
