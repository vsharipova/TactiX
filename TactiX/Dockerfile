# Базовый образ для runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

RUN dotnet tool install --global dotnet-ef --version 7.0.*

COPY ["TactiX/TactiX.csproj", "TactiX/"]
COPY ["TactiX.sln", "."]
RUN dotnet restore "TactiX/TactiX.csproj"

COPY . .
WORKDIR "/src/TactiX"
RUN dotnet build "TactiX.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TactiX.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TactiX.dll"]