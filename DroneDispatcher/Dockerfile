#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["DroneDispatcher/DroneDispatcher.csproj", "DroneDispatcher/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Domain.Implementation/Domain.Implementation.csproj", "Domain.Implementation/"]
RUN dotnet restore "DroneDispatcher/DroneDispatcher.csproj"
COPY . .
WORKDIR "/src/DroneDispatcher"
RUN dotnet build "DroneDispatcher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DroneDispatcher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DroneDispatcher.dll"]