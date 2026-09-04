import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Book {
  bookId: number;
  title: string;
  authorName: string;
  publishedDate?: string | null;
}

export interface CreateBookRequest {
  title: string;
  authorName: string;
  publishedDate?: string | null;
}

@Injectable({ providedIn: 'root' })
export class BookService {
  private readonly apiUrl = 'https://bookquato.onrender.com/api';

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Book[]> {
    return this.http.get<Book[]>(`${this.apiUrl}/books`);
  }

  getById(id: number): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/books/${id}`);
  }

  create(payload: CreateBookRequest): Observable<Book> {
    return this.http.post<Book>(`${this.apiUrl}/books`, payload);
  }

  update(id: number, payload: CreateBookRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/books/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/books/${id}`);
  }
}
