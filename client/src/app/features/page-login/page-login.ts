import { Component, signal } from '@angular/core';
import { themes } from '../../layout/theme';
import { Button } from '../../shared/components/button/button';

@Component({
  selector: 'app-page-login',
  imports: [Button],
  templateUrl: './page-login.html',
  styleUrl: './page-login.css',
})
export class PageLogin {
  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'corporate');
  protected themes = themes;

  ngOnInit(): void {
    document.documentElement.setAttribute('data-theme', this.selectedTheme());
  }

  handleSelectTheme(theme: string) {
    this.selectedTheme.set(theme);
    localStorage.setItem('theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
    const elem = document.activeElement as HTMLDivElement;
    if (elem) elem.blur();
  }
}
