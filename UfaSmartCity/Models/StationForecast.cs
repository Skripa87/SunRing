using System;

namespace UfaSmartCity.Models
{
    public partial class StationForecast
    {
        public int Id { get; set; }
        public Nullable<int> Arrt { get; set; }
        public string Where { get; set; }
        public string Vehid { get; set; }
        public Nullable<int> Rid { get; set; }
        public string Rtype { get; set; }
        public string Rnum { get; set; }
        public string Lastst { get; set; }
    }
}
