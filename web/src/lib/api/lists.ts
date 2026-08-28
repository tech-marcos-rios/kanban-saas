import { apiFetch } from "../api-client";
import type { BoardListResponse } from "../types";

export function getLists(boardId: string) {
  return apiFetch<BoardListResponse[]>(`/api/v1/boards/${boardId}/lists`);
}

export function createList(boardId: string, title: string) {
  return apiFetch<BoardListResponse>(`/api/v1/boards/${boardId}/lists`, {
    method: "POST",
    body: { title },
  });
}

export function renameList(boardId: string, listId: string, title: string) {
  return apiFetch<BoardListResponse>(`/api/v1/boards/${boardId}/lists/${listId}`, {
    method: "PUT",
    body: { title },
  });
}

export function moveList(boardId: string, listId: string, position: number) {
  return apiFetch<BoardListResponse[]>(`/api/v1/boards/${boardId}/lists/${listId}/position`, {
    method: "PUT",
    body: { position },
  });
}

export function deleteList(boardId: string, listId: string) {
  return apiFetch<void>(`/api/v1/boards/${boardId}/lists/${listId}`, { method: "DELETE" });
}
