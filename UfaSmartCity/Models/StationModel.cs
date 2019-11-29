using System;

namespace UfaSmartCity.Models
{
    public class StationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool Type { get; set; }
        public bool Active { get; set; }
        public string AccessCode { get; set; }
        public string UserCity { get; set; }
        public virtual InformationTable InformationTable { get; set; }

        public StationModel() { }
        public StationModel(Station station, string cityprefix, string city)
        {
            Id = cityprefix + station.id;
            Name = station.name;
            Description = station.descr;
            UserCity = city;
            try
            {
                Lat = Convert.ToDouble(station.lat.Substring(0, 2) + "." + station.lat.Substring(2, station.lat.Length - 2));
                Lng = Convert.ToDouble(station.lng.Substring(0, 2) + "." + station.lng.Substring(2, station.lng.Length - 2));
            }
            catch(Exception e)
            {
                Lat = Convert.ToDouble(station.lat.Substring(0, 2) + "," + station.lat.Substring(2, station.lat.Length - 2));
                Lng = Convert.ToDouble(station.lng.Substring(0, 2) + "," + station.lng.Substring(2, station.lng.Length - 2));
            }
            Type = station.type == "1"
                 ? true
                 : false;
            Active = false;
        }
    }      
}