import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Quote {
  quoteId: number;
  text: string;
  authorName?: string | null;
  createAtUtc: string;
}

export interface QuoteRequest {
  text: string;
  authorName?: string | null;
}

@Injectable({ providedIn: 'root' })
export class QuoteService {
  private readonly apiUrl = 'http://localhost:5181/api';

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Quote[]> {
    return this.http.get<Quote[]>(`${this.apiUrl}/quotes`);
  }

  create(payload: QuoteRequest): Observable<Quote> {
    return this.http.post<Quote>(`${this.apiUrl}/quotes`, payload);
  }

  update(id: number, payload: QuoteRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/quotes/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/quotes/${id}`);
  }
}
