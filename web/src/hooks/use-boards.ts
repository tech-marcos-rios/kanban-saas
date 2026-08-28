import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as boardsApi from "@/lib/api/boards";

export function useBoards() {
  return useQuery({ queryKey: ["boards"], queryFn: boardsApi.getMyBoards });
}

export function useBoard(boardId: string) {
  return useQuery({
    queryKey: ["boards", boardId],
    queryFn: () => boardsApi.getBoard(boardId),
    enabled: !!boardId,
  });
}

export function useCreateBoard() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (name: string) => boardsApi.createBoard(name),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["boards"] }),
  });
}

export function useDeleteBoard() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (boardId: string) => boardsApi.deleteBoard(boardId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["boards"] }),
  });
}
