import { Component, inject, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { HistoryService } from '../../core/services/history.service';
import { HistoryItem } from '../../core/models/prompt.models';

@Component({
  selector: 'app-prompt-history',
  standalone: true,
  imports: [
    DatePipe,
    MatCardModule,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
  ],
  templateUrl: './prompt-history.html',
  styleUrl: './prompt-history.scss',
})
export class PromptHistory {
  readonly select = output<HistoryItem>();

  protected readonly historyService = inject(HistoryService);

  preview(text: string): string {
    const trimmed = text.trim().replace(/\s+/g, ' ');
    return trimmed.length > 70 ? `${trimmed.slice(0, 70)}…` : trimmed;
  }
}
