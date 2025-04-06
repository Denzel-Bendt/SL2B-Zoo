using System.ComponentModel.DataAnnotations;

namespace Zoo.Models
{
    public class Animal
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //[Required]
        public string Species { get; set; }

        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public int Age { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public AnimalSize Size { get; set; }
        public DietaryClass DietaryClass { get; set; }
        public ActivityPattern ActivityPattern { get; set; }
        public string FeedingSchedule { get; set; }
        public int? PreyId { get; set; }
        public Animal? Prey { get; set; }

        public int? EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; }

        public double SpaceRequirement { get; set; }
        public SecurityLevel SecurityRequirement { get; set; }
    }

    public enum AnimalSize { Microscopic, VerySmall, Small, Medium, Large, VeryLarge }
    public enum DietaryClass { Carnivore, Herbivore, Omnivore, Insectivore, Piscivore }
    public enum ActivityPattern { Diurnal, Nocturnal, Cathemeral }
    public enum SecurityLevel { Low, Medium, High }
}