#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["DroneSimulator/DroneSimulator.csproj", "DroneSimulator/"]
RUN dotnet restore "DroneSimulator/DroneSimulator.csproj"
COPY . .
WORKDIR "/src/DroneSimulator"
RUN dotnet build "DroneSimulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DroneSimulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DroneSimulator.dll"]