import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { Book, BookService } from '../../services/book.service';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <section class="page-shell container">
      <div class="page-header">
        <div>
          <p class="eyebrow">Library</p>
          <h1>Books</h1>
        </div>
        <button class="primary btn btn-primary" type="button" (click)="toggleForm()">
          {{ showForm ? 'Close form' : 'Add book' }}
        </button>
      </div>

      <form *ngIf="showForm" [formGroup]="form" (ngSubmit)="submit()" class="book-form">
        <div class="field">
          <label>Title</label>
          <input class="form-control" formControlName="title" />
        </div>
        <div class="field">
          <label>Author</label>
          <input class="form-control" formControlName="authorName" />
        </div>
        <div class="field">
          <label>Published date</label>
          <input class="form-control" type="date" formControlName="publishedDate" />
        </div>
        <button class="primary btn btn-primary" type="submit" [disabled]="form.invalid || submitting">
          {{ submitting ? 'Saving...' : 'Save book' }}
        </button>
      </form>

      <div *ngIf="loading" class="state">Loading books...</div>
      <div *ngIf="!loading && errorMessage" class="state error-state">{{ errorMessage }}</div>
      <div *ngIf="!loading && books.length === 0" class="state">No books yet. Add the first one.</div>

      <div class="book-grid">
        <article class="book-card card" *ngFor="let book of books">
          <div class="meta-row">
            <span class="badge">Book</span>
            <button type="button" class="ghost btn btn-light" (click)="deleteBook(book.bookId)">Delete</button>
          </div>
          <h2>{{ book.title }}</h2>
          <p><strong>Author:</strong> {{ book.authorName }}</p>
          <p><strong>Published:</strong> {{ book.publishedDate || 'N/A' }}</p>
        </article>
      </div>
    </section>
  `,
  styles: [
    `
      @use '../../shared/styles/feature-page' as feature-page;
      @include feature-page.base;

      .book-form { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem; background: var(--panel); border: 1px solid rgba(148,163,184,0.25); border-radius: 18px; padding: 1rem; margin-bottom: 1.5rem; }
      .book-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 1rem; }
      .book-card { background: var(--panel); border: 1px solid rgba(148,163,184,0.25); border-radius: 18px; padding: 1.25rem; box-shadow: 0 14px 35px rgba(15,23,42,0.05); }
      .meta-row { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
      .badge { background: #eef2ff; color: #4338ca; padding: 0.35rem 0.6rem; border-radius: 999px; font-size: 0.74rem; font-weight: 700; }
      h2 { margin: 0 0 0.75rem; font-size: 1.35rem; }
      p { margin: 0.35rem 0; color: var(--muted-text); }
      @media (max-width: 640px) {
        .book-form { grid-template-columns: 1fr; padding: 0.9rem; }
        .book-grid { grid-template-columns: 1fr; }
      }
    `,
  ],
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  loading = true;
  showForm = false;
  submitting = false;
  errorMessage = '';

  form = new FormGroup({
    title: new FormControl('', [Validators.required]),
    authorName: new FormControl('', [Validators.required]),
    publishedDate: new FormControl(''),
  });

  constructor(
    private readonly bookService: BookService,
    private readonly router: Router,
    private readonly changeDetectorRef: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) {
      this.form.reset();
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = {
      title: this.form.get('title')?.value ?? '',
      authorName: this.form.get('authorName')?.value ?? '',
      publishedDate: this.form.get('publishedDate')?.value ?? null,
    };

    this.submitting = true;

    this.bookService.create(payload).subscribe({
      next: () => {
        this.form.reset();
        this.showForm = false;
        this.loadBooks();
      },
      error: () => {
        this.submitting = false;
      },
      complete: () => {
        this.submitting = false;
      },
    });
  }

  deleteBook(id: number): void {
    if (!confirm('Delete this book?')) {
      return;
    }

    this.bookService.delete(id).subscribe({
      next: () => this.loadBooks(),
      error: () => alert('Unable to delete book.'),
    });
  }

  private loadBooks(): void {
    this.loading = true;
    this.errorMessage = '';
    this.bookService.getAll().pipe(finalize(() => {
      this.loading = false;
      this.changeDetectorRef.markForCheck();
    })).subscribe({
      next: (books) => {
        this.books = books;
        this.changeDetectorRef.markForCheck();
      },
      error: () => {
        this.books = [];
        this.errorMessage = 'Unable to load books. Please sign in again.';
        this.changeDetectorRef.markForCheck();
        this.router.navigate(['/login']);
      },
    });
  }
}
