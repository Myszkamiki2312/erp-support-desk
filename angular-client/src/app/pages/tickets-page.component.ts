import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';
import { ApiService } from '../core/api.service';
import {
  CreateTicketRequest,
  SupportMetaResponse,
  TicketFilters,
  TicketListItem
} from '../core/models';

@Component({
  selector: 'app-tickets-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="page">
      <div class="page-header">
        <div>
          <p class="eyebrow">Angular workspace</p>
          <h2>Zgloszenia serwisowe w Angularze</h2>
          <p class="subtitle">
            Widok wykorzystuje API .NET i lekki frontend Angular do filtrowania, przegladu i dodawania zgłoszen.
          </p>
        </div>
        <button type="button" class="primary-button" (click)="loadTickets()">Odswiez liste</button>
      </div>

      @if (loading() || saving()) {
        <div class="banner banner-info">Trwa przetwarzanie danych...</div>
      }

      @if (metaError()) {
        <div class="banner banner-error">{{ metaError() }}</div>
      }

      @if (successMessage()) {
        <div class="banner banner-success">{{ successMessage() }}</div>
      }

      <section class="panel">
        <div class="section-head">
          <h3>Filtry operacyjne</h3>
          <span>{{ tickets().length }} rekordow</span>
        </div>

        <div class="filter-grid">
          <label class="field">
            <span>Szukaj</span>
            <input type="text" [(ngModel)]="filters.search" (input)="loadTickets()" placeholder="Numer, temat lub klient" />
          </label>

          <label class="field">
            <span>Status</span>
            <select [(ngModel)]="filters.status" (change)="loadTickets()">
              <option value="">Wszystkie</option>
              @for (item of meta()?.statuses ?? []; track item) {
                <option [value]="item">{{ item }}</option>
              }
            </select>
          </label>

          <label class="field">
            <span>Priorytet</span>
            <select [(ngModel)]="filters.priority" (change)="loadTickets()">
              <option value="">Wszystkie</option>
              @for (item of meta()?.priorities ?? []; track item) {
                <option [value]="item">{{ item }}</option>
              }
            </select>
          </label>

          <label class="field">
            <span>Modul</span>
            <select [(ngModel)]="filters.module" (change)="loadTickets()">
              <option value="">Wszystkie</option>
              @for (item of meta()?.modules ?? []; track item) {
                <option [value]="item">{{ item }}</option>
              }
            </select>
          </label>
        </div>

        <div class="table-shell">
          <table class="ticket-table">
            <thead>
              <tr>
                <th>Numer</th>
                <th>Temat</th>
                <th>Klient</th>
                <th>Modul</th>
                <th>Priorytet</th>
                <th>Status</th>
                <th>Prowadzacy</th>
                <th>Plan h</th>
                <th>Wyk h</th>
                <th>Termin</th>
              </tr>
            </thead>
            <tbody>
              @for (ticket of tickets(); track ticket.id) {
                <tr>
                  <td>{{ ticket.number }}</td>
                  <td>
                    <strong>{{ ticket.title }}</strong>
                  </td>
                  <td>{{ ticket.client }}</td>
                  <td>{{ ticket.module }}</td>
                  <td>
                    <span class="pill">{{ ticket.priority }}</span>
                  </td>
                  <td>{{ ticket.status }}</td>
                  <td>{{ ticket.assignedEngineer }}</td>
                  <td>{{ ticket.plannedHours }}</td>
                  <td>{{ ticket.spentHours }}</td>
                  <td>{{ formatDateTime(ticket.dueAt) }}</td>
                </tr>
              } @empty {
                <tr>
                  <td colspan="10" class="empty-state">Brak zgłoszen dla aktualnych filtrow.</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </section>

      <section class="panel">
        <div class="section-head">
          <h3>Dodaj nowe zgloszenie</h3>
          <span>Przeplyw API POST /api/tickets</span>
        </div>

        <form class="form-grid" (ngSubmit)="submitTicket()">
          <label class="field">
            <span>Klient</span>
            <select name="clientId" [(ngModel)]="draft.clientId">
              @for (client of meta()?.clients ?? []; track client.id) {
                <option [ngValue]="client.id">{{ client.name }}</option>
              }
            </select>
          </label>

          <label class="field">
            <span>Modul</span>
            <select name="module" [(ngModel)]="draft.module">
              @for (item of meta()?.modules ?? []; track item) {
                <option [value]="item">{{ item }}</option>
              }
            </select>
          </label>

          <label class="field">
            <span>Status</span>
            <select name="status" [(ngModel)]="draft.status">
              @for (item of meta()?.statuses ?? []; track item) {
                <option [value]="item">{{ item }}</option>
              }
            </select>
          </label>

          <label class="field">
            <span>Priorytet</span>
            <select name="priority" [(ngModel)]="draft.priority">
              @for (item of meta()?.priorities ?? []; track item) {
                <option [value]="item">{{ item }}</option>
              }
            </select>
          </label>

          <label class="field">
            <span>Osoba odpowiedzialna</span>
            <select name="assignedEngineer" [(ngModel)]="draft.assignedEngineer">
              @for (item of meta()?.engineers ?? []; track item) {
                <option [value]="item">{{ item }}</option>
              }
            </select>
          </label>

          <label class="field">
            <span>Zrodlo zgloszenia</span>
            <select name="sourceChannel" [(ngModel)]="draft.sourceChannel">
              @for (item of meta()?.sourceChannels ?? []; track item) {
                <option [value]="item">{{ item }}</option>
              }
            </select>
          </label>

          <label class="field field-full">
            <span>Temat</span>
            <input type="text" name="title" [(ngModel)]="draft.title" placeholder="Np. blad synchronizacji dokumentow magazynowych" />
          </label>

          <label class="field">
            <span>Wersja systemu</span>
            <input type="text" name="affectedVersion" [(ngModel)]="draft.affectedVersion" />
          </label>

          <label class="field">
            <span>Termin</span>
            <input type="datetime-local" name="dueAt" [(ngModel)]="draft.dueAt" />
          </label>

          <label class="field">
            <span>Planowane godziny</span>
            <input type="number" min="0" max="500" step="0.5" name="plannedHours" [(ngModel)]="draft.plannedHours" />
          </label>

          <label class="field">
            <span>Przepracowane godziny</span>
            <input type="number" min="0" max="500" step="0.5" name="spentHours" [(ngModel)]="draft.spentHours" />
          </label>

          <label class="field field-full">
            <span>Opis techniczny</span>
            <textarea name="description" rows="6" [(ngModel)]="draft.description"></textarea>
          </label>

          <div class="checkbox-row field-full">
            <label class="checkbox">
              <input type="checkbox" name="requiresDeployment" [(ngModel)]="draft.requiresDeployment" />
              <span>Wymaga wdrozenia na produkcje</span>
            </label>
            <label class="checkbox">
              <input type="checkbox" name="isBillable" [(ngModel)]="draft.isBillable" />
              <span>Rozlicz jako prace platna</span>
            </label>
          </div>

          <div class="form-actions field-full">
            <button type="submit" class="primary-button">Dodaj zgloszenie</button>
            <button type="button" class="secondary-button" (click)="resetDraft()">Wyczysc formularz</button>
          </div>
        </form>
      </section>
    </section>
  `,
  styles: [`
    .page {
      display: grid;
      gap: 20px;
    }

    .page-header,
    .section-head {
      display: flex;
      align-items: end;
      justify-content: space-between;
      gap: 16px;
    }

    .eyebrow {
      margin: 0 0 8px;
      text-transform: uppercase;
      letter-spacing: 0.16em;
      font-size: 0.72rem;
      color: #b45309;
    }

    h2,
    h3,
    p {
      margin: 0;
    }

    .subtitle {
      margin-top: 10px;
      color: #475569;
      line-height: 1.6;
      max-width: 720px;
    }

    .panel {
      padding: 24px;
      border-radius: 28px;
      background: rgba(255, 255, 255, 0.9);
      border: 1px solid rgba(148, 163, 184, 0.28);
      box-shadow: 0 18px 50px rgba(15, 23, 42, 0.08);
      backdrop-filter: blur(10px);
    }

    .section-head {
      margin-bottom: 18px;
    }

    .section-head span {
      color: #64748b;
      font-size: 0.92rem;
    }

    .filter-grid,
    .form-grid {
      display: grid;
      gap: 16px;
    }

    .filter-grid {
      grid-template-columns: repeat(4, minmax(0, 1fr));
      margin-bottom: 20px;
    }

    .form-grid {
      grid-template-columns: repeat(2, minmax(0, 1fr));
    }

    .field {
      display: grid;
      gap: 8px;
    }

    .field span {
      font-size: 0.92rem;
      font-weight: 700;
      color: #334155;
    }

    .field input,
    .field select,
    .field textarea {
      width: 100%;
      border: 1px solid #cbd5e1;
      border-radius: 14px;
      padding: 12px 14px;
      background: #fff;
      color: #0f172a;
    }

    .field textarea {
      resize: vertical;
      min-height: 150px;
    }

    .field-full {
      grid-column: 1 / -1;
    }

    .table-shell {
      overflow-x: auto;
      border: 1px solid #e2e8f0;
      border-radius: 18px;
    }

    .ticket-table {
      width: 100%;
      border-collapse: collapse;
      min-width: 1080px;
      background: #fff;
    }

    .ticket-table th,
    .ticket-table td {
      padding: 14px 16px;
      border-bottom: 1px solid #e2e8f0;
      text-align: left;
      vertical-align: top;
      color: #0f172a;
    }

    .ticket-table th {
      background: #f8fafc;
      font-size: 0.82rem;
      text-transform: uppercase;
      letter-spacing: 0.08em;
      color: #64748b;
    }

    .ticket-table tbody tr:hover {
      background: #f8fafc;
    }

    .pill {
      display: inline-flex;
      align-items: center;
      padding: 6px 10px;
      border-radius: 999px;
      background: #fff7ed;
      color: #c2410c;
      font-size: 0.82rem;
      font-weight: 700;
    }

    .empty-state {
      text-align: center;
      color: #64748b;
      padding: 28px 16px;
    }

    .banner {
      padding: 14px 18px;
      border-radius: 18px;
      font-weight: 600;
    }

    .banner-info {
      background: rgba(239, 246, 255, 0.96);
      color: #1d4ed8;
      border: 1px solid rgba(59, 130, 246, 0.22);
    }

    .banner-success {
      background: rgba(236, 253, 245, 0.96);
      color: #166534;
      border: 1px solid rgba(34, 197, 94, 0.28);
    }

    .banner-error {
      background: rgba(254, 242, 242, 0.96);
      color: #b91c1c;
      border: 1px solid rgba(239, 68, 68, 0.28);
    }

    .checkbox-row,
    .form-actions {
      display: flex;
      gap: 16px;
      flex-wrap: wrap;
      align-items: center;
    }

    .checkbox {
      display: inline-flex;
      align-items: center;
      gap: 10px;
      color: #334155;
      font-weight: 600;
    }

    .primary-button,
    .secondary-button {
      border: 0;
      border-radius: 999px;
      padding: 12px 18px;
      font-weight: 700;
      cursor: pointer;
      transition: 160ms ease;
    }

    .primary-button {
      background: linear-gradient(135deg, #f97316, #ea580c);
      color: #fff;
      box-shadow: 0 10px 24px rgba(234, 88, 12, 0.22);
    }

    .secondary-button {
      background: #e2e8f0;
      color: #0f172a;
    }

    @media (max-width: 1100px) {
      .filter-grid,
      .form-grid {
        grid-template-columns: 1fr;
      }

      .page-header,
      .section-head {
        flex-direction: column;
        align-items: start;
      }
    }
  `]
})
export class TicketsPageComponent {
  private readonly api = inject(ApiService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly meta = signal<SupportMetaResponse | null>(null);
  protected readonly metaError = signal('');
  protected readonly tickets = signal<TicketListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly successMessage = signal('');

  protected readonly filters: TicketFilters = {
    search: '',
    status: '',
    priority: '',
    module: ''
  };

  protected draft = this.createEmptyDraft();

  constructor() {
    this.loadMeta();
    this.loadTickets();
  }

  protected loadMeta(): void {
    this.api.getSupportMeta()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.meta.set(response);
          this.metaError.set('');
          this.applyMetaDefaults(response);
        },
        error: () => {
          this.metaError.set('Nie udalo sie pobrac slownikow dla Angulara. Sprawdz, czy backend .NET dziala na porcie 5080.');
        }
      });
  }

  protected loadTickets(): void {
    this.loading.set(true);

    this.api.getTickets(this.filters)
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: (response) => this.tickets.set(response),
        error: () => this.metaError.set('Nie udalo sie pobrac listy zgloszen z API.')
      });
  }

  protected submitTicket(): void {
    if (!this.isDraftValid()) {
      this.metaError.set('Uzupelnij klienta, temat, opis oraz podstawowe pola zgloszenia.');
      return;
    }

    this.metaError.set('');
    this.successMessage.set('');
    this.saving.set(true);

    const payload: CreateTicketRequest = {
      clientId: this.draft.clientId,
      title: this.draft.title.trim(),
      description: this.draft.description.trim(),
      module: this.draft.module,
      status: this.draft.status,
      priority: this.draft.priority,
      assignedEngineer: this.draft.assignedEngineer,
      plannedHours: this.draft.plannedHours,
      spentHours: this.draft.spentHours,
      sourceChannel: this.draft.sourceChannel,
      affectedVersion: this.draft.affectedVersion.trim(),
      dueAt: new Date(this.draft.dueAt).toISOString(),
      requiresDeployment: this.draft.requiresDeployment,
      isBillable: this.draft.isBillable
    };

    this.api.createTicket(payload)
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          this.successMessage.set('Zgloszenie zostalo dodane przez klient Angular i zapisane w backendzie .NET.');
          this.resetDraft();
          this.loadTickets();
        },
        error: () => {
          this.metaError.set('Nie udalo sie zapisac zgloszenia. Sprawdz, czy backend jest uruchomiony i baza jest dostepna.');
        }
      });
  }

  protected resetDraft(): void {
    this.draft = this.createEmptyDraft();
    const currentMeta = this.meta();

    if (currentMeta) {
      this.applyMetaDefaults(currentMeta);
    }
  }

  protected formatDateTime(value: string): string {
    return new Intl.DateTimeFormat('pl-PL', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(new Date(value));
  }

  private applyMetaDefaults(meta: SupportMetaResponse): void {
    this.draft = {
      ...this.draft,
      clientId: this.draft.clientId || meta.clients[0]?.id || 0,
      module: this.draft.module || meta.modules[0] || '',
      status: this.draft.status || meta.statuses[0] || '',
      priority: this.draft.priority || meta.priorities[2] || meta.priorities[0] || '',
      assignedEngineer: this.draft.assignedEngineer || meta.engineers[0] || '',
      sourceChannel: this.draft.sourceChannel || meta.sourceChannels[0] || ''
    };
  }

  private createEmptyDraft() {
    const dueDate = new Date();
    dueDate.setDate(dueDate.getDate() + 2);
    dueDate.setHours(12, 0, 0, 0);

    return {
      clientId: 0,
      title: '',
      description: '',
      module: '',
      status: '',
      priority: '',
      assignedEngineer: '',
      plannedHours: 4,
      spentHours: 0,
      sourceChannel: '',
      affectedVersion: 'Comarch ERP XL 2024.1',
      dueAt: this.toDateTimeLocalValue(dueDate),
      requiresDeployment: false,
      isBillable: true
    };
  }

  private toDateTimeLocalValue(date: Date): string {
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60_000);
    return localDate.toISOString().slice(0, 16);
  }

  private isDraftValid(): boolean {
    return Boolean(
      this.draft.clientId &&
      this.draft.title.trim() &&
      this.draft.description.trim() &&
      this.draft.module &&
      this.draft.status &&
      this.draft.priority &&
      this.draft.assignedEngineer &&
      this.draft.sourceChannel &&
      this.draft.affectedVersion.trim() &&
      this.draft.dueAt
    );
  }
}
