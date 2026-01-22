import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

type InputVariant =
  | 'primary'
  | 'secondary'
  | 'accent'
  | 'info'
  | 'success'
  | 'warning'
  | 'error'
  | 'neutral';

type InputSize = 'xs' | 'sm' | 'md' | 'lg';

type InputStyle = 'bordered' | 'ghost' | 'default';

type InputType = 'text' | 'email' | 'password' | 'number' | 'search' | 'tel' | 'url';

@Component({
  selector: 'app-inputfield',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './inputfield.html',
})
export class Inputfield {
  readonly value = input<string>('');
  readonly type = input<InputType>('text');
  readonly placeholder = input<string>('');

  readonly variant = input<InputVariant | null>(null);
  readonly size = input<InputSize>('md');
  readonly style = input<InputStyle>('bordered');

  readonly disabled = input(false);
  readonly readonly = input(false);
  readonly required = input(false);
  readonly fullWidth = input(false);
  readonly invalid = input(false);

  readonly name = input<string | null>(null);
  readonly autocomplete = input<string | null>(null);

  readonly valueChange = output<string>();

  private readonly variantClassMap: Record<InputVariant, string> = {
    primary: 'input-primary',
    secondary: 'input-secondary',
    accent: 'input-accent',
    info: 'input-info',
    success: 'input-success',
    warning: 'input-warning',
    error: 'input-error',
    neutral: 'input-neutral',
  };

  private readonly sizeClassMap: Record<InputSize, string> = {
    xs: 'input-xs',
    sm: 'input-sm',
    md: 'input-md',
    lg: 'input-lg',
  };

  private readonly styleClassMap: Record<InputStyle, string> = {
    bordered: 'input-bordered',
    ghost: 'input-ghost',
    default: '',
  };

  protected readonly classes = computed(() =>
    [
      'input',
      this.variant() ? this.variantClassMap[this.variant()!] : '',
      this.sizeClassMap[this.size()],
      this.styleClassMap[this.style()],
      this.fullWidth() ? 'w-full' : '',
    ]
      .filter(Boolean)
      .join(' '),
  );

  protected handleInput(event: Event) {
    const target = event.target as HTMLInputElement;
    this.valueChange.emit(target.value);
  }
}
