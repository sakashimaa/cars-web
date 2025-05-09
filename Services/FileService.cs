using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace cars_web.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private readonly string _uploadPath;

        public FileService(IWebHostEnvironment environment, IConfiguration configuration, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
            _uploadPath = "uploads/images";
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Попытка загрузить пустой файл");
                    return null;
                }

                // Проверка расширения файла
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                
                if (!Array.Exists(validExtensions, e => e == extension))
                {
                    _logger.LogWarning("Попытка загрузить файл с недопустимым расширением: {Extension}", extension);
                    return null;
                }

                // Создаем уникальное имя файла
                var fileName = $"{Guid.NewGuid()}{extension}";
                
                // Проверка и создание директории для загрузки
                var uploadDir = Path.Combine(_environment.WebRootPath, _uploadPath);
                _logger.LogInformation("Директория для загрузки: {UploadDir}", uploadDir);
                
                if (!Directory.Exists(uploadDir))
                {
                    _logger.LogInformation("Директория не существует, создаем: {UploadDir}", uploadDir);
                    try
                    {
                        Directory.CreateDirectory(uploadDir);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Не удалось создать директорию для загрузки: {UploadDir}", uploadDir);
                        return null;
                    }
                }

                // Проверка доступа на запись
                try
                {
                    // Тестовый файл для проверки прав доступа
                    var testPath = Path.Combine(uploadDir, "test_write_access.tmp");
                    using (var fs = new FileStream(testPath, FileMode.Create))
                    {
                        fs.WriteByte(1);
                    }
                    if (File.Exists(testPath))
                    {
                        File.Delete(testPath);
                        _logger.LogInformation("Проверка доступа на запись прошла успешно");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Нет доступа на запись в директорию: {UploadDir}", uploadDir);
                    return null;
                }

                // Путь для сохранения файла
                var filePath = Path.Combine(uploadDir, fileName);
                _logger.LogInformation("Сохранение файла: {FilePath}", filePath);
                
                // Сохраняем файл
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    
                    // Проверка, что файл был успешно создан
                    if (!File.Exists(filePath))
                    {
                        _logger.LogError("Файл не был создан: {FilePath}", filePath);
                        return null;
                    }
                    
                    _logger.LogInformation("Файл успешно сохранен: {FilePath}", filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при записи файла: {FilePath}", filePath);
                    return null;
                }

                // Возвращаем относительный путь для сохранения в БД
                var relativeFilePath = $"{_uploadPath}/{fileName}";
                _logger.LogInformation("Относительный путь к файлу: {RelativeFilePath}", relativeFilePath);
                return relativeFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанная ошибка при сохранении изображения");
                return null;
            }
        }

        public bool DeleteImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    return false;
                }

                // Получаем полный путь к файлу
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении изображения: {Path}", imagePath);
                return false;
            }
        }
    }
} 