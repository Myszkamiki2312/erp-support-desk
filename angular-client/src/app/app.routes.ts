import { Routes } from '@angular/router';
import { DashboardPageComponent } from './pages/dashboard-page.component';
import { TicketsPageComponent } from './pages/tickets-page.component';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'dashboard'
  },
  {
    path: 'dashboard',
    component: DashboardPageComponent
  },
  {
    path: 'tickets',
    component: TicketsPageComponent
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
