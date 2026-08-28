import { forwardRef, type HTMLAttributes } from "react";
import { User } from "lucide-react";
import type { CardResponse } from "@/lib/types";

interface CardItemProps extends HTMLAttributes<HTMLDivElement> {
  card: CardResponse;
  isDragging?: boolean;
}

export const CardItem = forwardRef<HTMLDivElement, CardItemProps>(function CardItem(
  { card, isDragging, ...rest },
  ref,
) {
  return (
    <div
      ref={ref}
      {...rest}
      className={`rounded-md border border-gray-200 bg-white p-3 shadow-sm ${
        isDragging ? "opacity-50" : ""
      } ${rest.className ?? ""}`}
    >
      <p className="text-sm font-medium text-gray-900">{card.title}</p>
      {card.description && (
        <p className="mt-1 line-clamp-2 text-xs text-gray-500">{card.description}</p>
      )}
      {card.assignedUserName && (
        <p className="mt-2 flex items-center gap-1 text-xs text-gray-400">
          <User className="h-3 w-3" />
          {card.assignedUserName}
        </p>
      )}
    </div>
  );
});
