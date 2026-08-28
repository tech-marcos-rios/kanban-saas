import { RegisterForm } from "@/components/auth/RegisterForm";

export default function RegisterPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center px-4">
      <h1 className="mb-8 text-2xl font-bold text-gray-900">Kanban SaaS</h1>
      <RegisterForm />
    </div>
  );
}
