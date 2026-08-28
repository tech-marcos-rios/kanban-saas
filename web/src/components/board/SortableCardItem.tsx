"use client";

import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { CardItem } from "./CardItem";
import type { CardResponse } from "@/lib/types";

export function SortableCardItem({ card }: { card: CardResponse }) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: card.id,
    data: { type: "card", card },
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  return (
    <CardItem
      ref={setNodeRef}
      style={style}
      card={card}
      isDragging={isDragging}
      {...attributes}
      {...listeners}
    />
  );
}
