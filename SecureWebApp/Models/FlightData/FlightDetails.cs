using System;

namespace SALUSUAV_Demo.Models.FlightData { 
    public class FlightDetails
    {
        public Guid Id { get; set; }
        public Guid FlightId { get; set; }
        public string LatLonListGround { get; set; }
        public string LatLonListAir { get; set; }
        
    }
   
}
