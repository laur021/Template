import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthStateService } from './core/services/auth-state-service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('client');
  protected readonly authState = inject(AuthStateService);

  ngOnInit(): void {
    // Try to restore session at app startup (uses refresh cookie)
    this.authState.tryRestoreSession();
  }
}
