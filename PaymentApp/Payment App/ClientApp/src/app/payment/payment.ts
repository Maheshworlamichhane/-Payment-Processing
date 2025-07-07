// import { Component } from '@angular/core';
// import { FormsModule, NgForm } from '@angular/forms';
// import { CommonModule } from '@angular/common';
// import { HttpClient, HttpClientModule } from '@angular/common/http';

// @Component({
//   selector: 'app-payment-form',
//   standalone: true,
//   imports: [FormsModule, CommonModule, HttpClientModule],
//   templateUrl: './payment.html',
//   styleUrls: ['./payment.css']
// })
// export class PaymentComponent {
//   amount!: number;
//   currency = 'USD';
//   name = '';
//   email = '';
//   paymentMethod: 'Card' | 'bank' = 'Card';
//   cardNumber = '';
//   bankAccountNumber = '';
//   description = '';
//   currencies = ['USD', 'EUR', 'GBP', 'INR'];
//   userid='';
//   constructor(private http: HttpClient) {}

//   onSubmit(form: NgForm) {
//     if (form.valid) {
//       const payload = {
//         CustomerName: this.name,
//         Currency: this.currency,
//         Email: this.email,
//         Amount: this.amount,
//         PaymentMethod: this.paymentMethod,
//         CardOrAccountNumber:
//           this.paymentMethod === 'Card' ? this.cardNumber : this.bankAccountNumber,
//         Description: this.description,
//         UserId: localStorage.getItem('userId') || ''
//       };

//       this.http.post('https://localhost:7039/api/Payments/process', payload)
//         .subscribe({
//           next: (response) => {
//             alert('✅ Payment submitted successfully!');
//             console.log('API response:', response);
//             form.resetForm({ currency: 'USD', paymentMethod: 'card' });
//           },
//           error: (error) => {
//             console.error('❌ Payment submission failed:', error);
//             alert('❌ Payment failed. Check your form or try again.');
//           }
//         });
//     } else {
//       alert('Please fill all required fields correctly.');
//     }
//   }
// }

import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-payment-form',
  standalone: true,
  imports: [FormsModule, CommonModule, HttpClientModule],
  templateUrl: './payment.html',
  styleUrls: ['./payment.css']
})
export class PaymentComponent {
  amount!: number;
  currency = 'USD';
  currencies = ['USD', 'EUR', 'NPR'];

  name: string = '';
  email: string = '';
  paymentMethod: 'Card' | 'BankTransfer' = 'Card';
  cardOrAccountNumber: string = '';
  description: string = '';
  userid: string = '';

  constructor(private http: HttpClient) {}

  onSubmit(form: NgForm) {
    if (form.invalid) {
      alert('Please fill all required fields correctly.');
      return;
    }

    const payload = {
      customerName: this.name,
      currency: this.currency,
      email: this.email,
      amount: this.amount,
      paymentMethod: this.paymentMethod,
      cardOrAccountNumber: this.cardOrAccountNumber,
      description: this.description,
      userId:''
    };
const token = localStorage.getItem('token'); // get JWT from storage

  const headers = new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${token}` // attach token
  });
    console.log('✅ Sending Payment:', payload);

    this.http.post('https://localhost:7039/api/Payments/process', payload, { headers })
      .subscribe({
        next: (response) => {
          alert('✅ Payment submitted successfully!');
          console.log('API response:', response);
          form.resetForm({ currency: 'USD', paymentMethod: 'Card' });
        },
        error: (error) => {
          console.error('❌ Payment submission failed:', error);
          alert('❌ Payment failed. Check your form or try again.');
        }
      });
  }
}
