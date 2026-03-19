import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateTicketRequest,
  DashboardResponse,
  SupportMetaResponse,
  TicketFilters,
  TicketListItem
} from './models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly http = inject(HttpClient);

  getDashboard(): Observable<DashboardResponse> {
    return this.http.get<DashboardResponse>('/api/dashboard');
  }

  getTickets(filters: TicketFilters): Observable<TicketListItem[]> {
    let params = new HttpParams();

    if (filters.search) {
      params = params.set('search', filters.search);
    }

    if (filters.status) {
      params = params.set('status', filters.status);
    }

    if (filters.priority) {
      params = params.set('priority', filters.priority);
    }

    if (filters.module) {
      params = params.set('module', filters.module);
    }

    return this.http.get<TicketListItem[]>('/api/tickets', { params });
  }

  getSupportMeta(): Observable<SupportMetaResponse> {
    return this.http.get<SupportMetaResponse>('/api/support/meta');
  }

  createTicket(payload: CreateTicketRequest): Observable<unknown> {
    return this.http.post('/api/tickets', payload);
  }
}
