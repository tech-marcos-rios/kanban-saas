import { apiFetch } from "../api-client";
import type { CardResponse } from "../types";

export function getCardsByList(boardId: string, listId: string) {
  return apiFetch<CardResponse[]>(`/api/v1/boards/${boardId}/cards/lists/${listId}`);
}

export function createCard(boardId: string, listId: string, title: string, description?: string) {
  return apiFetch<CardResponse>(`/api/v1/boards/${boardId}/cards/lists/${listId}`, {
    method: "POST",
    body: { title, description: description ?? null, dueDate: null },
  });
}

export function updateCard(
  boardId: string,
  cardId: string,
  data: { title: string; description: string | null; dueDate: string | null },
) {
  return apiFetch<CardResponse>(`/api/v1/boards/${boardId}/cards/${cardId}`, {
    method: "PUT",
    body: data,
  });
}

export function assignCard(boardId: string, cardId: string, userId: string | null) {
  return apiFetch<CardResponse>(`/api/v1/boards/${boardId}/cards/${cardId}/assign`, {
    method: "PUT",
    body: { userId },
  });
}

export function moveCard(boardId: string, cardId: string, listId: string, position: number) {
  return apiFetch<CardResponse>(`/api/v1/boards/${boardId}/cards/${cardId}/move`, {
    method: "PUT",
    body: { listId, position },
  });
}

export function deleteCard(boardId: string, cardId: string) {
  return apiFetch<void>(`/api/v1/boards/${boardId}/cards/${cardId}`, { method: "DELETE" });
}
