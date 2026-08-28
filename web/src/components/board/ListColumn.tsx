import { useDroppable } from "@dnd-kit/core";
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable";
import { SortableCardItem } from "./SortableCardItem";
import { CreateCardForm } from "./CreateCardForm";
import type { BoardListResponse, CardResponse } from "@/lib/types";

interface ListColumnProps {
  boardId: string;
  list: BoardListResponse;
  cards: CardResponse[];
  dragHandleProps?: Record<string, unknown>;
}

export function ListColumn({ boardId, list, cards, dragHandleProps }: ListColumnProps) {
  // droppable independiente del sortable de tarjetas: sin esto, soltar una tarjeta sobre
  // una lista vacía (sin ninguna otra tarjeta a la cual "engancharse") no dispara nada.
  const { setNodeRef } = useDroppable({ id: list.id, data: { type: "list", listId: list.id } });
  const cardIds = cards.map((c) => c.id);

  return (
    <div className="w-72 shrink-0 rounded-lg bg-gray-100 p-3">
      <h3
        {...dragHandleProps}
        className="mb-3 cursor-grab px-1 text-sm font-semibold text-gray-700 active:cursor-grabbing"
      >
        {list.title}
      </h3>

      <div ref={setNodeRef} className="min-h-[4px] space-y-2">
        <SortableContext items={cardIds} strategy={verticalListSortingStrategy}>
          {cards.map((card) => (
            <SortableCardItem key={card.id} card={card} />
          ))}
        </SortableContext>
      </div>

      <div className="mt-2">
        <CreateCardForm boardId={boardId} listId={list.id} />
      </div>
    </div>
  );
}
