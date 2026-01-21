import { Routes } from '@angular/router';
import { PageDashboard } from './features/page-dashboard/page-dashboard';
import { Layout } from './layout/layout';
import { PageLogin } from './features/page-login/page-login';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: PageLogin },
  {
    path: '',
    component: Layout,
    runGuardsAndResolvers: 'always',
    children: [{ path: 'dashboard', component: PageDashboard }],
  },
];
