"use client";

import { useState, type FormEvent } from "react";
import { Plus } from "lucide-react";
import { useCreateCard } from "@/hooks/use-cards";

export function CreateCardForm({ boardId, listId }: { boardId: string; listId: string }) {
  const [title, setTitle] = useState("");
  const [isOpen, setIsOpen] = useState(false);
  const createCard = useCreateCard(boardId, listId);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!title.trim()) return;
    await createCard.mutateAsync(title.trim());
    setTitle("");
    setIsOpen(false);
  };

  if (!isOpen) {
    return (
      <button
        onClick={() => setIsOpen(true)}
        className="flex w-full items-center gap-1 rounded-md px-2 py-1.5 text-left text-sm text-gray-500 hover:bg-gray-200"
      >
        <Plus className="h-4 w-4" />
        Agregar tarjeta
      </button>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-1.5">
      <textarea
        autoFocus
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        placeholder="Título de la tarjeta"
        rows={2}
        className="w-full resize-none rounded-md border border-gray-300 px-2 py-1.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
      />
      <div className="flex gap-1.5">
        <button
          type="submit"
          disabled={createCard.isPending}
          className="rounded-md bg-blue-600 px-3 py-1 text-xs font-medium text-white hover:bg-blue-700 disabled:opacity-50"
        >
          Agregar
        </button>
        <button
          type="button"
          onClick={() => setIsOpen(false)}
          className="rounded-md px-3 py-1 text-xs text-gray-500 hover:bg-gray-200"
        >
          Cancelar
        </button>
      </div>
    </form>
  );
}
