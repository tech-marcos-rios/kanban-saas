"use client";

import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { ListColumn } from "./ListColumn";
import type { BoardListResponse, CardResponse } from "@/lib/types";

export function SortableListColumn({
  boardId,
  list,
  cards,
}: {
  boardId: string;
  list: BoardListResponse;
  cards: CardResponse[];
}) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: list.id,
    data: { type: "list-column", listId: list.id },
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <div ref={setNodeRef} style={style}>
      <ListColumn boardId={boardId} list={list} cards={cards} dragHandleProps={{ ...attributes, ...listeners }} />
    </div>
  );
}
