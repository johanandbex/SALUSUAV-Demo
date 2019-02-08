using System;

namespace SALUSUAV_Demo.Models.UAVData
{
    public class UavData
    {
        public Guid Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int PurchaseYear { get; set; }
        public int MaxWindSpeed { get; set; }
        public int MaxAltitude { get; set; }
        public bool BeyondSightEnabled { get; set; }
        public bool SafetyMechanism { get; set; }
        public string UserId { get; set; }
        public int Weight { get; set; }

    }
}
