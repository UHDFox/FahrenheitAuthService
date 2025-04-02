FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-client

WORKDIR /app

# Копирование NuGet конфигурации
COPY ./FahrenheitAuthService.Client/NuGet.config /app/
RUN ls -l /app/NuGet.config

# Копирование .csproj файлов
COPY ./src/FahrenheitAuthService.Contracts/FahrenheitAuthService.Contracts.csproj /app/FahrenheitAuthService.Contracts/
COPY ./src/Web/Web.csproj /app/Web/
COPY ./FahrenheitAuthService.Client/FahrenheitAuthService.Client.csproj /app/FahrenheitAuthService.Client/
COPY ./src/Domain/Domain.csproj /app/Domain/
COPY ./src/Business/Business.csproj /app/Business/
COPY ./src/Repository/Repository.csproj /app/Repository/

# Копирование исходников
COPY ./src /app/

# Восстановление зависимостей
RUN dotnet restore /app/FahrenheitAuthService.Client/FahrenheitAuthService.Client.csproj --configfile /app/NuGet.config

# Новый этап для сборки веб-приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-web

WORKDIR /app

# Копирование .csproj файлов для веб-приложения
COPY ./src/FahrenheitAuthService.Contracts/FahrenheitAuthService.Contracts.csproj /app/FahrenheitAuthService.Contracts/
RUN ls /app/FahrenheitAuthService.Contracts/FahrenheitAuthService.Contracts.csproj
COPY ./src/Web/Web.csproj /app/Web/
COPY ./src/Domain/Domain.csproj /app/Domain/
COPY ./src/Business/Business.csproj /app/Business/
COPY ./src/Repository/Repository.csproj /app/Repository/

# Копирование конфигурации NuGet для веб-приложения
COPY ./FahrenheitAuthService.Client/NuGet.config /app/

# Копирование исходников для веб-приложения
COPY ./src /app/

# Восстановление зависимостей для веб-приложения
RUN dotnet restore /app/Web/Web.csproj --configfile /app/NuGet.config

# Очистка проектов
RUN dotnet clean /app/Web/Web.csproj -c Release
RUN dotnet clean /app/Business/Business.csproj -c Release
RUN dotnet clean /app/Domain/Domain.csproj -c Release
RUN dotnet clean /app/Repository/Repository.csproj -c Release
RUN dotnet clean /app/FahrenheitAuthService.Contracts/FahrenheitAuthService.Contracts.csproj -c Release

# Публикация веб-приложения
RUN dotnet publish /app/Web/Web.csproj -c Release -o /Release

# Финальный образ для выполнения приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Копирование опубликованного веб-приложения в финальный образ
COPY --from=build-web /Release ./ 

# Установка точки входа
ENTRYPOINT ["dotnet", "Web.dll"]
