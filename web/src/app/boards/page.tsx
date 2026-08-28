"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { X } from "lucide-react";
import { useBoards } from "@/hooks/use-boards";
import { BoardCard } from "@/components/boards/BoardCard";
import { CreateBoardForm } from "@/components/boards/CreateBoardForm";

const NOTICE_MESSAGES: Record<string, string> = {
  "no-access": "No tenés acceso a ese tablero, o ya no sos miembro.",
  removed: "Te sacaron de ese tablero.",
};

export default function BoardsPage() {
  const { data: boards, isLoading, isError } = useBoards();
  const router = useRouter();
  const searchParams = useSearchParams();
  // Se deriva directo de la URL en vez de copiarlo a un useState: así no hace falta un
  // efecto para "leerlo una vez" (el aviso desaparece solo con dismiss, que limpia el ?notice).
  const notice = NOTICE_MESSAGES[searchParams.get("notice") ?? ""];
  const dismiss = () => router.replace("/boards");

  return (
    <div className="mx-auto max-w-6xl px-4 py-8">
      {notice && (
        <div className="mb-4 flex items-center justify-between rounded-md bg-amber-50 px-4 py-2 text-sm text-amber-800">
          {notice}
          <button onClick={dismiss} className="rounded-md p-1 hover:bg-amber-100">
            <X className="h-4 w-4" />
          </button>
        </div>
      )}

      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-xl font-bold text-gray-900">Mis tableros</h1>
        <CreateBoardForm />
      </div>

      {isLoading && <p className="text-sm text-gray-500">Cargando tableros...</p>}
      {isError && <p className="text-sm text-red-600">No se pudieron cargar los tableros.</p>}

      {boards && boards.length === 0 && (
        <p className="text-sm text-gray-500">Todavía no tenés tableros. Creá el primero.</p>
      )}

      {boards && boards.length > 0 && (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {boards.map((board) => (
            <BoardCard key={board.id} board={board} />
          ))}
        </div>
      )}
    </div>
  );
}
