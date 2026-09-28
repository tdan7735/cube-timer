FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/backend.csproj backend/
COPY Cube.Core/Cube.Core.csproj Cube.Core/
RUN dotnet restore backend/backend.csproj

COPY backend/ backend/
COPY Cube.Core/ Cube.Core/
RUN dotnet publish backend/backend.csproj --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "backend.dll"]
