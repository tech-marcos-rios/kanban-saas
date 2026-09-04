import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import * as membersApi from "@/lib/api/members";

export function useMembers(boardId: string) {
  return useQuery({
    queryKey: ["members", boardId],
    queryFn: () => membersApi.getMembers(boardId),
    enabled: !!boardId,
  });
}

export function useInviteMember(boardId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ email, role }: { email: string; role: "Editor" | "Viewer" }) =>
      membersApi.inviteMember(boardId, email, role),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["members", boardId] }),
  });
}

export function useRemoveMember(boardId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (userId: string) => membersApi.removeMember(boardId, userId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["members", boardId] }),
  });
}
