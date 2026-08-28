import { RequireAuth } from "@/components/auth/RequireAuth";
import { AppHeader } from "@/components/layout/AppHeader";

export default function BoardsLayout({ children }: { children: React.ReactNode }) {
  return (
    <RequireAuth>
      <AppHeader />
      <main className="flex-1">{children}</main>
    </RequireAuth>
  );
}
