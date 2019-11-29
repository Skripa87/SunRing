namespace UfaSmartCity.Models
{
    public enum CityKey { STERLITAMAK, UFA}

    public class City
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CityKey CityKey { get; set; }

        public City() { }

    }
}