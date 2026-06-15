/** Первое слово имени для приветствий */
export function firstName(fullName: string | null | undefined): string {
  const trimmed = fullName?.trim();
  if (!trimmed) return '';
  return trimmed.split(/\s+/)[0] ?? trimmed;
}

export function greetingLine(fullName: string | null | undefined, fallback = 'друг'): string {
  const name = firstName(fullName);
  return name ? name : fallback;
}
