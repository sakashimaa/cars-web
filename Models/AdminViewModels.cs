using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cars_web.Models
{
    // Модели для администраторов
    public class AdminViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
        
        [JsonPropertyName("isSuperAdmin")]
        public bool IsSuperAdmin { get; set; }
        
        [JsonPropertyName("permissions")]
        public List<string> Permissions { get; set; }
        
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        
        [JsonPropertyName("lastLoginAt")]
        public DateTime? LastLoginAt { get; set; }
    }

    // Модели для пользователей
    public class UserViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("isEmailConfirmed")]
        public bool IsEmailConfirmed { get; set; }
        
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class UserUpdateModel
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }

    // Модели для автомобилей
    public class CarViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string FullDescription { get; set; }
        
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class CarCreateModel
    {
        [Required(ErrorMessage = "Название обязательно")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string FullDescription { get; set; }
        
        [Required(ErrorMessage = "Цена обязательна")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Количество обязательно")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Изображение обязательно")]
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
    }

    public class CarUpdateModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string FullDescription { get; set; }
        
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        
        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
    }

    // Модели для работников
    public class WorkerViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string FullDescription { get; set; }
        
        [JsonPropertyName("position")]
        public string Position { get; set; }
        
        [JsonPropertyName("workTime")]
        public int? WorkTime { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
        
        [JsonPropertyName("reviews")]
        public List<ReviewViewModel> Reviews { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class WorkerCreateModel
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string FullDescription { get; set; }
        
        [JsonPropertyName("position")]
        public string Position { get; set; }
        
        [JsonPropertyName("workTime")]
        public int? WorkTime { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
    }

    public class WorkerUpdateModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string FullDescription { get; set; }
        
        [JsonPropertyName("position")]
        public string Position { get; set; }
        
        [JsonPropertyName("workTime")]
        public int? WorkTime { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
    }

    public class ReviewViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string FullDescription { get; set; }
        
        [JsonPropertyName("rating")]
        public int Rating { get; set; }
    }

    // Модели для услуг
    public class ServiceViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("longDescription")]
        public string LongDescription { get; set; }
        
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class ServiceCreateModel
    {
        [Required(ErrorMessage = "Название обязательно")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Краткое описание обязательно")]
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("longDescription")]
        public string LongDescription { get; set; }
        
        [Required(ErrorMessage = "Цена обязательна")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
    }

    public class ServiceUpdateModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string ShortDescription { get; set; }
        
        [JsonPropertyName("longDescription")]
        public string LongDescription { get; set; }
        
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }
    }
} 