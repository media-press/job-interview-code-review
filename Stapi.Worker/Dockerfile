FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Stapi.Worker/Stapi.Worker.csproj", "Stapi.Worker/"]
RUN dotnet restore "Stapi.Worker/Stapi.Worker.csproj"
COPY . .
WORKDIR "/src/Stapi.Worker"
RUN dotnet build "Stapi.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stapi.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stapi.Worker.dll"]