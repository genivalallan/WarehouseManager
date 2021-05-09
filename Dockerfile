FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

ARG db_host=localhost
ARG db_port=3306
ARG db_user=root
ARG db_password
ARG db_name=warehouse

ENV ASPNETCORE_DB_CONN_STRING=server=${DB_HOST};port=${DB_PORT};database=${DB_NAME};user=${DB_USER};password=${DB_PASSWORD}

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["WarehouseManager.csproj", "./"]
RUN dotnet restore "WarehouseManager.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WarehouseManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WarehouseManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WarehouseManager.dll"]
