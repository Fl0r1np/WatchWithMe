import { Component } from '@angular/core';
import { RouterLink, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login-success',
  imports: [RouterLink],
  template: `
    <p>
      login-success works!
    </p>
  `,
  styleUrl: './login-success.css',
})
export class LoginSuccess {

}
