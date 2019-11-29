namespace UfaSmartCity.Models
{
    public class CreateNewUserViewModel
    {
        public string Email { get; set; }
        public string CityName { get; set; }
    }

    public class CreateNewCityViewModel
    {
        public string CityName { get; set; }
        public string CityKey { get; set; }
    }
}