import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { createBoardConnection } from "@/lib/signalr";

/**
 * Se une al grupo SignalR del tablero y, ante cada evento, invalida la query correspondiente
 * de TanStack Query en vez de parchear el cache a mano — más simple, y como los eventos ya
 * traen el dato fresco del server, el refetch es casi instantáneo.
 */
export function useBoardRealtime(boardId: string) {
  const queryClient = useQueryClient();
  const router = useRouter();

  useEffect(() => {
    if (!boardId) return;

    const connection = createBoardConnection();

    const invalidateLists = () => queryClient.invalidateQueries({ queryKey: ["lists", boardId] });
    const invalidateCards = () => queryClient.invalidateQueries({ queryKey: ["cards", boardId] });

    connection.on("ListCreated", invalidateLists);
    connection.on("ListUpdated", invalidateLists);
    connection.on("ListsReordered", invalidateLists);
    connection.on("ListDeleted", invalidateLists);

    connection.on("CardCreated", invalidateCards);
    connection.on("CardUpdated", invalidateCards);
    connection.on("CardMoved", invalidateCards);
    connection.on("CardDeleted", invalidateCards);

    connection.on("MembersChanged", () =>
      queryClient.invalidateQueries({ queryKey: ["members", boardId] }),
    );

    connection.on("RemovedFromBoard", () => {
      queryClient.invalidateQueries({ queryKey: ["boards"] });
      router.replace("/boards?notice=removed");
    });

    // En dev, React StrictMode monta el efecto, lo desmonta y lo vuelve a montar como
    // chequeo de sanidad — eso para la conexión a mitad de la negociación y tira un
    // AbortError esperable que no es un error real, así que no lo logueamos.
    let cleanedUp = false;

    (async () => {
      try {
        await connection.start();
      } catch (err) {
        if (!cleanedUp) console.error("Error conectando a SignalR:", err);
        return;
      }

      try {
        await connection.invoke("JoinBoard", boardId);
      } catch {
        // El hub rechaza el join con un HubException cuando el usuario no es miembro de
        // este tablero (o ya no existe) — es un estado de negocio esperado, no un error
        // de conexión, así que no lo logueamos como tal: solo redirigimos afuera. El
        // console.error acá terminaba disparando el overlay de errores de Next en dev.
        if (!cleanedUp) router.replace("/boards?notice=no-access");
      }
    })();

    return () => {
      cleanedUp = true;
      connection.invoke("LeaveBoard", boardId).catch(() => {});
      connection.stop();
    };
  }, [boardId, queryClient, router]);
}
