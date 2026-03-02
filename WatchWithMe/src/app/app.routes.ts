import { authGuard } from './auth-guard';
import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard-component/dashboard-component';
import { HomeComponent } from './home-component/home-component';
import { LoginComponent } from './login-component/login-component'; 
import { RegisterComponent } from './register-component/register-component';
import { LoginSuccessComponent } from './login-success-component/login-success-component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'login-success',
        component: LoginSuccessComponent
    },
    {
        path: 'register',
        component: RegisterComponent
    },
    {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [authGuard] // Protect the dashboard route with the auth guard
    },
    {
        path: '**',
        redirectTo: '404'
    }
];
