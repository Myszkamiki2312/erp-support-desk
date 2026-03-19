export interface MetricCard {
  title: string;
  value: string;
  accentClass: string;
  hint: string;
}

export interface ModuleLoad {
  module: string;
  tickets: number;
  progress: number;
}

export interface TicketSnapshot {
  id: number;
  number: string;
  title: string;
  client: string;
  module: string;
  priority: string;
  status: string;
  assignedEngineer: string;
  dueAt: string;
}

export interface DeadlineSnapshot {
  number: string;
  title: string;
  client: string;
  dueAt: string;
  status: string;
}

export interface ClientPulse {
  id: number;
  name: string;
  supportPlan: string;
  openTickets: number;
  activeUsers: number;
  integrationHealth: number;
  lastReviewAt: string;
}

export interface DashboardResponse {
  metrics: MetricCard[];
  moduleWorkload: ModuleLoad[];
  priorityQueue: TicketSnapshot[];
  integrationHealth: unknown[];
  upcomingDeadlines: DeadlineSnapshot[];
  clientPulse: ClientPulse[];
}

export interface TicketListItem {
  id: number;
  number: string;
  title: string;
  client: string;
  module: string;
  priority: string;
  status: string;
  assignedEngineer: string;
  plannedHours: number;
  spentHours: number;
  dueAt: string;
}

export interface SupportClient {
  id: number;
  name: string;
  erpEnvironment: string;
  supportPlan: string;
}

export interface SupportMetaResponse {
  clients: SupportClient[];
  statuses: string[];
  priorities: string[];
  modules: string[];
  sourceChannels: string[];
  engineers: string[];
}

export interface TicketFilters {
  search: string;
  status: string;
  priority: string;
  module: string;
}

export interface CreateTicketRequest {
  clientId: number;
  title: string;
  description: string;
  module: string;
  status: string;
  priority: string;
  assignedEngineer: string;
  plannedHours: number;
  spentHours: number;
  sourceChannel: string;
  affectedVersion: string;
  dueAt: string;
  requiresDeployment: boolean;
  isBillable: boolean;
}
