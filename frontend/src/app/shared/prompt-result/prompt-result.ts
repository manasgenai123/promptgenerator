import { Component, computed, inject, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OptimizePromptResponse } from '../../core/models/prompt.models';

@Component({
  selector: 'app-prompt-result',
  standalone: true,
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressBarModule,
    MatTooltipModule,
  ],
  templateUrl: './prompt-result.html',
  styleUrl: './prompt-result.scss',
})
export class PromptResult {
  readonly result = input.required<OptimizePromptResponse>();

  private readonly snackBar = inject(MatSnackBar);

  protected readonly tokensSaved = computed(() => {
    const r = this.result();
    return Math.max(0, r.originalTokenEstimate - r.optimizedTokenEstimate);
  });

  protected readonly scoreColor = computed(() => {
    const score = this.result().qualityScore;
    if (score >= 75) return 'primary';
    if (score >= 50) return 'accent';
    return 'warn';
  });

  async copy(text: string): Promise<void> {
    try {
      await navigator.clipboard.writeText(text);
      this.snackBar.open('Copied to clipboard', 'Dismiss', { duration: 2000 });
    } catch {
      this.snackBar.open('Unable to copy', 'Dismiss', { duration: 2000 });
    }
  }
}
