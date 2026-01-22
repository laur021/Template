import { Routes } from '@angular/router';
import { PageLogin } from './features/page-login/page-login';
import { PageDashboard } from './features/page-dashboard/page-dashboard';
import { Layout } from './layout/layout';
import { AuthGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  // -----------------------------
  // Public routes
  // -----------------------------
  {
    path: 'login',
    component: PageLogin,
  },

  // -----------------------------
  // Protected app shell
  // -----------------------------
  {
    path: '',
    component: Layout,
    canActivate: [AuthGuard],
    runGuardsAndResolvers: 'always',
    children: [
      {
        path: 'dashboard',
        component: PageDashboard,
      },
    ],
  },

  // -----------------------------
  // Fallback
  // -----------------------------
  {
    path: '**',
    redirectTo: 'login',
  },
];
