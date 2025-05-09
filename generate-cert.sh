#!/bin/bash
set -e

# Создаем директорию для сертификатов, если ее нет
mkdir -p https

# Генерируем сертификат
dotnet dev-certs https -ep https/aspnetapp.pfx -p "yourpassword"

echo "Сертификат SSL успешно сгенерирован в папке https/"
echo "Используйте пароль 'yourpassword' для этого сертификата" 