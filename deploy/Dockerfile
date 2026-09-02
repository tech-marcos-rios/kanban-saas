FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto primero para aprovechar el cache de capas
COPY Kanban.sln ./
COPY api/Kanban.Domain/Kanban.Domain.csproj                 api/Kanban.Domain/
COPY api/Kanban.Application/Kanban.Application.csproj       api/Kanban.Application/
COPY api/Kanban.Infrastructure/Kanban.Infrastructure.csproj api/Kanban.Infrastructure/
COPY api/Kanban.Api/Kanban.Api.csproj                        api/Kanban.Api/

RUN dotnet restore api/Kanban.Api/Kanban.Api.csproj

COPY . .

RUN dotnet publish api/Kanban.Api/Kanban.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Kanban.Api.dll"]
