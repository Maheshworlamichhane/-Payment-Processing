// import { RouterModule, Routes } from '@angular/router';
// import { Login} from './auth/login/login'; 
// import { Register } from './auth/register/register';
// import { Dashboard} from './Dashboard/dashboard';
// import { PaymentComponent } from './payment/payment';
// import { TransactionHistoryComponent } from './transaction/transaction';

// export const routes: Routes = [
//      { path: 'login', component: Login },
//        { path: '', redirectTo: 'login', pathMatch: 'full' },
//         { path: 'register', component: Register },
//        { path: '', redirectTo: 'register', pathMatch: 'full' },
//          { path: 'dashboard', component: Dashboard},
//          { path: 'payment', component: PaymentComponent },
//          { path: 'transaction', component: TransactionHistoryComponent },
//       { path: '**', redirectTo: 'login' }
//       ];



import { RouterModule, Routes } from '@angular/router';
import { Login} from './auth/login/login'; 
import { Register } from './auth/register/register';
import { Dashboard} from './Dashboard/dashboard';
import { PaymentComponent } from './payment/payment';
import { TransactionHistoryComponent } from './transaction/transaction';
import { AuthGuard } from './auth.guard';  // adjust path

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'dashboard', component: Dashboard, canActivate: [AuthGuard] },
  { path: 'payment', component: PaymentComponent, canActivate: [AuthGuard] },
  { path: 'transaction', component: TransactionHistoryComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];
