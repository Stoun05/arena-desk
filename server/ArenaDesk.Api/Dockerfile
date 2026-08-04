FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY server/ArenaDesk.Api/ArenaDesk.Api.csproj server/ArenaDesk.Api/
COPY shared/ArenaDesk.Contracts/ArenaDesk.Contracts.csproj shared/ArenaDesk.Contracts/
RUN dotnet restore server/ArenaDesk.Api/ArenaDesk.Api.csproj
COPY server/ArenaDesk.Api/ server/ArenaDesk.Api/
COPY shared/ArenaDesk.Contracts/ shared/ArenaDesk.Contracts/
RUN dotnet publish server/ArenaDesk.Api/ArenaDesk.Api.csproj --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "ArenaDesk.Api.dll"]
