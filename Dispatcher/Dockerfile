#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 4000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Dispatcher/Dispatcher.csproj", "Dispatcher/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["DomainImplementation/DomainImplementation.csproj", "DomainImplementation/"]
RUN dotnet restore "Dispatcher/Dispatcher.csproj"
COPY . .
WORKDIR "/src/Dispatcher"
RUN dotnet build "Dispatcher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dispatcher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dispatcher.dll"]