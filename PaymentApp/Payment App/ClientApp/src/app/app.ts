
// import { Component } from '@angular/core';
// import { Router, RouterOutlet } from '@angular/router';
// import { CommonModule } from '@angular/common';
// import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
// import { AuthInterceptor } from './auth.interceptor'; // adjust path accordingly

// @Component({
//   selector: 'app-root',
//   standalone: true,
//   imports: [RouterOutlet, CommonModule, HttpClientModule],
//   providers: [
//     { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
//   ],
//   templateUrl: './app.html',
//   styleUrls: ['./app.css']
// })
// export class App {
//   isLoggedIn = false;

//   constructor(private router: Router) {
//     this.router.events.subscribe(() => {
//       this.isLoggedIn = localStorage.getItem('isLoggedIn') === 'true';
//     });
//   }

//   goToPaymentForm() {
//     this.router.navigate(['/payment']);
//   }

//   goToTransactionHistory() {
//     this.router.navigate(['/transaction']);
//   }

//   onLoginClick() {
//     this.router.navigate(['/login']);
//   }

//   onRegisterClick() {
//     this.router.navigate(['/register']);
//   }

//   onLogout() {
//     localStorage.removeItem('isLoggedIn');
//     localStorage.removeItem('token');
//     this.isLoggedIn = false;
//     this.router.navigate(['/login']);
//   }
// }

import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';
import {jwtDecode} from 'jwt-decode';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, HttpClientModule],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  isLoggedIn = false;
  userRole: string | null = null;

  constructor(private router: Router) {
    this.router.events.subscribe(() => {
      this.isLoggedIn = localStorage.getItem('isLoggedIn') === 'true';

      const token = localStorage.getItem('token');
      if (token) {
        try {
          const decoded: any = jwtDecode(token);
          const roles = decoded['role'];

          // Handle single or multiple roles
          this.userRole = Array.isArray(roles) ? roles[0] : roles;
        } catch (err) {
          console.error('Token decoding failed', err);
          this.userRole = null;
        }
      } else {
        this.userRole = null;
      }
    });
  }

  get isAdmin(): boolean {
    return this.userRole === 'Admin';
  }

  get isUser(): boolean {
    return this.userRole === 'User';
  }

  goToPaymentForm() {
    this.router.navigate(['/payment']);
  }

  goToTransactionHistory() {
    this.router.navigate(['/transaction']);
  }

    goToTransactionHistoryIndividual() {
    this.router.navigate(['/transaction']);
  }

  onLoginClick() {
    this.router.navigate(['/login']);
  }

  onRegisterClick() {
    this.router.navigate(['/register']);
  }

  onLogout() {
    localStorage.removeItem('isLoggedIn');
    localStorage.removeItem('token');
    this.isLoggedIn = false;
    this.userRole = null;
    this.router.navigate(['/login']);
  }
}
