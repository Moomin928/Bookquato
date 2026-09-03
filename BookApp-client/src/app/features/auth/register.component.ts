import { CommonModule, NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, NgIf],
  template: `
    <section class="auth-shell container">
      <div class="auth-card card">
        <h1>Create account</h1>
        <p>Register to save your favorite books and quotes.</p>

        <form [formGroup]="form" (ngSubmit)="submit()">
          <label>
            Username
            <input class="form-control" type="text" formControlName="userName" placeholder="Choose a username" />
          </label>

          <label>
            Password
            <input class="form-control" type="password" formControlName="password" placeholder="Create password" />
          </label>

          <button class="btn btn-primary" type="submit" [disabled]="form.invalid || submitting">
            {{ submitting ? 'Creating account...' : 'Register' }}
          </button>

          <p class="error" *ngIf="errorMessage">{{ errorMessage }}</p>
        </form>

        <div class="auth-footer">
          Already have an account?
          <a routerLink="/login">Login</a>
        </div>
      </div>
    </section>
  `,
  styleUrl: './auth.scss',
})
export class RegisterComponent {
  protected form = new FormGroup({
    userName: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });

  protected submitting = false;
  protected errorMessage = '';

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {}

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = {
      userName: this.form.get('userName')?.value ?? '',
      password: this.form.get('password')?.value ?? '',
    };

    this.submitting = true;
    this.errorMessage = '';

    this.authService.register(payload).subscribe({
      next: () => this.router.navigate(['/books']),
      error: () => {
        this.errorMessage = 'This username is already taken.';
        this.submitting = false;
      },
      complete: () => {
        this.submitting = false;
      },
    });
  }
}
