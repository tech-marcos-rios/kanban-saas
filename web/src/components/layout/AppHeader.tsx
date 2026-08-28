"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { LogOut, LayoutGrid } from "lucide-react";
import { useAuth } from "@/providers/auth-provider";

export function AppHeader() {
  const { user, logout } = useAuth();
  const router = useRouter();

  const handleLogout = async () => {
    await logout();
    router.push("/login");
  };

  return (
    <header className="border-b border-gray-200 bg-white">
      <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
        <Link href="/boards" className="flex items-center gap-2 font-semibold text-gray-900">
          <LayoutGrid className="h-5 w-5 text-blue-600" />
          Kanban SaaS
        </Link>

        {user && (
          <div className="flex items-center gap-4 text-sm text-gray-600">
            <span>{user.name}</span>
            <button
              onClick={handleLogout}
              className="flex items-center gap-1 rounded-md px-2 py-1 hover:bg-gray-100"
            >
              <LogOut className="h-4 w-4" />
              Salir
            </button>
          </div>
        )}
      </div>
    </header>
  );
}
