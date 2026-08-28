import { apiFetch } from "../api-client";
import type { BoardResponse } from "../types";

export function getMyBoards() {
  return apiFetch<BoardResponse[]>("/api/v1/boards");
}

export function getBoard(boardId: string) {
  return apiFetch<BoardResponse>(`/api/v1/boards/${boardId}`);
}

export function createBoard(name: string) {
  return apiFetch<BoardResponse>("/api/v1/boards", { method: "POST", body: { name } });
}

export function renameBoard(boardId: string, name: string) {
  return apiFetch<BoardResponse>(`/api/v1/boards/${boardId}`, { method: "PUT", body: { name } });
}

export function deleteBoard(boardId: string) {
  return apiFetch<void>(`/api/v1/boards/${boardId}`, { method: "DELETE" });
}
