import { Injectable, signal } from '@angular/core';
import { HistoryItem, OptimizePromptResponse } from '../models/prompt.models';

const STORAGE_KEY = 'prompt-optimizer-history';
const MAX_ITEMS = 50;

@Injectable({ providedIn: 'root' })
export class HistoryService {
  readonly items = signal<HistoryItem[]>(this.load());

  add(response: OptimizePromptResponse): void {
    const item: HistoryItem = {
      ...response,
      id: crypto.randomUUID(),
      createdAt: new Date().toISOString(),
    };
    const next = [item, ...this.items()].slice(0, MAX_ITEMS);
    this.items.set(next);
    this.persist(next);
  }

  remove(id: string): void {
    const next = this.items().filter((i) => i.id !== id);
    this.items.set(next);
    this.persist(next);
  }

  clear(): void {
    this.items.set([]);
    this.persist([]);
  }

  private load(): HistoryItem[] {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return raw ? (JSON.parse(raw) as HistoryItem[]) : [];
    } catch {
      return [];
    }
  }

  private persist(items: HistoryItem[]): void {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
    } catch {
      // Ignore storage quota / unavailable errors.
    }
  }
}
