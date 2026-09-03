import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/auth/auth.guard';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { BookListComponent } from './features/books/book-list.component';
import { QuoteListComponent } from './features/quotes/quote-list.component';

export const routes: Routes = [
  { path: '', redirectTo: '/books', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [guestGuard] },
  { path: 'books', component: BookListComponent, canActivate: [authGuard] },
  { path: 'quotes', component: QuoteListComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '/books' },
];
