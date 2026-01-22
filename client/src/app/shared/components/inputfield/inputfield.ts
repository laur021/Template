import { ChangeDetectionStrategy, Component, computed, forwardRef, input, output, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

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
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => Inputfield),
      multi: true,
    },
  ],
})
export class Inputfield implements ControlValueAccessor {
  readonly value = input<string>('');
  readonly type = input<InputType>('text');
  readonly placeholder = input<string>('');

  readonly variant = input<InputVariant | null>(null);
  readonly size = input<InputSize>('md');
  readonly style = input<InputStyle>('bordered');

  readonly disabled = input(false);
  protected readonly cvaDisabled = signal(false);
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

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string): void {
    this._value = value ?? '';
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  protected readonly isDisabled = computed(() => this.disabled() || this.cvaDisabled());

  setDisabledState(isDisabled: boolean): void {
    this.cvaDisabled.set(isDisabled);
  }

  /* internal value */
  protected _value = '';

  protected handleInput(event: Event) {
    const target = event.target as HTMLInputElement;
    this._value = target.value;
    this.onChange(target.value);
  }

  protected handleBlur() {
    this.onTouched();
  }
}
