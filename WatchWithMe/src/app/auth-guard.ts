import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth-service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  
  // Injecting the services
  const authService = inject(AuthService);
  const router = inject(Router);

  // Check if the user is authenticated
  if (authService.isAuthenticated()) {
    return true; // Allow access to the route
  }
  else{
    router.navigate(['/login']); // Redirect to the login page
    return false; // Deny access to the route
  }

};
