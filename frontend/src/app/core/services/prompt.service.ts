import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  DashboardStats,
  OptimizePromptRequest,
  OptimizePromptResponse,
} from '../models/prompt.models';

@Injectable({ providedIn: 'root' })
export class PromptService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  optimize(request: OptimizePromptRequest): Observable<OptimizePromptResponse> {
    return this.http.post<OptimizePromptResponse>(
      `${this.baseUrl}/prompt/optimize`,
      request,
    );
  }

  getStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.baseUrl}/dashboard/stats`);
  }
}
