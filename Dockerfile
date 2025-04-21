# Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-dotnet-env
WORKDIR /app
COPY . .
RUN dotnet build TGParser.sln /restore /t:Publish /p:Configuration="Release" /p:OutputPath=/app/out/publish

# Сборка приложения
FROM mcr.microsoft.com/dotnet/aspnet:9.0 as base
WORKDIR /opt/TGParser
COPY --from=build-dotnet-env /app/out/publish .

# Добавление настройки openssl
RUN apt-get -y update
RUN apt-get -y install openssl

ENTRYPOINT ["dotnet", "TGParser.API.dll"]