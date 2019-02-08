using System;
using System.ComponentModel.DataAnnotations;
using What3Words;
using What3Words.Enums;

namespace SALUSUAV_Demo.Models.FlightData
{
    public class FlightData
    {
        public Guid Id { get; set; }
        public string FlightName { get; set; }

       [Display(Name = "Flight Date")]
        [DataType(DataType.Date)]
        public DateTime? FlightDate { get; set; } 
        public int FlightHours { get; set; }
        [Display(Name= "W3W Location Name")]
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public string UserId { get; set; }
        public Guid Uav { get; set; }
        public string WindSpeed { get; set; }
        public string Temperature { get; set; }
        public string Visibility { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public Boolean Flown { get; set; }
        [Display(Name = "Flown Date")]
        [DataType(DataType.Date)]
        public DateTime? FlownDate { get; set; }
        public double MaxRisk { get; set; }
        public double AvgRisk { get; set; }
        [Range(10, 1000)]
        public int Radius { get; set; }
        //public string LatLonList { get; set; }

        public string LocationMap()
        {
            return "https://map.what3words.com/" + Location;
        }
    }
    public class FlighDataEdit
    {
        public Guid Id { get; set; }
        public string FlightName { get; set; }

        [Display(Name = "Flight Date")]
        [DataType(DataType.Date)]
        public DateTime? FlightDate{get;set;}
        public int FlightHours { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public string WindSpeed { get; set; }
        public string Temperature { get; set; }
        public string Visibility { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        [Range(10,1000)]
        public int Radius { get; set; }
        public double MaxRisk { get; set; }
        public double AvgRisk { get; set; }
        public string LatLonListGround { get; set; }
        public string LatLonListAir { get; set; }

        public void MapToModel(FlightData flight)
        {
            flight.FlightName = FlightName;
            flight.FlightDate = FlightDate;
            flight.FlightHours = FlightHours;
            flight.Location = GetLocationW3W(Latitude, Longitude);
            flight.Latitude = Latitude;
            flight.Longitude = Longitude;
            flight.Elevation = Elevation;
            flight.WindSpeed = WindSpeed;
            flight.Temperature = Temperature;
            flight.Visibility = Visibility;
            flight.Sunrise = Sunrise;
            flight.Sunset = Sunset;
            flight.Radius = Radius;
            flight.MaxRisk = MaxRisk;
            flight.AvgRisk = AvgRisk;
        }

        public string GetLocationW3W(double lat, double lon)
        {
            What3WordsService myW3WService =  new What3WordsService("W59GIYRH", LanguageCode.EN);
            var myLocationCall = myW3WService.GetReverseGeocodingAsync(lat, lon);
            string myLocation = myLocationCall.Result.Words;
            return myLocation;
        }
    }

}
