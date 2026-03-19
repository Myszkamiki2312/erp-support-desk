import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ApiService } from '../core/api.service';
import { DashboardResponse } from '../core/models';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, DecimalPipe],
  template: `
    <section class="page">
      <div class="hero card">
        <div>
          <p class="eyebrow">Angular cockpit</p>
          <h2>Widok operacyjny dla supportu ERP</h2>
          <p class="hero-copy">
            Front SPA korzysta z tego samego backendu .NET i pokazuje kolejke priorytetow, obciazenie modulow oraz puls klientow.
          </p>
        </div>
        <div class="hero-actions">
          <a routerLink="/tickets" class="action-primary">Przejdz do zgloszen</a>
          <a href="http://localhost:5080" target="_blank" rel="noreferrer" class="action-secondary">Otworz MVC demo</a>
        </div>
      </div>

      @if (loading()) {
        <div class="card state">Trwa pobieranie danych dashboardu...</div>
      } @else if (error()) {
        <div class="card state state-error">{{ error() }}</div>
      } @else if (dashboard(); as view) {
        <div class="metrics-grid">
          @for (metric of view.metrics; track metric.title) {
            <article class="card metric-card">
              <p class="metric-title">{{ metric.title }}</p>
              <strong>{{ metric.value }}</strong>
              <span>{{ metric.hint }}</span>
            </article>
          }
        </div>

        <div class="content-grid">
          <section class="card">
            <div class="section-header">
              <h3>Obciazenie modulow</h3>
              <span>{{ view.moduleWorkload.length }} obszarow</span>
            </div>
            <div class="module-list">
              @for (module of view.moduleWorkload; track module.module) {
                <div class="module-row">
                  <div class="module-meta">
                    <strong>{{ module.module }}</strong>
                    <span>{{ module.tickets }} aktywnych tematow</span>
                  </div>
                  <div class="module-bar">
                    <div class="module-bar-fill" [style.width.%]="module.progress"></div>
                  </div>
                </div>
              }
            </div>
          </section>

          <section class="card">
            <div class="section-header">
              <h3>Deadline w najblizszych dniach</h3>
              <span>{{ view.upcomingDeadlines.length }} wpisow</span>
            </div>
            <div class="compact-table">
              @for (deadline of view.upcomingDeadlines; track deadline.number) {
                <div class="table-row">
                  <div>
                    <strong>{{ deadline.number }}</strong>
                    <p>{{ deadline.title }}</p>
                  </div>
                  <div class="table-side">
                    <span>{{ deadline.client }}</span>
                    <strong>{{ deadline.dueAt | date:'dd.MM HH:mm' }}</strong>
                  </div>
                </div>
              }
            </div>
          </section>
        </div>

        <div class="content-grid">
          <section class="card">
            <div class="section-header">
              <h3>Kolejka priorytetowa</h3>
              <span>{{ view.priorityQueue.length }} zgłoszen</span>
            </div>
            <div class="compact-table">
              @for (ticket of view.priorityQueue; track ticket.id) {
                <div class="table-row">
                  <div>
                    <strong>{{ ticket.number }}</strong>
                    <p>{{ ticket.title }}</p>
                  </div>
                  <div class="table-side">
                    <span>{{ ticket.priority }} / {{ ticket.status }}</span>
                    <strong>{{ ticket.assignedEngineer }}</strong>
                  </div>
                </div>
              }
            </div>
          </section>

          <section class="card">
            <div class="section-header">
              <h3>Puls klientow</h3>
              <span>{{ view.clientPulse.length }} srodowisk</span>
            </div>
            <div class="client-grid">
              @for (client of view.clientPulse; track client.id) {
                <article class="client-card">
                  <div>
                    <h4>{{ client.name }}</h4>
                    <p>{{ client.supportPlan }} plan wsparcia</p>
                  </div>
                  <div class="client-kpis">
                    <span>{{ client.openTickets }} open</span>
                    <span>{{ client.activeUsers }} users</span>
                    <span>{{ client.integrationHealth | number:'1.0-0' }}% health</span>
                  </div>
                </article>
              }
            </div>
          </section>
        </div>
      }
    </section>
  `,
  styles: [`
    .page {
      display: grid;
      gap: 20px;
    }

    .card {
      background: rgba(255, 255, 255, 0.88);
      border: 1px solid rgba(148, 163, 184, 0.28);
      border-radius: 28px;
      padding: 24px;
      box-shadow: 0 18px 50px rgba(15, 23, 42, 0.08);
      backdrop-filter: blur(10px);
    }

    .hero {
      display: flex;
      justify-content: space-between;
      gap: 20px;
      align-items: end;
      background:
        linear-gradient(135deg, rgba(15, 23, 42, 0.95), rgba(17, 94, 89, 0.92)),
        #0f172a;
      color: #f8fafc;
    }

    .eyebrow {
      margin: 0 0 8px;
      text-transform: uppercase;
      letter-spacing: 0.16em;
      font-size: 0.72rem;
      color: #fbbf24;
    }

    h2,
    h3,
    h4,
    p {
      margin: 0;
    }

    h2 {
      font-size: 2rem;
      margin-bottom: 12px;
    }

    .hero-copy {
      max-width: 700px;
      color: #dbeafe;
      line-height: 1.6;
    }

    .hero-actions {
      display: flex;
      flex-wrap: wrap;
      gap: 12px;
    }

    .action-primary,
    .action-secondary {
      text-decoration: none;
      border-radius: 999px;
      padding: 12px 18px;
      font-weight: 700;
      transition: 160ms ease;
    }

    .action-primary {
      background: #fbbf24;
      color: #1f2937;
    }

    .action-secondary {
      background: rgba(255, 255, 255, 0.12);
      color: #f8fafc;
      border: 1px solid rgba(255, 255, 255, 0.18);
    }

    .metrics-grid,
    .content-grid {
      display: grid;
      gap: 20px;
    }

    .metrics-grid {
      grid-template-columns: repeat(4, minmax(0, 1fr));
    }

    .content-grid {
      grid-template-columns: repeat(2, minmax(0, 1fr));
    }

    .metric-card {
      display: grid;
      gap: 10px;
    }

    .metric-title {
      color: #475569;
      font-size: 0.9rem;
    }

    .metric-card strong {
      font-size: 2rem;
      color: #0f172a;
    }

    .metric-card span {
      color: #64748b;
      line-height: 1.5;
    }

    .section-header {
      display: flex;
      align-items: baseline;
      justify-content: space-between;
      gap: 16px;
      margin-bottom: 18px;
    }

    .section-header span {
      color: #64748b;
      font-size: 0.9rem;
    }

    .module-list,
    .compact-table,
    .client-grid {
      display: grid;
      gap: 14px;
    }

    .module-row {
      display: grid;
      gap: 10px;
    }

    .module-meta {
      display: flex;
      justify-content: space-between;
      gap: 16px;
      color: #334155;
    }

    .module-bar {
      width: 100%;
      height: 10px;
      border-radius: 999px;
      background: #e2e8f0;
      overflow: hidden;
    }

    .module-bar-fill {
      height: 100%;
      border-radius: 999px;
      background: linear-gradient(90deg, #f97316, #14b8a6);
    }

    .table-row,
    .client-card {
      display: flex;
      justify-content: space-between;
      gap: 18px;
      padding: 14px 0;
      border-bottom: 1px solid #e2e8f0;
    }

    .table-row:last-child,
    .client-card:last-child {
      border-bottom: 0;
      padding-bottom: 0;
    }

    .table-row p,
    .client-card p {
      color: #64748b;
      margin-top: 4px;
    }

    .table-side {
      display: grid;
      gap: 6px;
      text-align: right;
      color: #475569;
      white-space: nowrap;
    }

    .client-card {
      align-items: center;
    }

    .client-kpis {
      display: flex;
      flex-wrap: wrap;
      gap: 10px;
      justify-content: end;
    }

    .client-kpis span {
      padding: 8px 12px;
      border-radius: 999px;
      background: #f1f5f9;
      color: #0f172a;
      font-size: 0.9rem;
    }

    .state {
      color: #334155;
    }

    .state-error {
      color: #b91c1c;
      border-color: rgba(239, 68, 68, 0.28);
      background: rgba(254, 242, 242, 0.92);
    }

    @media (max-width: 1100px) {
      .metrics-grid,
      .content-grid {
        grid-template-columns: 1fr;
      }

      .hero {
        flex-direction: column;
        align-items: start;
      }
    }
  `]
})
export class DashboardPageComponent {
  private readonly api = inject(ApiService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly dashboard = signal<DashboardResponse | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal('');

  constructor() {
    this.api.getDashboard()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.dashboard.set(response);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Nie udalo sie pobrac dashboardu. Upewnij sie, ze backend .NET dziala na porcie 5080.');
          this.loading.set(false);
        }
      });
  }
}
