import { useQueries } from "@tanstack/react-query";
import * as cardsApi from "@/lib/api/cards";
import type { BoardListResponse, CardResponse } from "@/lib/types";

/**
 * Trae las tarjetas de todas las listas de un tablero en paralelo (una query por lista).
 * El drag-and-drop entre columnas necesita las tarjetas de todo el tablero en un solo
 * lugar (no una por columna) para poder mover una tarjeta de una lista a otra.
 */
export function useBoardCards(boardId: string, lists: BoardListResponse[]) {
  const results = useQueries({
    queries: lists.map((list) => ({
      queryKey: ["cards", boardId, list.id],
      queryFn: () => cardsApi.getCardsByList(boardId, list.id),
      enabled: !!boardId,
    })),
  });

  const isLoading = results.some((r) => r.isLoading);
  const cardsByListId: Record<string, CardResponse[]> = {};
  lists.forEach((list, i) => {
    cardsByListId[list.id] = results[i].data ?? [];
  });

  return { cardsByListId, isLoading };
}
