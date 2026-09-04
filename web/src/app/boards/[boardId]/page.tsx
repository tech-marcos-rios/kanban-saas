"use client";

import { useEffect, useRef, useState } from "react";
import { useParams } from "next/navigation";
import { useQueryClient } from "@tanstack/react-query";
import Link from "next/link";
import { ArrowLeft, Users } from "lucide-react";
import {
  DndContext,
  DragOverlay,
  PointerSensor,
  closestCorners,
  useSensor,
  useSensors,
  type DragEndEvent,
  type DragOverEvent,
  type DragStartEvent,
} from "@dnd-kit/core";
import { SortableContext, arrayMove, horizontalListSortingStrategy } from "@dnd-kit/sortable";
import { useBoard } from "@/hooks/use-boards";
import { useLists, useMoveList } from "@/hooks/use-lists";
import { useBoardCards } from "@/hooks/use-board-cards";
import { useMoveCard } from "@/hooks/use-cards";
import { useBoardRealtime } from "@/hooks/use-board-realtime";
import { SortableListColumn } from "@/components/board/SortableListColumn";
import { CardItem } from "@/components/board/CardItem";
import { CreateListForm } from "@/components/board/CreateListForm";
import { MembersModal } from "@/components/board/MembersModal";
import type { BoardListResponse, CardResponse } from "@/lib/types";

export default function BoardDetailPage() {
  const { boardId } = useParams<{ boardId: string }>();
  const { data: board } = useBoard(boardId);
  const { data: listsData, isLoading, isError } = useLists(boardId);
  const lists = listsData ? [...listsData].sort((a, b) => a.position - b.position) : [];
  const { cardsByListId } = useBoardCards(boardId, lists);
  const moveList = useMoveList(boardId);
  const moveCard = useMoveCard(boardId);
  const queryClient = useQueryClient();
  useBoardRealtime(boardId);

  // Estado local: es el que se pinta y se reordena en vivo durante el drag. Se re-sincroniza
  // con lo que trae la API cuando no hay un drag en curso (si no, cada refetch de fondo
  // "pelearía" con el reordenamiento optimista y la tarjeta saltaría de vuelta a su lugar).
  const [columnOrder, setColumnOrder] = useState<BoardListResponse[]>([]);
  const [cardsByList, setCardsByList] = useState<Record<string, CardResponse[]>>({});
  const isDraggingRef = useRef(false);
  const [activeCard, setActiveCard] = useState<CardResponse | null>(null);
  const [activeList, setActiveList] = useState<BoardListResponse | null>(null);
  const [isMembersOpen, setIsMembersOpen] = useState(false);

  useEffect(() => {
    if (isDraggingRef.current) return;
    setColumnOrder(lists);
    setCardsByList(cardsByListId);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [listsData, JSON.stringify(cardsByListId)]);

  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 5 } }));

  const findListIdForCard = (cardId: string): string | undefined =>
    Object.keys(cardsByList).find((listId) => cardsByList[listId].some((c) => c.id === cardId));

  const handleDragStart = (event: DragStartEvent) => {
    isDraggingRef.current = true;
    const { active } = event;
    if (active.data.current?.type === "card") {
      setActiveCard(active.data.current.card as CardResponse);
    } else if (active.data.current?.type === "list-column") {
      setActiveList(columnOrder.find((l) => l.id === active.id) ?? null);
    }
  };

  const handleDragOver = (event: DragOverEvent) => {
    const { active, over } = event;
    if (!over || active.data.current?.type !== "card") return;

    const activeId = active.id as string;
    const overId = over.id as string;

    const sourceListId = findListIdForCard(activeId);
    const targetListId =
      over.data.current?.type === "card" ? findListIdForCard(overId) : (overId as string);

    if (!sourceListId || !targetListId || sourceListId === targetListId) return;
    if (!cardsByList[targetListId]) return;

    setCardsByList((prev) => {
      const sourceCards = [...prev[sourceListId]];
      const cardIndex = sourceCards.findIndex((c) => c.id === activeId);
      if (cardIndex === -1) return prev;
      const [movedCard] = sourceCards.splice(cardIndex, 1);

      const targetCards = [...prev[targetListId]];
      const overIndex = targetCards.findIndex((c) => c.id === overId);
      const insertAt = overIndex >= 0 ? overIndex : targetCards.length;
      targetCards.splice(insertAt, 0, movedCard);

      return { ...prev, [sourceListId]: sourceCards, [targetListId]: targetCards };
    });
  };

  const handleDragEnd = (event: DragEndEvent) => {
    isDraggingRef.current = false;
    const { active, over } = event;
    setActiveCard(null);
    setActiveList(null);
    if (!over) return;

    if (active.data.current?.type === "list-column") {
      const oldIndex = columnOrder.findIndex((l) => l.id === active.id);
      const newIndex = columnOrder.findIndex((l) => l.id === over.id);
      if (oldIndex === -1 || newIndex === -1 || oldIndex === newIndex) return;

      const reordered = arrayMove(columnOrder, oldIndex, newIndex);
      setColumnOrder(reordered);
      moveList.mutate({ listId: active.id as string, position: newIndex });
      return;
    }

    if (active.data.current?.type === "card") {
      const cardId = active.id as string;
      const listId = findListIdForCard(cardId);
      if (!listId) return;

      const overId = over.id as string;
      const cardsInList = cardsByList[listId];
      const oldIndex = cardsInList.findIndex((c) => c.id === cardId);
      const overIndex = cardsInList.findIndex((c) => c.id === overId);

      let finalIndex = oldIndex;
      if (overIndex >= 0 && overIndex !== oldIndex) {
        const reordered = arrayMove(cardsInList, oldIndex, overIndex);
        setCardsByList((prev) => ({ ...prev, [listId]: reordered }));
        finalIndex = overIndex;
      }

      moveCard.mutate(
        { cardId, listId, position: finalIndex },
        {
          onSettled: () => {
            queryClient.invalidateQueries({ queryKey: ["cards", boardId] });
          },
        },
      );
    }
  };

  return (
    <div className="flex h-[calc(100vh-57px)] flex-col">
      <div className="flex items-start justify-between border-b border-gray-200 bg-white px-4 py-3">
        <div>
          <Link
            href="/boards"
            className="flex items-center gap-1 text-xs text-gray-500 hover:text-gray-700"
          >
            <ArrowLeft className="h-3.5 w-3.5" />
            Mis tableros
          </Link>
          <h1 className="mt-1 text-lg font-bold text-gray-900">{board?.name ?? "..."}</h1>
        </div>
        <button
          onClick={() => setIsMembersOpen(true)}
          className="flex items-center gap-1.5 rounded-md border border-gray-300 px-2.5 py-1.5 text-xs font-medium text-gray-700 hover:bg-gray-50"
        >
          <Users className="h-3.5 w-3.5" />
          Miembros
        </button>
      </div>

      <MembersModal
        boardId={boardId}
        isOpen={isMembersOpen}
        onClose={() => setIsMembersOpen(false)}
        currentRole={board?.role}
      />

      <div className="flex-1 overflow-x-auto px-4 py-4">
        {isLoading && <p className="text-sm text-gray-500">Cargando tablero...</p>}
        {isError && <p className="text-sm text-red-600">No se pudo cargar el tablero.</p>}

        {listsData && (
          <DndContext
            sensors={sensors}
            collisionDetection={closestCorners}
            onDragStart={handleDragStart}
            onDragOver={handleDragOver}
            onDragEnd={handleDragEnd}
          >
            <div className="flex items-start gap-3">
              <SortableContext
                items={columnOrder.map((l) => l.id)}
                strategy={horizontalListSortingStrategy}
              >
                {columnOrder.map((list) => (
                  <SortableListColumn
                    key={list.id}
                    boardId={boardId}
                    list={list}
                    cards={cardsByList[list.id] ?? []}
                  />
                ))}
              </SortableContext>
              <CreateListForm boardId={boardId} />
            </div>

            <DragOverlay>
              {activeCard && <CardItem card={activeCard} />}
              {activeList && (
                <div className="w-72 rounded-lg bg-gray-200 p-3 shadow-lg">
                  <h3 className="text-sm font-semibold text-gray-700">{activeList.title}</h3>
                </div>
              )}
            </DragOverlay>
          </DndContext>
        )}
      </div>
    </div>
  );
}
