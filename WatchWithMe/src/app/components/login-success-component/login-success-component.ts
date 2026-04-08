import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '@services/auth-service/auth-service';

@Component({
  selector: 'app-login-success',
  imports: [],
  templateUrl: './login-success-component.html',
  styleUrl: './login-success-component.css',
})
export class LoginSuccessComponent implements OnInit {

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ){}

  ngOnInit(): void {

    this.route.queryParams.subscribe(params => {

      // Get the token from the query parameters
      const token = params['token'];

      if (token) {

        // Save the token
        this.authService.saveToken(token);

        // Redirect to the home page
        this.router.navigate(['/']);

      } else {

        // If no token is found, redirect to the login page
        this.router.navigate(['/login']);

      }



    });

  }

}
