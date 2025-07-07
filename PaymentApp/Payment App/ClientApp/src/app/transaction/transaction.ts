import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

interface Transaction {
  transactionId: string;
  userId: string;
  amount: number;
  currency: string;
  status: string;
  timestamp: string;
}

@Component({
  selector: 'app-transaction-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './transaction.html',
  styleUrls: ['./transaction.css']
})
export class TransactionHistoryComponent implements OnInit, OnDestroy {
  transactions: Transaction[] = [];

  // Filters
  filterStatus: string = '';

  statuses: string[] = ['Initiated', 'Pending', 'Success', 'Failed'];

  private pollingIntervalId: any;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadTransactions();

    // Start polling every 5 seconds (optional)
    this.pollingIntervalId = setInterval(() => {
      this.loadTransactions();
    }, 5000);
  }

  ngOnDestroy() {
    if (this.pollingIntervalId) {
      clearInterval(this.pollingIntervalId);
    }
  }

  loadTransactions() {
    let params = new HttpParams();

    if (this.filterStatus) {
      params = params.set('status', this.filterStatus);
    }

    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });

    console.log('Calling API with params:', params.toString())

    this.http.get<Transaction[]>('https://localhost:7039/transactions', {
      headers,
      params
    }).subscribe({
      next: (data) => {
        this.transactions = data;
      },
      error: (err) => {
        console.error('Failed to load transactions', err);
      }
    });
  }

  onFilterChange() {
    this.loadTransactions();
  }
}

















// import { Component, OnInit } from '@angular/core';
// import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
// import { FormsModule } from '@angular/forms';
// import { CommonModule } from '@angular/common';

// interface Transaction {
//   transactionId: string;
//   userId: string;
//   amount: number;
//   currency: string;
//   status: string;
//   timestamp: string;
// }

// @Component({
//   selector: 'app-transaction-history',
//   standalone: true,
//   imports: [CommonModule, FormsModule],
//   templateUrl: './transaction.html',
//   styleUrls: ['./transaction.css']
// })
// export class TransactionHistoryComponent implements OnInit {
//   transactions: Transaction[] = [];

//   // Filters
//   filterUserId = '';
//   filterStatus = '';

//   statuses = ['Initiated', 'Pending', 'Success', 'Failed'];

//   constructor(private http: HttpClient) {}

//   ngOnInit() {
//     this.loadTransactions();
//   }

//   loadTransactions() {
//     let params = new HttpParams();

//     if (this.filterUserId) {
//       params = params.set('userId', this.filterUserId);
//     }

//     if (this.filterStatus) {
//       params = params.set('status', this.filterStatus);
//     }

//     const token = localStorage.getItem('token');
//     const headers = new HttpHeaders({
//       'Content-Type': 'application/json',
//       'Authorization': `Bearer ${token}`
//     });

//     this.http.get<Transaction[]>('https://localhost:7039/transactions', {
//       headers,
//       params
//     }).subscribe({
//       next: (data) => {
//         this.transactions = data;
//       },
//       error: (err) => {
//         console.error('Failed to load transactions', err);
//       }
//     });
//   }

//   onFilterChange() {
//     this.loadTransactions();
//   }
// }
