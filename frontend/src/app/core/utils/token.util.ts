/**
 * Client-side token/word estimation mirroring the backend `PromptAnalyzer`
 * heuristic (~4 characters per token, with a word-count floor). These figures
 * are approximations intended for UX guidance, not billing.
 */

export function countWords(text: string): number {
  const matches = text.match(/\S+/g);
  return matches ? matches.length : 0;
}

export function estimateTokens(text: string): number {
  if (!text || !text.trim()) {
    return 0;
  }
  const charBased = Math.ceil(text.length / 4);
  const wordBased = Math.ceil(countWords(text) * 1.33);
  return Math.max(charBased, wordBased);
}
