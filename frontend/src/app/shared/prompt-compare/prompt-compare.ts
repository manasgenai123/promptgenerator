import { Component, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { countWords, estimateTokens } from '../../core/utils/token.util';

interface PromptStats {
  tokens: number;
  words: number;
  chars: number;
}

interface GuidanceItem {
  icon: string;
  tone: 'positive' | 'warning' | 'info';
  message: string;
}

@Component({
  selector: 'app-prompt-compare',
  standalone: true,
  imports: [
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
  ],
  templateUrl: './prompt-compare.html',
  styleUrl: './prompt-compare.scss',
})
export class PromptCompare {
  protected readonly maxLength = 20000;

  protected readonly original = signal('');
  protected readonly optimized = signal('');
  protected readonly compared = signal(false);

  protected readonly originalStats = computed<PromptStats>(() => this.statsFor(this.original()));
  protected readonly optimizedStats = computed<PromptStats>(() => this.statsFor(this.optimized()));

  protected readonly tokenDelta = computed(
    () => this.optimizedStats().tokens - this.originalStats().tokens,
  );

  protected readonly tokensSaved = computed(() => Math.max(0, -this.tokenDelta()));

  protected readonly savedPercent = computed(() => {
    const original = this.originalStats().tokens;
    if (original <= 0) return 0;
    return Math.round((this.tokenDelta() / original) * -100);
  });

  /** Positive when the optimized prompt is leaner, negative when it grew. */
  protected readonly isLeaner = computed(() => this.tokenDelta() < 0);

  protected readonly canCompare = computed(
    () => this.original().trim().length > 0 && this.optimized().trim().length > 0,
  );

  protected readonly guidance = computed<GuidanceItem[]>(() => this.buildGuidance());

  setOriginal(value: string): void {
    this.original.set(value);
    this.compared.set(false);
  }

  setOptimized(value: string): void {
    this.optimized.set(value);
    this.compared.set(false);
  }

  compare(): void {
    if (this.canCompare()) {
      this.compared.set(true);
    }
  }

  clear(): void {
    this.original.set('');
    this.optimized.set('');
    this.compared.set(false);
  }

  private statsFor(text: string): PromptStats {
    return {
      tokens: estimateTokens(text),
      words: countWords(text),
      chars: text.length,
    };
  }

  private buildGuidance(): GuidanceItem[] {
    const items: GuidanceItem[] = [];
    const delta = this.tokenDelta();
    const percent = this.savedPercent();
    const optimizedText = this.optimized();
    const originalText = this.original();

    if (delta < 0) {
      items.push({
        icon: 'savings',
        tone: 'positive',
        message: `The optimized prompt is leaner by ${this.tokensSaved()} tokens (${percent}% fewer). You save these tokens on every future call.`,
      });
    } else if (delta > 0) {
      items.push({
        icon: 'trending_up',
        tone: 'warning',
        message: `The optimized prompt uses ${delta} more tokens. Make sure the added structure or detail improves the model's output enough to justify the extra cost.`,
      });
    } else {
      items.push({
        icon: 'balance',
        tone: 'info',
        message: 'Both prompts use roughly the same number of tokens. Focus on clarity and structure rather than length.',
      });
    }

    // Politeness / filler phrases that rarely help the model.
    const fillers = this.findFillers(optimizedText);
    if (fillers.length > 0) {
      items.push({
        icon: 'cleaning_services',
        tone: 'warning',
        message: `Remove filler phrases that add tokens without meaning: ${fillers.join(', ')}.`,
      });
    }

    // Structure signals improve output quality and are worth the tokens.
    const hasStructure = /(^|\n)\s*([-*]|\d+\.)\s/m.test(optimizedText);
    if (!hasStructure && this.optimizedStats().words > 30) {
      items.push({
        icon: 'format_list_bulleted',
        tone: 'info',
        message: 'Consider breaking long instructions into bullet points or numbered steps — clearer structure usually improves results.',
      });
    }

    // Explicit output-format directive.
    const hasFormat = /\b(json|markdown|table|format|step[- ]by[- ]step|output)\b/i.test(optimizedText);
    if (!hasFormat) {
      items.push({
        icon: 'data_object',
        tone: 'info',
        message: 'Specify the desired output format (e.g. JSON, markdown, a table) so the model does not waste tokens guessing.',
      });
    }

    // Redundant repetition.
    if (this.hasHeavyRepetition(optimizedText)) {
      items.push({
        icon: 'content_copy',
        tone: 'warning',
        message: 'Repeated words or phrases detected — consolidate duplicated instructions to cut tokens.',
      });
    }

    // Very long prompt warning.
    if (this.optimizedStats().tokens > 800) {
      items.push({
        icon: 'warning',
        tone: 'warning',
        message: 'This prompt is quite large. Trim background context the model does not need to lower cost and latency.',
      });
    }

    // Overly terse prompt.
    if (this.optimizedStats().words < 8 && originalText.trim().length > 0) {
      items.push({
        icon: 'info',
        tone: 'info',
        message: 'The optimized prompt is very short. Ensure it still contains enough context for a reliable answer.',
      });
    }

    return items;
  }

  private findFillers(text: string): string[] {
    const patterns: Array<{ label: string; regex: RegExp }> = [
      { label: '"please"', regex: /\bplease\b/i },
      { label: '"kindly"', regex: /\bkindly\b/i },
      { label: '"I want you to"', regex: /\bi want you to\b/i },
      { label: '"I would like you to"', regex: /\bi would like you to\b/i },
      { label: '"as an AI"', regex: /\bas an ai\b/i },
      { label: '"could you"', regex: /\bcould you\b/i },
      { label: '"basically"', regex: /\bbasically\b/i },
      { label: '"just"', regex: /\bjust\b/i },
      { label: '"very"', regex: /\bvery\b/i },
    ];
    return patterns.filter((p) => p.regex.test(text)).map((p) => p.label);
  }

  private hasHeavyRepetition(text: string): boolean {
    const words = text
      .toLowerCase()
      .match(/[a-z]{4,}/g);
    if (!words || words.length < 12) {
      return false;
    }
    const counts = new Map<string, number>();
    for (const word of words) {
      counts.set(word, (counts.get(word) ?? 0) + 1);
    }
    for (const count of counts.values()) {
      if (count >= 5) {
        return true;
      }
    }
    return false;
  }
}
