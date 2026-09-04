"use client";

import { useState, type FormEvent } from "react";
import { UserPlus, X } from "lucide-react";
import { Modal } from "@/components/ui/Modal";
import { useInviteMember, useMembers, useRemoveMember } from "@/hooks/use-members";
import { ApiClientError } from "@/lib/api-client";
import type { BoardRole } from "@/lib/types";

interface MembersModalProps {
  boardId: string;
  isOpen: boolean;
  onClose: () => void;
  currentRole: BoardRole | undefined;
}

export function MembersModal({ boardId, isOpen, onClose, currentRole }: MembersModalProps) {
  const { data: members, isLoading } = useMembers(boardId);
  const inviteMember = useInviteMember(boardId);
  const removeMember = useRemoveMember(boardId);

  const [email, setEmail] = useState("");
  const [role, setRole] = useState<"Editor" | "Viewer">("Editor");
  const [error, setError] = useState<string | null>(null);

  const isOwner = currentRole === "Owner";

  const handleInvite = async (e: FormEvent) => {
    e.preventDefault();
    if (!email.trim()) return;
    setError(null);
    try {
      await inviteMember.mutateAsync({ email: email.trim(), role });
      setEmail("");
    } catch (err) {
      setError(err instanceof ApiClientError ? err.message : "No se pudo invitar al miembro.");
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Miembros del tablero">
      <div className="space-y-3">
        {isLoading && <p className="text-sm text-gray-500">Cargando miembros...</p>}

        <ul className="max-h-56 space-y-1.5 overflow-y-auto">
          {members?.map((member) => (
            <li
              key={member.userId}
              className="flex items-center justify-between rounded-md border border-gray-200 px-2.5 py-1.5"
            >
              <div className="min-w-0">
                <p className="truncate text-sm font-medium text-gray-900">{member.name}</p>
                <p className="truncate text-xs text-gray-500">{member.email}</p>
              </div>
              <div className="flex shrink-0 items-center gap-2">
                <span className="rounded-full bg-gray-100 px-2 py-0.5 text-xs text-gray-600">
                  {member.role}
                </span>
                {isOwner && member.role !== "Owner" && (
                  <button
                    onClick={() => removeMember.mutate(member.userId)}
                    disabled={removeMember.isPending}
                    className="rounded p-1 text-gray-400 hover:bg-red-50 hover:text-red-600 disabled:opacity-50"
                    aria-label={`Quitar a ${member.name}`}
                  >
                    <X className="h-3.5 w-3.5" />
                  </button>
                )}
              </div>
            </li>
          ))}
        </ul>

        {isOwner && (
          <form onSubmit={handleInvite} className="space-y-1.5 border-t border-gray-100 pt-3">
            <div className="flex gap-1.5">
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Email del invitado"
                className="min-w-0 flex-1 rounded-md border border-gray-300 px-2 py-1.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              />
              <select
                value={role}
                onChange={(e) => setRole(e.target.value as "Editor" | "Viewer")}
                className="rounded-md border border-gray-300 px-2 py-1.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              >
                <option value="Editor">Editor</option>
                <option value="Viewer">Viewer</option>
              </select>
            </div>
            {error && <p className="text-xs text-red-600">{error}</p>}
            <button
              type="submit"
              disabled={inviteMember.isPending}
              className="flex items-center gap-1.5 rounded-md bg-blue-600 px-3 py-1.5 text-xs font-medium text-white hover:bg-blue-700 disabled:opacity-50"
            >
              <UserPlus className="h-3.5 w-3.5" />
              Invitar
            </button>
          </form>
        )}
      </div>
    </Modal>
  );
}
