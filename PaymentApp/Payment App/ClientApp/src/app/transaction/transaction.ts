// import { Component, OnInit, OnDestroy } from '@angular/core';
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
// export class TransactionHistoryComponent implements OnInit, OnDestroy {
//   transactions: Transaction[] = [];

//   // Filters
//   filterStatus: string = '';

//   statuses: string[] = ['Initiated', 'Pending', 'Success', 'Failed'];

//   private pollingIntervalId: any;

//   constructor(private http: HttpClient) {}

//   ngOnInit() {
//     this.loadTransactions();

//     // Start polling every 5 seconds (optional)
//     this.pollingIntervalId = setInterval(() => {
//       this.loadTransactions();
//     }, 5000);
//   }

//   ngOnDestroy() {
//     if (this.pollingIntervalId) {
//       clearInterval(this.pollingIntervalId);
//     }
//   }

//   loadTransactions() {
//     let params = new HttpParams();

//     if (this.filterStatus) {
//       params = params.set('status', this.filterStatus);
//     }

//     const token = localStorage.getItem('token');
//     const headers = new HttpHeaders({
//       'Content-Type': 'application/json',
//       'Authorization': `Bearer ${token}`
//     });

//     console.log('Calling API with params:', params.toString())

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






import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {jwtDecode} from 'jwt-decode';

interface Transaction {
  transactionId: string;
  userId: string;
  amount: number;
  currency: string;
  status: string;
  timestamp: string;
}

interface JwtPayload {
  role?: string | string[];
  [key: string]: any;
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

  // Filters and control flags
  individualTransactionId: string = '';
  showIndividualOnly: boolean = false;
  filterStatus: string = '';
  statuses: string[] = ['Initiated', 'Pending', 'Success', 'Failed'];

  private pollingIntervalId: any;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadTransactions();
    this.pollingIntervalId = setInterval(() => this.loadTransactions(), 5000);
  }

  ngOnDestroy() {
    if (this.pollingIntervalId) {
      clearInterval(this.pollingIntervalId);
    }
  }

  loadTransactions(): void {
    const token = localStorage.getItem('token');
    let headers = new HttpHeaders();

    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);

      // Decode token to get role
      let isAdmin = false;
      try {
        const decoded: JwtPayload = jwtDecode(token);
        const roles = Array.isArray(decoded.role) ? decoded.role : [decoded.role];
        isAdmin = roles.includes('Admin');
      } catch (err) {
        console.warn('Invalid JWT token format', err);
      }

      if (isAdmin) {
        // Admin view with optional status filter
        let params = new HttpParams();
        if (this.filterStatus) {
          params = params.set('status', this.filterStatus);
        }
        const url = 'https://localhost:7039/transactions';
        this.http.get<Transaction[]>(url, { headers, params }).subscribe({
          next: (data) => this.transactions = data,
          error: (err) => {
            console.error('Failed to load all transactions', err);
            this.transactions = [];
          }
        });
      } else  {
        // Individual search by transaction ID
        const url = 'https://localhost:7039/api/IndividualTransaction';
        this.http.get<Transaction>(url, { headers }).subscribe({
          next: (data) => this.transactions = [data],
          error: (err) => {
            console.error('Failed to load individual transaction', err);
            this.transactions = [];
          }
        });
      }
      
    }
  }

  onFilterChange() {
    this.loadTransactions();
  }

  onSearchIndividual() {
    if (this.individualTransactionId.trim()) {
      this.showIndividualOnly = true;
      this.loadTransactions();
    }
  }

  onReset() {
    this.showIndividualOnly = false;
    this.individualTransactionId = '';
    this.loadTransactions();
  }
}
