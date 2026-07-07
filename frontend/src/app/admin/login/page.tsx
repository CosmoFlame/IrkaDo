"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { adminApi, setToken } from "@/lib/adminApi";
import { Button, Card, ErrorText, Field, TextInput } from "@/components/admin/ui";

export default function AdminLoginPage() {
  const router = useRouter();
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const res = await adminApi.login(password);
      setToken(res.token);
      router.replace("/admin");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Sign in failed.");
      setLoading(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-zinc-50 px-6">
      <Card className="w-full max-w-sm">
        <h1 className="text-xl font-semibold tracking-tight text-zinc-900">Admin sign in</h1>
        <p className="mt-1 text-sm text-zinc-500">Enter the admin password to manage site content.</p>
        <form onSubmit={handleSubmit} className="mt-6 space-y-4">
          <Field label="Password">
            <TextInput
              type="password"
              autoFocus
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </Field>
          <ErrorText>{error}</ErrorText>
          <Button type="submit" disabled={loading} className="w-full">
            {loading ? "Signing in…" : "Sign in"}
          </Button>
        </form>
      </Card>
    </div>
  );
}
