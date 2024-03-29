﻿namespace UfaSmartCity.Models
{
    public class Predict
    {
        public string NumberBus { get; set; }
        public string EndStation { get; set; }
        public string TimePredict { get; set; }

        public Predict() { }

        public Predict(StationForecast stationForecast)
        {
            NumberBus = stationForecast.Rnum;
            EndStation = stationForecast.Where;
            TimePredict = stationForecast.Arrt != null
                        ? (((int)stationForecast.Arrt / 60) == 0
                          ? "1"
                          : ((int)stationForecast.Arrt / 60).ToString())
                        : "";
        }
    }
}