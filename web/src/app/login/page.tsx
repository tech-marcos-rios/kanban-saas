import { LoginForm } from "@/components/auth/LoginForm";

export default function LoginPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center px-4">
      <h1 className="mb-8 text-2xl font-bold text-gray-900">Kanban SaaS</h1>
      <LoginForm />
    </div>
  );
}
