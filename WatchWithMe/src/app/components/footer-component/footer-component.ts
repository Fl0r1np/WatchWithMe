import { Component } from '@angular/core';

@Component({
  selector: 'app-footer-component',
  imports: [],
  template: `
    <footer class="main-footer">
  
  <div class="main-footer-contact">
    <span class="contact-label">Contact us:</span>
    <a href="mailto:hello@watchwithme.com" class="contact-email">hello&#64;watchwithme.com</a>
  </div>

  <div class="main-footer-copyright">
    &copy; 2026 WatchWithMe. All rights reserved.
  </div>

  <div class="main-footer-socials">
    <span class="socials-label">Follow us:</span>
    <div class="socials-icons">
      <a href="#" class="social-link" aria-label="Discord">
        <img src="assets/icons/discord-logo.png" alt="Discord" class="social-icon">
      </a>
      <a href="#" class="social-link" aria-label="LinkedIn">
        <img src="assets/icons/linkedin-logo.png" alt="LinkedIn" class="social-icon">
      </a>
      <a href="#" class="social-link" aria-label="Github">
        <img src="assets/icons/github-logo.png" alt="Github" class="social-icon">
      </a>
    </div>
  </div>

</footer>
  `,
  styleUrl: './footer-component.css',
})
export class FooterComponent {

}
