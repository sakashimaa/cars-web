FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Устанавливаем переменные окружения
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["cars-web.csproj", "."]
RUN dotnet restore "cars-web.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "cars-web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "cars-web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Создаем директорию для загруженных изображений
RUN mkdir -p /app/wwwroot/uploads/images
# Устанавливаем права на директорию
RUN chmod -R 777 /app/wwwroot/uploads/images

# Создаем директорию для ключей шифрования
RUN mkdir -p /app/keys
ENV ASPNETCORE_DataProtection__Keys__Path=/app/keys
ENV ASPNETCORE_DataProtection__ApplicationName=CarsWeb

ENTRYPOINT ["dotnet", "cars-web.dll"] 