

// import { Component } from '@angular/core';
// import { HttpClient } from '@angular/common/http';
// import { FormsModule } from '@angular/forms';

// @Component({
//   selector: 'app-register',
//   standalone: true,
//   imports: [FormsModule],
//   templateUrl: './register.html',
//   styleUrls: ['./register.css']
// })
// export class Register {
//   registerData = {
//     email: '',
//     password: ''
//   };

//   constructor(private http: HttpClient) {}

//  onRegister() {
//   console.log('Registering user:', this.registerData);

//  console.log('Before HTTP post');

// this.http.post('https://localhost:7039/api/Auth/register', this.registerData, {
//   observe: 'response'
// }).subscribe({
//   next: (res) => {
//     console.log('Inside next:', res);
//     alert('Registration success!');
//   },
//   error: (err) => {
//     console.log('Inside error:', err);
//     alert('Registration failed!');
//   }
// });

// console.log('After HTTP post');

// }

// }


import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register {
  registerData = {
    email: '',
    password: '',
    confirmPassword: ''
  };

  constructor(private http: HttpClient) {}

  onRegister() {
    console.log('Registering user:', this.registerData);

    // Basic frontend validation
    if (!this.registerData.email || !this.registerData.password) {
      alert('⚠️ Please fill all fields');
      return;
    }



    console.log('📤 Before HTTP POST');

    this.http.post('https://localhost:7039/api/Auth/register', this.registerData, {
      observe: 'response'
    }).subscribe({
      next: (res) => {
        const body = res.body as { message?: string };
        console.log('✅ Inside next:', res);
        alert('✅ ' + (body?.message ?? 'User registered successfully!'));
      },
      error: (err) => {
        console.error('❌ Inside error:', err);

        if (err.error?.message) {
          alert('❌ ' + err.error.message);
        } else if (err.error?.errors) {
          // errors is array of strings (from your backend)
          const firstError = Array.isArray(err.error.errors) ? err.error.errors[0] : JSON.stringify(err.error.errors);
          alert('❌ ' + firstError);
        } else {
          alert('❌ Registration failed.');
        }
      },
      complete: () => {
        console.log('✔️ Request completed');
      }
    });

    console.log('📌 After HTTP POST');
  }
}

