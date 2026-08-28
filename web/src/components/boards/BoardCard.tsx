import Link from "next/link";
import { Users } from "lucide-react";
import type { BoardResponse } from "@/lib/types";

export function BoardCard({ board }: { board: BoardResponse }) {
  return (
    <Link
      href={`/boards/${board.id}`}
      className="block rounded-lg border border-gray-200 bg-white p-4 shadow-sm transition hover:border-blue-300 hover:shadow-md"
    >
      <h3 className="font-semibold text-gray-900">{board.name}</h3>
      <p className="mt-1 flex items-center gap-1 text-xs text-gray-500">
        <Users className="h-3.5 w-3.5" />
        {board.role === "Owner" ? "Sos el dueño" : `${board.role} · dueño: ${board.ownerName}`}
      </p>
    </Link>
  );
}
