
// import { Component } from '@angular/core';
// import { Router, RouterOutlet } from '@angular/router';
// import { CommonModule } from '@angular/common';
// import { Login } from "./auth/login/login";

// @Component({
//   selector: 'app-root',
//   standalone: true,
//   imports: [RouterOutlet, CommonModule],
//   templateUrl: './app.html',
//   styleUrls: ['./app.css']
// })
// export class App {
//   isLoggedIn = false;

//   constructor(private router: Router) {
//     // Update login state on every route change
//     this.router.events.subscribe(() => {
//       this.isLoggedIn = localStorage.getItem('isLoggedIn') === 'true';
//     });
//   }
//   goToPaymentForm() {
//     this.router.navigate(['/payment']);
//   }
//     goToTransactionHistory() {
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
//     this.isLoggedIn = false;
//     this.router.navigate(['/login']);
//   }
  
// }


import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor'; // adjust path accordingly

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

  constructor(private router: Router) {
    this.router.events.subscribe(() => {
      this.isLoggedIn = localStorage.getItem('isLoggedIn') === 'true';
    });
  }

  goToPaymentForm() {
    this.router.navigate(['/payment']);
  }

  goToTransactionHistory() {
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
    this.router.navigate(['/login']);
  }
}
