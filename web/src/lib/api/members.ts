import { apiFetch } from "../api-client";
import type { BoardMemberResponse } from "../types";

export function getMembers(boardId: string) {
  return apiFetch<BoardMemberResponse[]>(`/api/v1/boards/${boardId}/members`);
}

export function inviteMember(boardId: string, email: string, role: "Editor" | "Viewer") {
  return apiFetch<BoardMemberResponse>(`/api/v1/boards/${boardId}/members`, {
    method: "POST",
    body: { email, role },
  });
}

export function removeMember(boardId: string, userId: string) {
  return apiFetch<void>(`/api/v1/boards/${boardId}/members/${userId}`, { method: "DELETE" });
}
