import { authGuard } from '@core/auth-guard/auth-guard';
import { Routes } from '@angular/router';
import { DashboardComponent } from '@components/dashboard-component/dashboard-component';
import { HomeComponent } from '@components/home-component/home-component';
import { LoginComponent } from '@components/login-component/login-component'; 
import { RegisterComponent } from '@components/register-component/register-component';
import { LoginSuccessComponent } from '@components/login-success-component/login-success-component';

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
