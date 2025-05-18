FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["TactiX/TactiX.csproj", "TactiX/"]
RUN dotnet restore "TactiX/TactiX.csproj"

COPY . .

WORKDIR "/src/TactiX"
RUN dotnet build "TactiX.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TactiX.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TactiX.dll"]