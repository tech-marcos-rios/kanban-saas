"use client";

import { useState, type FormEvent } from "react";
import { Plus } from "lucide-react";
import { useCreateList } from "@/hooks/use-lists";

export function CreateListForm({ boardId }: { boardId: string }) {
  const [title, setTitle] = useState("");
  const [isOpen, setIsOpen] = useState(false);
  const createList = useCreateList(boardId);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!title.trim()) return;
    await createList.mutateAsync(title.trim());
    setTitle("");
    setIsOpen(false);
  };

  if (!isOpen) {
    return (
      <button
        onClick={() => setIsOpen(true)}
        className="flex w-72 shrink-0 items-center gap-2 rounded-lg bg-gray-100/60 p-3 text-sm text-gray-500 hover:bg-gray-100"
      >
        <Plus className="h-4 w-4" />
        Agregar lista
      </button>
    );
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="w-72 shrink-0 space-y-1.5 rounded-lg bg-gray-100 p-3"
    >
      <input
        autoFocus
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        placeholder="Título de la lista"
        className="w-full rounded-md border border-gray-300 px-2 py-1.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
      />
      <div className="flex gap-1.5">
        <button
          type="submit"
          disabled={createList.isPending}
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
