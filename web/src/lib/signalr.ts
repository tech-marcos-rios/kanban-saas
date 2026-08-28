import { HubConnectionBuilder, LogLevel, type HubConnection } from "@microsoft/signalr";
import { getAccessToken } from "./auth-storage";
import { API_URL } from "./api-client";

export function createBoardConnection(): HubConnection {
  return new HubConnectionBuilder()
    .withUrl(`${API_URL}/hubs/board`, {
      accessTokenFactory: () => getAccessToken() ?? "",
    })
    .withAutomaticReconnect()
    // Critical (no Warning): en dev, React StrictMode monta/desmonta el efecto que crea
    // la conexión como chequeo de sanidad, lo que aborta la negociación de la primera
    // conexión "fantasma" y el cliente de SignalR loguea un "stopped during negotiation"
    // a nivel Error — es ruido esperado, no una falla real (la segunda conexión, la que
    // queda montada, sí conecta bien). Warning dejaba pasar ese ruido a la consola.
    .configureLogging(LogLevel.Critical)
    .build();
}
