// Estos tipos calcan 1:1 con los DTOs (`record`) del backend en api/Kanban.Application/DTOs/.
// Si cambia un record ahí, hay que actualizar el tipo acá — no hay generación automática.

export type BoardRole = "Owner" | "Editor" | "Viewer";

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  userName: string;
  email: string;
  role: string;
}

export interface BoardResponse {
  id: string;
  name: string;
  ownerId: string;
  ownerName: string;
  role: BoardRole;
  createdAt: string;
  updatedAt: string | null;
}

export interface BoardListResponse {
  id: string;
  boardId: string;
  title: string;
  position: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface CardResponse {
  id: string;
  listId: string;
  title: string;
  description: string | null;
  position: number;
  assignedUserId: string | null;
  assignedUserName: string | null;
  dueDate: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface BoardMemberResponse {
  userId: string;
  name: string;
  email: string;
  role: BoardRole;
  joinedAt: string;
}

export interface ApiError {
  error: string;
}
