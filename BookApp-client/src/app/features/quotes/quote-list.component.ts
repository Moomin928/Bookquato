import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Quote, QuoteService } from '../../services/quote.service';

@Component({
  selector: 'app-quote-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <section class="page-shell container">
      <div class="page-header">
        <div>
          <p class="eyebrow">My favorites</p>
          <h1>Quotes</h1>
        </div>
      </div>

      <form [formGroup]="form" (ngSubmit)="submit()" class="quote-form">
        <h2>{{ editingId === null ? 'Add a quote' : 'Edit quote' }}</h2>
        <div class="field">
          <label>Quote</label>
          <textarea class="form-control" formControlName="text" rows="4" placeholder="Write a favorite quote..."></textarea>
        </div>
        <div class="field">
          <label>Author</label>
          <input class="form-control" formControlName="authorName" placeholder="Author or source" />
        </div>
        <button class="primary btn btn-primary" type="submit" [disabled]="form.invalid || submitting">
          {{ submitting ? 'Saving...' : 'Save quote' }}
        </button>
        <button *ngIf="editingId !== null" class="ghost btn btn-light" type="button" (click)="cancelEdit()">
          Cancel edit
        </button>
      </form>

      <div *ngIf="loading" class="state">Loading quotes...</div>
      <div *ngIf="!loading && quotes.length === 0" class="state">No quotes saved yet.</div>

      <div class="quote-list">
        <article class="quote-card card" *ngFor="let quote of quotes">
          <p class="quote-text">“{{ quote.text }}”</p>
          <div class="quote-footer">
            <span>{{ quote.authorName || 'Unknown author' }}</span>
            <div class="actions">
              <button type="button" class="ghost btn btn-light" (click)="fillForm(quote)">Edit</button>
              <button type="button" class="ghost danger btn btn-light" (click)="deleteQuote(quote.quoteId)">Delete</button>
            </div>
          </div>
        </article>
      </div>
    </section>
  `,
  styles: [
    `
      @use '../../shared/styles/feature-page' as feature-page;
      @include feature-page.base;

      .page-header { margin-bottom: 1.25rem; }
      .quote-form { display: grid; gap: 1rem; background: var(--panel); border: 1px solid rgba(148,163,184,0.25); border-radius: 18px; padding: 1rem; margin-bottom: 1.5rem; }
      .danger { background: #fee2e2; color: #991b1b; }
      .quote-list { display: grid; gap: 1rem; }
      .quote-card { background: var(--panel); border: 1px solid rgba(148,163,184,0.25); border-radius: 18px; padding: 1.2rem; }
      .quote-text { font-size: 1.15rem; line-height: 1.5; color: var(--text); margin: 0 0 1rem; }
      .quote-footer { display: flex; justify-content: space-between; align-items: center; gap: 1rem; }
      .actions { display: flex; gap: 0.5rem; }
      @media (max-width: 640px) {
        .quote-footer { flex-direction: column; align-items: stretch; }
        .actions { display: grid; grid-template-columns: 1fr 1fr; }
        .actions button { width: 100%; }
      }
    `,
  ],
})
export class QuoteListComponent implements OnInit {
  quotes: Quote[] = [];
  loading = true;
  submitting = false;
  editingId: number | null = null;

  form = new FormGroup({
    text: new FormControl('', [Validators.required]),
    authorName: new FormControl(''),
  });

  constructor(
    private readonly quoteService: QuoteService,
    private readonly changeDetectorRef: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadQuotes();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = {
      text: this.form.get('text')?.value ?? '',
      authorName: this.form.get('authorName')?.value ?? '',
    };

    this.submitting = true;

    if (this.editingId === null) {
      this.quoteService.create(payload).subscribe({
        next: () => {
          this.form.reset();
          this.loadQuotes();
        },
        error: () => {
          this.submitting = false;
        },
        complete: () => {
          this.submitting = false;
        },
      });
      return;
    }

    this.quoteService.update(this.editingId, payload).subscribe({
      next: () => {
        this.form.reset();
        this.editingId = null;
        this.loadQuotes();
      },
      error: () => {
        this.submitting = false;
      },
      complete: () => {
        this.submitting = false;
      },
    });
  }

  fillForm(quote: Quote): void {
    this.editingId = quote.quoteId;
    this.form.patchValue({
      text: quote.text,
      authorName: quote.authorName ?? '',
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.form.reset();
  }

  deleteQuote(id: number): void {
    if (!confirm('Delete this quote?')) {
      return;
    }

    this.quoteService.delete(id).subscribe({
      next: () => this.loadQuotes(),
      error: () => alert('Unable to delete quote.'),
    });
  }

  private loadQuotes(): void {
    this.loading = true;
    this.quoteService.getAll().subscribe({
      next: (quotes) => {
        this.quotes = quotes;
        this.loading = false;
        this.changeDetectorRef.markForCheck();
      },
      error: () => {
        this.loading = false;
        this.changeDetectorRef.markForCheck();
        alert('Please login to view your quotes.');
      },
    });
  }
}
