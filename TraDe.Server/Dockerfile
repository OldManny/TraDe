# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy projects and restore dependencies
COPY ["TraDe.Server/TraDe.Server.csproj", "TraDe.Server/"]
COPY ["TraDe.Core/TraDe.Core.csproj", "TraDe.Core/"]
RUN dotnet restore "TraDe.Server/TraDe.Server.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "TraDe.Server/TraDe.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 8080
ENTRYPOINT ["dotnet", "TraDe.Server.dll"]