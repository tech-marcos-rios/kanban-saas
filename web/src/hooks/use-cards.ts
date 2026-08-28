import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as cardsApi from "@/lib/api/cards";

export function useCardsByList(boardId: string, listId: string) {
  return useQuery({
    queryKey: ["cards", boardId, listId],
    queryFn: () => cardsApi.getCardsByList(boardId, listId),
    enabled: !!boardId && !!listId,
  });
}

export function useCreateCard(boardId: string, listId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (title: string) => cardsApi.createCard(boardId, listId, title),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["cards", boardId, listId] }),
  });
}

export function useDeleteCard(boardId: string, listId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (cardId: string) => cardsApi.deleteCard(boardId, cardId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["cards", boardId, listId] }),
  });
}

// El move puede cambiar la lista de destino, así que invalida ambas listas de cards
// (origen y destino) en vez de una sola.
export function useMoveCard(boardId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      cardId,
      listId,
      position,
    }: {
      cardId: string;
      listId: string;
      position: number;
    }) => cardsApi.moveCard(boardId, cardId, listId, position),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["cards", boardId] }),
  });
}
