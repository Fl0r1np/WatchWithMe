import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd, Event } from '@angular/router';
import { HeaderComponent } from '@components/header-component/header-component';
import { FooterComponent } from '@components/footer-component/footer-component';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('WatchWithMe');

  // Variable to know when to show the header and footer
  showHeaderFooter: boolean = true;

  constructor(private router: Router) {
    
    // Listen to route changes
    this.router.events.pipe(
      filter((event: Event): event is NavigationEnd => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      if( event.urlAfterRedirects.includes('/login') || event.urlAfterRedirects.includes('/register') ){
        this.showHeaderFooter = false;
      }
      else{
        this.showHeaderFooter = true;
      }
    });

  }

}
