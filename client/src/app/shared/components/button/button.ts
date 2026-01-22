import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

type ButtonVariant =
  | 'primary'
  | 'secondary'
  | 'accent'
  | 'ghost'
  | 'link'
  | 'neutral'
  | 'info'
  | 'success'
  | 'warning'
  | 'error';

type ButtonStyle = 'solid' | 'outline' | 'dash' | 'soft';
type ButtonSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl';
type ButtonModifier = 'wide' | 'square' | 'circle';
type ButtonType = 'button' | 'submit' | 'reset';
type IconPosition = 'start' | 'end';

@Component({
  selector: 'app-button',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './button.html',
})
export class Button {
  // inputs
  readonly variant = input<ButtonVariant>('primary');
  readonly style = input<ButtonStyle>('solid');
  readonly size = input<ButtonSize>('md');
  readonly type = input<ButtonType>('button');

  readonly disabled = input(false);
  readonly active = input(false);
  readonly fullWidth = input(false);
  readonly modifier = input<ButtonModifier | null>(null);
  readonly loading = input(false);

  readonly icon = input<string | null>(null);
  readonly iconPosition = input<IconPosition>('start');

  // output
  readonly clicked = output<void>();

  private readonly variantClassMap: Record<ButtonVariant, string> = {
    primary: 'btn-primary',
    secondary: 'btn-secondary',
    accent: 'btn-accent',
    neutral: 'btn-neutral',
    info: 'btn-info',
    success: 'btn-success',
    warning: 'btn-warning',
    error: 'btn-error',
    ghost: 'btn-ghost',
    link: 'btn-link',
  };

  private readonly styleClassMap: Record<ButtonStyle, string> = {
    solid: '',
    outline: 'btn-outline',
    dash: 'btn-dash',
    soft: 'btn-soft',
  };

  private readonly sizeClassMap: Record<ButtonSize, string> = {
    xs: 'btn-xs',
    sm: 'btn-sm',
    md: 'btn-md',
    lg: 'btn-lg',
    xl: 'btn-xl',
  };

  private readonly modifierClassMap: Record<ButtonModifier, string> = {
    wide: 'btn-wide',
    square: 'btn-square',
    circle: 'btn-circle',
  };

  protected readonly isDisabled = computed(() => this.disabled() || this.loading());

  protected readonly classes = computed(() =>
    [
      'btn',
      this.variantClassMap[this.variant()],
      this.styleClassMap[this.style()],
      this.sizeClassMap[this.size()],
      this.fullWidth() ? 'btn-block' : '',
      this.modifier() ? this.modifierClassMap[this.modifier()!] : '',
      this.active() ? 'btn-active' : '',
      this.isDisabled() ? 'btn-disabled' : '',
    ]
      .filter(Boolean)
      .join(' '),
  );
}
