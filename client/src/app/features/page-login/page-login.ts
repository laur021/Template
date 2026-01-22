import { Component, signal } from '@angular/core';
import { themes } from '../../layout/theme';
import { Button } from '../../shared/components/button/button';
import { Inputfield } from '../../shared/components/inputfield/inputfield';

@Component({
  selector: 'app-page-login',
  imports: [Button, Inputfield],
  templateUrl: './page-login.html',
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
