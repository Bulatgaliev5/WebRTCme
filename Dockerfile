FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

RUN dotnet publish WebRTCme.Connection/Signaling/WebRTCme.Connection.Signaling.Server/WebRTCme.Connection.Signaling.Server.csproj \
    --no-dependencies -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebRTCme.Connection.Signaling.Server.dll"]
