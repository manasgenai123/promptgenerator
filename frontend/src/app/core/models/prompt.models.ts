export type OptimizationType =
  | 'General'
  | 'Technical'
  | 'Business'
  | 'CodeGeneration'
  | 'UserStory'
  | 'TestCase';

export interface OptimizationOption {
  value: OptimizationType;
  label: string;
}

export const OPTIMIZATION_OPTIONS: OptimizationOption[] = [
  { value: 'General', label: 'General Optimization' },
  { value: 'Technical', label: 'Technical Prompt' },
  { value: 'Business', label: 'Business Prompt' },
  { value: 'CodeGeneration', label: 'Code Generation Prompt' },
  { value: 'UserStory', label: 'User Story Prompt' },
  { value: 'TestCase', label: 'Test Case Prompt' },
];

export interface OptimizePromptRequest {
  prompt: string;
  optimizationType: OptimizationType;
}

export interface OptimizePromptResponse {
  originalPrompt: string;
  optimizedPrompt: string;
  improvements: string[];
  qualityScore: number;
  originalTokenEstimate: number;
  optimizedTokenEstimate: number;
  promptTokens: number;
  completionTokens: number;
  totalTokens: number;
  tokenUsageSource: string;
  optimizationType: string;
}

export interface HistoryItem extends OptimizePromptResponse {
  id: string;
  createdAt: string;
}

export interface RecentOptimization {
  id: string;
  optimizationType: string;
  qualityScore: number;
  createdAtUtc: string;
}

export interface DashboardStats {
  totalOptimizations: number;
  averageQualityScore: number;
  totalTokensSaved: number;
  optimizationsByType: Record<string, number>;
  recent: RecentOptimization[];
}
