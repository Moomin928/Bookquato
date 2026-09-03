import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    <nav class="topbar navbar navbar-expand-md">
      <div class="brand navbar-brand"><i class="fa-solid fa-book-open" aria-hidden="true"></i> Book Quote</div>
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#mainNavigation"
        aria-controls="mainNavigation"
        [attr.aria-expanded]="menuOpen()"
        aria-label="Toggle navigation"
        (click)="toggleMenu()"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" [class.show]="menuOpen()" id="mainNavigation">
        <div class="nav-links navbar-nav" *ngIf="authService.isAuthenticated()">
          <a class="nav-link" routerLink="/books" routerLinkActive="active" (click)="closeMenu()">Books</a>
          <a class="nav-link" routerLink="/quotes" routerLinkActive="active" (click)="closeMenu()">Quotes</a>
        </div>
        <div class="nav-actions ms-auto">
          <button class="theme-toggle" type="button" (click)="toggleTheme()">
            <i
              class="fa-solid"
              [class.fa-sun]="!isDarkMode()"
              [class.fa-moon]="isDarkMode()"
              aria-hidden="true"
            ></i>
            {{ isDarkMode() ? 'Light' : 'Dark' }} mode
          </button>
          <ng-container *ngIf="authService.isAuthenticated(); else guestLinks">
            <span class="user-label">{{ authService.user() }}</span>
            <button class="logout" type="button" (click)="logout()">Logout</button>
          </ng-container>
          <ng-template #guestLinks>
            <a routerLink="/login" class="login-link" (click)="closeMenu()">Login</a>
          </ng-template>
        </div>
      </div>
    </nav>
  `,
  styles: [
    `
      :host { display: block; }
      .topbar {
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 1rem;
        padding: 1rem 1.5rem;
        background: rgba(15, 23, 42, 0.9);
        color: white;
        position: sticky;
        top: 0;
        z-index: 10;
        backdrop-filter: blur(12px);
      }
      .brand { font-size: 1.2rem; font-weight: 800; letter-spacing: 0.06em; text-transform: uppercase; }
      .nav-links, .nav-actions { display: flex; align-items: center; gap: 1rem; min-width: 0; }
      .navbar-toggler { border-color: rgba(255,255,255,0.35); }
      .navbar-toggler-icon { filter: invert(1); }
      a { color: white; text-decoration: none; opacity: 0.8; }
      a.active { opacity: 1; font-weight: 700; }
      .login-link, .logout, .theme-toggle { border: 1px solid rgba(255,255,255,0.2); border-radius: 999px; background: transparent; color: white; text-decoration: none; padding: 0.55rem 0.9rem; cursor: pointer; }
      .logout { background: rgba(255,255,255,0.08); }
      .user-label { opacity: 0.9; max-width: 12rem; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
      @media (max-width: 640px) {
        .topbar { padding: 0.8rem 1rem; }
        .navbar-collapse { padding-top: 0.75rem; }
        .nav-links, .nav-actions { width: 100%; justify-content: space-between; }
        .nav-actions { gap: 0.5rem; flex-wrap: wrap; }
        .user-label { max-width: 8rem; }
        .login-link, .logout, .theme-toggle { padding: 0.55rem 0.7rem; }
      }
    `,
  ],
})
export class NavComponent {
  readonly isDarkMode = signal(false);
  readonly menuOpen = signal(false);

  constructor(
    protected readonly authService: AuthService,
    private readonly router: Router,
  ) {
    const savedTheme = localStorage.getItem('book_quote_theme');
    const dark = savedTheme === 'dark';
    this.isDarkMode.set(dark);
    document.documentElement.setAttribute('data-theme', dark ? 'dark' : 'light');
  }

  toggleTheme(): void {
    const next = !this.isDarkMode();
    this.isDarkMode.set(next);
    document.documentElement.setAttribute('data-theme', next ? 'dark' : 'light');
    localStorage.setItem('book_quote_theme', next ? 'dark' : 'light');
  }

  toggleMenu(): void {
    this.menuOpen.update((open) => !open);
  }

  closeMenu(): void {
    this.menuOpen.set(false);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
