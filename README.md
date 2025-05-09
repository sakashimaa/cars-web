# Cars Web - Приложение для автосалона

## Запуск с использованием Docker

### Требования

- Docker
- Docker Compose
- .NET SDK (для генерации сертификатов)

### Инструкции по запуску

1. Клонировать репозиторий:

```bash
git clone <url-репозитория>
cd cars-web
```

2. Генерация SSL-сертификатов для HTTPS (требуется .NET SDK):

```bash
# Для Linux/Mac:
chmod +x generate-cert.sh
./generate-cert.sh

# Для Windows (PowerShell):
mkdir -p https
dotnet dev-certs https -ep https\aspnetapp.pfx -p "yourpassword"
```

3. Запустить приложение с помощью Docker Compose:

```bash
docker-compose up -d
```

4. Приложение будет доступно по следующим адресам:
   - HTTP: http://localhost:8080
   - HTTPS: https://localhost:8443

### Остановка приложения

```bash
docker-compose down
```

### Примечания

- Загруженные изображения сохраняются в Docker volume `uploaded_images` и сохраняются между перезапусками контейнера
- Для изменения портов отредактируйте файл `docker-compose.yml`
- Пароль для SSL-сертификата установлен как "yourpassword" - при необходимости измените его в файлах `docker-compose.yml` и `generate-cert.sh`

## Разработка

### Требования для локальной разработки

- .NET 9.0 SDK
- Visual Studio 2022 или другая IDE

### Запуск для разработки

```bash
dotnet restore
dotnet run
```
