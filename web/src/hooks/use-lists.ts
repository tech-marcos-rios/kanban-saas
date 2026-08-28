import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as listsApi from "@/lib/api/lists";

export function useLists(boardId: string) {
  return useQuery({
    queryKey: ["lists", boardId],
    queryFn: () => listsApi.getLists(boardId),
    enabled: !!boardId,
  });
}

export function useCreateList(boardId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (title: string) => listsApi.createList(boardId, title),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["lists", boardId] }),
  });
}

export function useRenameList(boardId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ listId, title }: { listId: string; title: string }) =>
      listsApi.renameList(boardId, listId, title),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["lists", boardId] }),
  });
}

export function useMoveList(boardId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ listId, position }: { listId: string; position: number }) =>
      listsApi.moveList(boardId, listId, position),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["lists", boardId] }),
  });
}

export function useDeleteList(boardId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (listId: string) => listsApi.deleteList(boardId, listId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["lists", boardId] }),
  });
}
