import { Component, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import {
  OPTIMIZATION_OPTIONS,
  OptimizationType,
  OptimizePromptRequest,
} from '../../core/models/prompt.models';

@Component({
  selector: 'app-prompt-input',
  standalone: true,
  imports: [
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './prompt-input.html',
  styleUrl: './prompt-input.scss',
})
export class PromptInput {
  readonly loading = input(false);
  readonly optimize = output<OptimizePromptRequest>();

  protected readonly options = OPTIMIZATION_OPTIONS;
  protected readonly maxLength = 20000;

  protected readonly prompt = signal('');
  protected readonly optimizationType = signal<OptimizationType>('General');

  setPrompt(value: string): void {
    this.prompt.set(value);
  }

  setType(value: OptimizationType): void {
    this.optimizationType.set(value);
  }

  submit(): void {
    const prompt = this.prompt().trim();
    if (prompt.length < 5 || this.loading()) {
      return;
    }
    this.optimize.emit({ prompt, optimizationType: this.optimizationType() });
  }

  clear(): void {
    this.prompt.set('');
  }
}
