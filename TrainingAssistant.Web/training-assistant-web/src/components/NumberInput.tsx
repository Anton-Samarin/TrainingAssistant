import { FocusEvent, InputHTMLAttributes, useEffect, useRef, useState } from 'react';
import { TextInput } from './Field';

interface NumberInputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'value' | 'onChange' | 'type'> {
  value: number;
  onChange: (value: number) => void;
  integer?: boolean;
}

function formatValue(n: number, integer?: boolean) {
  return integer ? String(Math.round(n)) : String(n);
}

function parseValue(raw: string, integer?: boolean): number | null {
  if (raw === '' || raw === '-' || raw === '.') return null;
  const n = integer ? parseInt(raw, 10) : parseFloat(raw);
  return Number.isFinite(n) ? n : null;
}

function isValidPartial(raw: string, integer?: boolean) {
  return integer ? /^\d*$/.test(raw) : /^\d*\.?\d*$/.test(raw);
}

export function NumberInput({
  value,
  onChange,
  integer,
  min,
  max,
  onBlur,
  onFocus,
  ...rest
}: NumberInputProps) {
  const [text, setText] = useState(() => formatValue(value, integer));
  const focusedRef = useRef(false);

  useEffect(() => {
    if (!focusedRef.current) {
      setText(formatValue(value, integer));
    }
  }, [value, integer]);

  function clamp(n: number) {
    let result = n;
    if (min !== undefined) result = Math.max(result, Number(min));
    if (max !== undefined) result = Math.min(result, Number(max));
    return result;
  }

  function handleChange(raw: string) {
    if (!isValidPartial(raw, integer)) return;
    setText(raw);
    const n = parseValue(raw, integer);
    if (n !== null) onChange(n);
  }

  function handleBlur(e: FocusEvent<HTMLInputElement>) {
    focusedRef.current = false;
    const n = parseValue(text, integer);
    if (n === null) {
      setText(formatValue(value, integer));
    } else {
      const clamped = clamp(n);
      onChange(clamped);
      setText(formatValue(clamped, integer));
    }
    onBlur?.(e);
  }

  return (
    <TextInput
      type="text"
      inputMode={integer ? 'numeric' : 'decimal'}
      autoComplete="off"
      value={text}
      onChange={(e) => handleChange(e.target.value)}
      onFocus={(e) => {
        focusedRef.current = true;
        onFocus?.(e);
      }}
      onBlur={handleBlur}
      {...rest}
    />
  );
}
