import { Component, inject, signal } from '@angular/core';
import { KeyValuePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PromptService } from '../../core/services/prompt.service';
import { DashboardStats } from '../../core/models/prompt.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [KeyValuePipe, MatCardModule, MatIconModule, MatButtonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private readonly promptService = inject(PromptService);

  protected readonly stats = signal<DashboardStats | null>(null);
  protected readonly loading = signal(false);

  constructor() {
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);
    this.promptService.getStats().subscribe({
      next: (stats) => {
        this.stats.set(stats);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
