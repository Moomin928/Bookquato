import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

export interface AuthResponse {
  userId: string;
  userName: string;
  token: string;
}

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface RegisterRequest {
  userName: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'http://localhost:5181/api';
  readonly user = signal<string | null>(this.getStoredUser());
  readonly isAuthenticated = signal<boolean>(this.hasToken());

  constructor(private readonly http: HttpClient) {}

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/login`, payload)
      .pipe(tap((response) => this.storeSession(response)));
  }

  register(payload: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/register`, payload)
      .pipe(tap((response) => this.storeSession(response)));
  }

  logout(): void {
    localStorage.removeItem('book_quote_token');
    localStorage.removeItem('book_quote_user');
    this.user.set(null);
    this.isAuthenticated.set(false);
  }

  getToken(): string | null {
    const token = localStorage.getItem('book_quote_token');

    if (!token) {
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      if (typeof payload.exp === 'number' && payload.exp <= Math.floor(Date.now() / 1000)) {
        this.logout();
        return null;
      }
      return token;
    } catch {
      this.logout();
      return null;
    }
  }

  private storeSession(response: AuthResponse): void {
    localStorage.setItem('book_quote_token', response.token);
    localStorage.setItem('book_quote_user', response.userName);
    this.user.set(response.userName);
    this.isAuthenticated.set(true);
  }

  private getStoredUser(): string | null {
    return localStorage.getItem('book_quote_user');
  }

  private hasToken(): boolean {
    return Boolean(localStorage.getItem('book_quote_token'));
  }
}
