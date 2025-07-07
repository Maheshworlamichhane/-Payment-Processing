
// import { Component } from '@angular/core';
// import { HttpClient } from '@angular/common/http';
// import { FormsModule } from '@angular/forms';
// import { Router } from '@angular/router';

// @Component({
//   selector: 'app-login',
//   standalone: true,
//   imports: [FormsModule],
//   templateUrl: './login.html',
//   styleUrls: ['./login.css']
// })
// export class Login {
//   loginData = {
//     email: '',
//     password: ''
//   };

//   constructor(private http: HttpClient, private router: Router) {}

//   onSubmit() {
//     console.log('Sending login request:', this.loginData);

//     this.http.post('https://localhost:7039/api/Auth/login', this.loginData)
//       .subscribe({
//         next: (res: any) => {
//           console.log('Login successful', res);
//           alert("Login Success");
//           localStorage.setItem('token', res.token); // or response.jwt, depending on your API
//           // Navigate to dashboard
//           localStorage.setItem('isLoggedIn', 'true');
//           this.router.navigate(['/dashboard']);

          
//         },
//         error: (err) => {
//           console.error('Login failed', err);
//           alert("Login Failed");
//         }
//       });
//   }
// }


import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode'; 

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
  loginData = {
    email: '',
    password: ''
  };

  constructor(private http: HttpClient, private router: Router) {}

  onSubmit() {
    console.log('Sending login request:', this.loginData);

    this.http.post('https://localhost:7039/api/Auth/login', this.loginData)
      .subscribe({
        next: (res: any) => {
          console.log('Login successful', res);
          alert("Login Success");
          localStorage.setItem('token', res.token);
          localStorage.setItem('isLoggedIn', 'true');

          // ✅ Decode token to check role
          const decodedToken: any = jwtDecode(res.token);
          const roles = decodedToken['role']; // role could be a string or array

          console.log('Decoded roles:', roles);

          if (roles.includes('Admin')) {
            this.router.navigate(['/dashboard']);
          } else if (roles.includes('User')) {
            this.router.navigate(['/dashboard']);
          } else {
            alert('Unknown role');
          }
        },
        error: (err) => {
          console.error('Login failed', err);
          alert("Login Failed");
        }
      });
  }
}
