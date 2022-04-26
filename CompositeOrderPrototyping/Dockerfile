FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CompositeOrderTest/CompositeOrderTest.csproj", "CompositeOrderTest/"]
RUN dotnet restore "CompositeOrderTest/CompositeOrderTest.csproj"
COPY . .
WORKDIR "/src/CompositeOrderTest"
RUN dotnet build "CompositeOrderTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CompositeOrderTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompositeOrderTest.dll"]
