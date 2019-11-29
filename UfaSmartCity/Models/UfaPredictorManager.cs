using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace UfaSmartCity.Models
{
    public class UfaPredictorManager : AbstractPredictManager
    {
        public new IEnumerable<StationForecast> GetStationForecast(string idStation)
        {
            string result = null;
            var http = "http://glonass.ufagortrans.ru/php/getStationForecasts.php?sid=" + idStation + "&type=0&city=ufagortrans&info=12345&_=1517558480816";
            //********kostil
            var request = (HttpWebRequest)WebRequest.Create(http);
            //request.KeepAlive = true;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                try
                {
                    result = reader.ReadToEnd();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            var jResult = JToken.Parse(result).ToObject<IEnumerable<StationForecast>>()
                                .ToList();
            jResult.RemoveAll(j => j.Arrt < 20 || j.Arrt > 1201);
            return jResult;
        }

        public new IEnumerable<Station> GetStations()
        {
            string result = null;
            var http = "http://glonass.ufagortrans.ru/php/getStations.php?city=ufagortrans&info=12345&_=1517558480807";
            var request = (HttpWebRequest)WebRequest.Create(http);
            //********kostil
            //request.KeepAlive = true;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                try
                {
                    result = reader.ReadToEnd();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            var jResult = JToken.Parse(result).ToObject<IEnumerable<Station>>().ToList();
            return jResult;
        }

        public new string GetLatLngStationFor(string idStation)
        {
            string result = "";
            var http = "http://glonass.ufagortrans.ru/php/getStations.php?city=ufagortrans&info=12345&_=1517558480807";
            var request = (HttpWebRequest)WebRequest.Create(http);
            //********kostil
            //request.KeepAlive = true;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                    {
                        try
                        {
                            result = reader.ReadToEnd();
                        }
                        catch(Exception e)
                        {
                            return "54.773353;55.893373";
                        }
                    }
                }
            }
            var jResult = JToken.Parse(result).ToObject<IEnumerable<Station>>().ToList();
            var station = jResult.Find(s => string.Equals(s.id.ToString(), idStation));
            string lat = "";
            string lng = "";
            if (station != null)
            {
                lat = station.lat;
                lng = station.lng;
            }
            if (string.IsNullOrEmpty(lat)) return "";
            if (string.IsNullOrEmpty(lng)) return "";
            if (lat.IndexOf('.') < 0)
                lat = lat.Substring(0, 2) + "." + lat.Substring(2, lat.Length - 3);
            if (lng.IndexOf('.') < 0)
                lng = lng.Substring(0, 2) + "." + lng.Substring(2, lng.Length - 3);
            return lat + ";" + lng;
        }

        public UfaPredictorManager()
        {
            City = "ufa";
        }
    }
}