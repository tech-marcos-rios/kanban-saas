# 04 — Kanban SaaS: tablero colaborativo en tiempo real

> GitHub: [tech-marcos-rios/kanban-saas](https://github.com/tech-marcos-rios/kanban-saas)

El proyecto "estrella" del portafolio. Demuestra que podés construir un producto SaaS real con multiusuario y real-time. Tiempo estimado: **2 semanas**. Estado: 🚧 en progreso — backend (auth + modelo de dominio) arrancado, día 1-2 del plan.

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
- [ ] CRUD de tarjetas dentro de una lista.
- [ ] Hubs de SignalR para real-time.
- [ ] Frontend Next.js con drag-and-drop.

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
