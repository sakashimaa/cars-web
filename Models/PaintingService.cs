using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cars_web.Models
{
    public class PaintingService
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string ShortDescription { get; set; }
        
        public string LongDescription { get; set; }
        
        public string? Price { get; set; }
        
        public string Type { get; set; }
        
        public int EstimatedDays { get; set; }
        
        public List<string> Reviews { get; set; }
        
        public List<string> BeforeAfterImages { get; set; }
        
        public string AdditionalRequirements { get; set; }
        
        public int WarrantyMonths { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
} 