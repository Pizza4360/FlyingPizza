#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:net6.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:net6.0 AS build
WORKDIR /src
COPY ["SimulatedDrone/SimulatedDrone.csproj", "SimulatedDrone/"]
RUN dotnet restore "SimulatedDrone/SimulatedDrone.csproj"
COPY . .
WORKDIR "/src/SimulatedDrone"
RUN dotnet build "SimulatedDrone.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimulatedDrone.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimulatedDrone.dll"]