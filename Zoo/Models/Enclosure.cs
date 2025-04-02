namespace Zoo.Models
{
    public class Enclosure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Animal> Animals { get; set; } = new();
        public Climate Climate { get; set; }
        public HabitatType HabitatType { get; set; }
        public SecurityLevel SecurityLevel { get; set; }
        public double Size { get; set; }
    }

    public enum Climate { Tropical, Temperate, Arctic }
    [Flags]
    public enum HabitatType { Forest = 1, Aquatic = 2, Desert = 4, Grassland = 8 }
}