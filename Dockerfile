# Стадия 1: Сборка и упаковка NuGet-пакета для клиента
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-client

WORKDIR /app

# Копируем конфигурацию NuGet
COPY ./src/FahrenheitAuthService.Client/NuGet.config /app/nuget

# Копируем проекты клиента и его зависимости
COPY ./src/Web/Web.csproj /app/Web/
COPY ./FahrenheitAuthService.Client/FahrenheitAuthService.Client.csproj /app/FahrenheitAuthService.Client/
COPY ./src/Domain/Domain.csproj /app/Domain/
COPY ./src/Business/Business.csproj /app/Business/
COPY ./src/Repository/Repository.csproj /app/Repository/

# Восстанавливаем зависимости клиента
RUN dotnet restore /app/FahrenheitAuthService.Client/FahrenheitAuthService.Client.csproj

# Копируем исходный код
COPY ./src /app/

# Упаковываем NuGet-пакет только для клиента
RUN rm -f /app/nuget && mkdir -p /app/nuget && \
    dotnet pack /app/FahrenheitAuthService.Client/FahrenheitAuthService.Client.csproj -c Release -o /app/nuget



# Стадия 2: Сборка и публикация веб-приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-web

WORKDIR /app

COPY ./src/Web/Web.csproj /app/Web/
COPY ./src/Domain/Domain.csproj /app/Domain/
COPY ./src/Business/Business.csproj /app/Business/
COPY ./src/Repository/Repository.csproj /app/Repository/

COPY ./src /app/




COPY . ./

RUN dotnet restore /app/Web/Web.csproj

RUN dotnet clean /app/Web/Web.csproj -c Release
RUN dotnet clean /app/Business/Business.csproj -c Release
RUN dotnet clean /app/Domain/Domain.csproj -c Release
RUN dotnet clean /app/Repository/Repository.csproj -c Release




RUN dotnet publish /app/Web/Web.csproj -c Release -o /Release



# Стадия 3: Финальный образ для веб-приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Копируем опубликованные файлы веб-приложения
COPY --from=build-web /Release .

# Точка входа
ENTRYPOINT ["dotnet", "Web.dll"]
