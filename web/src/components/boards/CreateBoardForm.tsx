"use client";

import { useState, type FormEvent } from "react";
import { Plus } from "lucide-react";
import { useCreateBoard } from "@/hooks/use-boards";

export function CreateBoardForm() {
  const [name, setName] = useState("");
  const [isOpen, setIsOpen] = useState(false);
  const createBoard = useCreateBoard();

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;
    await createBoard.mutateAsync(name.trim());
    setName("");
    setIsOpen(false);
  };

  if (!isOpen) {
    return (
      <button
        onClick={() => setIsOpen(true)}
        className="flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
      >
        <Plus className="h-4 w-4" />
        Nuevo tablero
      </button>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="flex items-center gap-2">
      <input
        autoFocus
        value={name}
        onChange={(e) => setName(e.target.value)}
        placeholder="Nombre del tablero"
        className="rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
      />
      <button
        type="submit"
        disabled={createBoard.isPending}
        className="rounded-md bg-blue-600 px-3 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
      >
        Crear
      </button>
      <button
        type="button"
        onClick={() => setIsOpen(false)}
        className="rounded-md px-3 py-2 text-sm text-gray-500 hover:bg-gray-100"
      >
        Cancelar
      </button>
    </form>
  );
}
