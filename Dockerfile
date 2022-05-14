# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GeoMediaService/GeoMediaService.csproj", "GeoMediaService/"]
RUN dotnet restore "GeoMediaService/GeoMediaService.csproj"
COPY . .
WORKDIR "/src/GeoMediaService"
RUN dotnet build "GeoMediaService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeoMediaService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeoMediaService.dll"]