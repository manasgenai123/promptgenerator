import { Component, inject, signal } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Navbar } from './shared/navbar/navbar';
import { PromptInput } from './shared/prompt-input/prompt-input';
import { PromptResult } from './shared/prompt-result/prompt-result';
import { PromptHistory } from './shared/prompt-history/prompt-history';
import { Dashboard } from './shared/dashboard/dashboard';
import { PromptService } from './core/services/prompt.service';
import { HistoryService } from './core/services/history.service';
import {
  HistoryItem,
  OptimizePromptRequest,
  OptimizePromptResponse,
} from './core/models/prompt.models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [Navbar, PromptInput, PromptResult, PromptHistory, Dashboard],
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  private readonly promptService = inject(PromptService);
  private readonly historyService = inject(HistoryService);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly loading = signal(false);
  protected readonly result = signal<OptimizePromptResponse | null>(null);

  optimize(request: OptimizePromptRequest): void {
    this.loading.set(true);
    this.promptService.optimize(request).subscribe({
      next: (response) => {
        this.result.set(response);
        this.historyService.add(response);
        this.loading.set(false);
      },
      error: (error) => {
        this.loading.set(false);
        const message =
          error?.error?.errors?.[0] ??
          error?.error?.title ??
          'Optimization failed. Please check the API and try again.';
        this.snackBar.open(message, 'Dismiss', { duration: 4000 });
      },
    });
  }

  showHistoryItem(item: HistoryItem): void {
    this.result.set(item);
  }
}

