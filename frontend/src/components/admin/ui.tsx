"use client";

import Link from "next/link";
import type {
  ButtonHTMLAttributes,
  InputHTMLAttributes,
  ReactNode,
  SelectHTMLAttributes,
  TextareaHTMLAttributes,
} from "react";

export function cx(...parts: Array<string | false | null | undefined>): string {
  return parts.filter(Boolean).join(" ");
}

const buttonBase =
  "inline-flex items-center justify-center rounded-full px-5 py-2 text-sm font-semibold transition disabled:opacity-60 disabled:cursor-not-allowed";

const buttonVariants = {
  primary: "bg-zinc-900 text-white hover:bg-zinc-700",
  secondary: "border border-zinc-300 bg-white text-zinc-700 hover:bg-zinc-100",
  danger: "border border-red-200 bg-white text-red-600 hover:bg-red-50",
} as const;

type ButtonProps = {
  variant?: keyof typeof buttonVariants;
  children: ReactNode;
} & ButtonHTMLAttributes<HTMLButtonElement>;

export function Button({ variant = "primary", className, children, ...rest }: ButtonProps) {
  return (
    <button {...rest} className={cx(buttonBase, buttonVariants[variant], className)}>
      {children}
    </button>
  );
}

export function LinkButton({
  href,
  variant = "primary",
  children,
}: {
  href: string;
  variant?: keyof typeof buttonVariants;
  children: ReactNode;
}) {
  return (
    <Link href={href} className={cx(buttonBase, buttonVariants[variant])}>
      {children}
    </Link>
  );
}

export function PageHeader({ title, description, actions }: { title: string; description?: string; actions?: ReactNode }) {
  return (
    <div className="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight text-zinc-900">{title}</h1>
        {description && <p className="mt-1 text-sm text-zinc-500">{description}</p>}
      </div>
      {actions && <div className="flex gap-3">{actions}</div>}
    </div>
  );
}

export function Field({ label, hint, children }: { label: string; hint?: string; children: ReactNode }) {
  return (
    <label className="block">
      <span className="mb-1.5 block text-sm font-medium text-zinc-700">{label}</span>
      {children}
      {hint && <span className="mt-1 block text-xs text-zinc-400">{hint}</span>}
    </label>
  );
}

const inputClass =
  "w-full rounded-lg border border-zinc-300 bg-white px-3 py-2 text-sm text-zinc-900 outline-none focus:border-zinc-500";

export function TextInput(props: InputHTMLAttributes<HTMLInputElement>) {
  return <input {...props} className={cx(inputClass, props.className)} />;
}

export function TextArea(props: TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return <textarea {...props} className={cx(inputClass, "min-h-28", props.className)} />;
}

export function Select(props: SelectHTMLAttributes<HTMLSelectElement>) {
  return <select {...props} className={cx(inputClass, props.className)} />;
}

export function Checkbox({ label, checked, onChange }: { label: string; checked: boolean; onChange: (v: boolean) => void }) {
  return (
    <label className="flex items-center gap-2 text-sm font-medium text-zinc-700">
      <input
        type="checkbox"
        checked={checked}
        onChange={(e) => onChange(e.target.checked)}
        className="h-4 w-4 rounded border-zinc-300"
      />
      {label}
    </label>
  );
}

export function Card({ children, className }: { children: ReactNode; className?: string }) {
  return <div className={cx("rounded-2xl border border-zinc-200 bg-white p-6 shadow-sm", className)}>{children}</div>;
}

export function ErrorText({ children }: { children: ReactNode }) {
  if (!children) return null;
  return <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">{children}</p>;
}

export function SuccessText({ children }: { children: ReactNode }) {
  if (!children) return null;
  return <p className="rounded-lg bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{children}</p>;
}

export function StatusPill({ published }: { published: boolean }) {
  return (
    <span
      className={cx(
        "inline-flex rounded-full px-2.5 py-0.5 text-xs font-medium",
        published ? "bg-emerald-100 text-emerald-800" : "bg-zinc-100 text-zinc-500",
      )}
    >
      {published ? "Published" : "Draft"}
    </span>
  );
}

export function EmptyState({ children }: { children: ReactNode }) {
  return <div className="rounded-2xl border border-dashed border-zinc-300 p-10 text-center text-sm text-zinc-500">{children}</div>;
}
