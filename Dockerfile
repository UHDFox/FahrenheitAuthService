# Стадия 1: Сборка и восстановление зависимостей для клиента
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-client

WORKDIR /app

# Копируем конфигурацию NuGet
COPY ./FahrenheitAuthService.Client/NuGet.config /app/NuGet.config

# Копируем проекты клиента и его зависимости
COPY ./src/Contracts/Contracts.csproj /app/Contracts/
COPY ./src/Web/Web.csproj /app/Web/
COPY ./FahrenheitAuthService.Client/FahrenheitAuthService.Client.csproj /app/FahrenheitAuthService.Client/
COPY ./src/Domain/Domain.csproj /app/Domain/
COPY ./src/Business/Business.csproj /app/Business/
COPY ./src/Repository/Repository.csproj /app/Repository/

# Восстанавливаем зависимости клиента (без упаковки)
RUN dotnet restore /app/FahrenheitAuthService.Client/FahrenheitAuthService.Client.csproj --configfile /app/NuGet.config

# Копируем исходный код
COPY ./src /app/

# Стадия 2: Сборка и публикация веб-приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-web

WORKDIR /app

# Копируем проекты веб-приложения
COPY ./src/FahrenheitAuthService.Contracts/FahrenheitAuthService.Contracts.csproj /app/Contracts/
COPY ./src/Web/Web.csproj /app/Web/
COPY ./src/Domain/Domain.csproj /app/Domain/
COPY ./src/Business/Business.csproj /app/Business/
COPY ./src/Repository/Repository.csproj /app/Repository/

# Добавляем NuGet-источник (если нужно)
COPY --from=build-client /app/nuget /app/nuget
RUN dotnet nuget add source /app/nuget --name DockerFahrenheitRepo

# Копируем исходный код
COPY ./src /app/

# Восстанавливаем зависимости веб-приложения из NuGet
RUN dotnet restore /app/Web/Web.csproj --configfile /app/NuGet.config

# Очищаем старые сборки
RUN dotnet clean /app/Web/Web.csproj -c Release
RUN dotnet clean /app/Business/Business.csproj -c Release
RUN dotnet clean /app/Domain/Domain.csproj -c Release
RUN dotnet clean /app/Repository/Repository.csproj -c Release
RUN dotnet clean /app/FahrenheitAuthService.Contracts/FahrenheitAuthService.Contracts.csproj -c Release

# Собираем и публикуем веб-приложение
RUN dotnet publish /app/Web/Web.csproj -c Release -o /Release

# Стадия 3: Финальный образ для веб-приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Копируем опубликованные файлы веб-приложения
COPY --from=build-web /Release ./

# Точка входа
ENTRYPOINT ["dotnet", "Web.dll"]
