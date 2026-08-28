# 04 — Kanban SaaS: tablero colaborativo en tiempo real

> GitHub: [tech-marcos-rios/kanban-saas](https://github.com/tech-marcos-rios/kanban-saas)

El proyecto "estrella" del portafolio. Demuestra que podés construir un producto SaaS real con multiusuario y real-time. Tiempo estimado: **2 semanas**. Estado: 🚧 en progreso — backend completo (auth, tableros, listas, tarjetas, miembros, real-time, seguridad revisada) y frontend con el golden path funcionando (auth, tableros, drag-and-drop, tiempo real). Quedan: UI de invitaciones, comentarios/labels, deploy.

## ¿Qué construir?

Un clon funcional de Trello. Tableros con listas, listas con tarjetas, drag-and-drop, multiusuario, comentarios, real-time.

## Stack

**Backend**
- .NET 8 Web API
- SignalR (real-time)
- PostgreSQL + EF Core
- JWT auth

**Frontend**
- Next.js 14 + TypeScript + Tailwind
- `@dnd-kit/core` (drag and drop)
- Tanstack Query
- SignalR client

## Features mínimos (MVP)

1. Auth (reusar lo del Proyecto 1).
2. Crear/editar/eliminar tableros.
3. Listas dentro del tablero (To Do, Doing, Done o las que el user quiera).
4. Tarjetas dentro de listas con título y descripción Markdown.
5. Drag and drop de tarjetas entre listas y de listas dentro del tablero.
6. Invitar a otros usuarios al tablero por email.
7. **Real-time**: si dos usuarios tienen el tablero abierto, ven los cambios al instante (SignalR).
8. Comentarios en tarjetas.
9. Asignar usuarios a tarjetas.

## Features extra (si te sobra tiempo)

- Etiquetas (labels) de colores en tarjetas.
- Fechas de vencimiento con alertas.
- Adjuntar archivos (Azure Blob Storage).
- Tableros públicos (read-only).
- Exportar tablero a JSON.

## Modelo de datos

```
User (Id, Email, Name, PasswordHash)
Board (Id, Name, OwnerId)
BoardMember (BoardId, UserId, Role)  // owner, editor, viewer
List (Id, BoardId, Title, Position)
Card (Id, ListId, Title, Description, Position, AssignedUserId, DueDate)
Comment (Id, CardId, UserId, Content, CreatedAt)
Label (Id, BoardId, Name, Color)
CardLabel (CardId, LabelId)
```

## Real-time con SignalR

Cuando un usuario edita algo, el backend emite el evento al "grupo" del tablero:

```csharp
await _hub.Clients.Group($"board-{boardId}").SendAsync("CardMoved", cardId, newListId, newPosition);
```

El frontend escucha y actualiza la cache de Tanstack Query con `queryClient.setQueryData`.

## Progreso actual

- [x] Solución .NET 8 con Clean Architecture (Api / Application / Domain / Infrastructure / Tests).
- [x] Modelo de dominio: `User`, `Board`, `BoardList`, `BoardMember`, `BoardRole`, `Card`, `CardLabel`, `Comment`, `Label`, `Role`.
- [x] Migración inicial de EF Core (`InitialCreate`) contra PostgreSQL.
- [x] Auth: `AuthController` + `AuthService` (register/login/refresh).
- [x] `docker-compose.dev.yml` con Postgres para desarrollo local.
- [x] CRUD de tableros (`BoardsController`): crear, listar los propios, ver, renombrar, eliminar — con permisos por rol (Owner/Editor/Viewer) vía `BoardMember`.
- [x] CRUD de listas (`BoardListsController`, anidado en `/boards/{boardId}/lists`): crear, listar, renombrar, reordenar (drag-and-drop de listas), eliminar. Mismo esquema de permisos que tableros (Viewer solo lee).
- [x] CRUD de tarjetas (`CardsController`, en `/boards/{boardId}/cards`): crear en una lista, ver, editar (título/descripción/vencimiento), asignar a un miembro del tablero, mover (misma lista o entre listas, con drag-and-drop), eliminar.
- [x] Invitar/quitar miembros (`BoardMembersController`, en `/boards/{boardId}/members`): invitar por email con rol Editor o Viewer, listar miembros, eliminar. Solo el Owner puede invitar o eliminar; el Owner del tablero no se puede eliminar.
- [x] Real-time con SignalR (`BoardHub` en `/hubs/board`): el cliente se une al grupo del tablero con `JoinBoard(boardId)` (valida membership) y recibe `ListCreated/Updated/Deleted`, `ListsReordered`, `CardCreated/Updated/Deleted`, `CardMoved` a medida que otros usuarios editan. El JWT se pasa como `?access_token=` en la conexión porque WebSocket no puede mandar headers custom.
- [x] Revisión de seguridad completa: cuenta admin hardcodeada eliminada, validación de contraseña en el registro, timing attack en login mitigado (hash bcrypt señuelo), refresh tokens hasheados (SHA-256) en la base, Swagger oculto fuera de Development, emails normalizados (trim + lowercase), invitaciones con rate limit, y un miembro eliminado ahora se saca en vivo de su grupo de SignalR en vez de seguir recibiendo eventos hasta reconectar.
- [x] Frontend Next.js 16 + TypeScript + Tailwind (`web/`): auth (login/register) contra la API real, listar/crear tableros, listas y tarjetas, drag-and-drop de tarjetas entre listas y de listas dentro del tablero (`@dnd-kit`), tiempo real con el cliente de SignalR (`@microsoft/signalr`) sobre TanStack Query. Probado de punta a punta con dos usuarios simultáneos en un tablero (Playwright). Falta: UI para invitar/gestionar miembros (el endpoint del backend ya existe) y comentarios/labels.
- [ ] Comentarios en tarjetas y etiquetas (labels).
- [ ] UI para invitar/quitar miembros del tablero (el backend ya lo soporta).

## Plan paso a paso

### Semana 1
- Día 1-2: Modelo de datos, migraciones, auth.
- Día 3-4: CRUD de tableros, listas, tarjetas.
- Día 5-7: Frontend con tableros y drag-and-drop básico.

### Semana 2
- Día 8-9: SignalR — eventos de cambios en tarjetas y listas.
- Día 10-11: Invitaciones, miembros, comentarios.
- Día 12-13: Polish (animaciones, estados vacíos, empty states).
- Día 14: Deploy + README + video demo.

## Por qué este proyecto

Es la prueba más fuerte de que sos capaz de construir SaaS reales. Si un cliente busca alguien para construirle "una herramienta interna como Trello pero para X", te elige a vos.

También te sirve como base reutilizable: la mitad de los proyectos freelance que vas a ver son variantes de "tablero kanban con cositas extra".
