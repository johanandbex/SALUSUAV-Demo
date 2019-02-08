﻿using System;

namespace SALUSUAV_Demo.Extensions {

    /// <summary>
    /// A geograpic coordinate with a latitude and longitude
    /// </summary>
    public class GeoCoordinate : ICoordinate
    {
        #region Public Fields

        /// <summary>
        /// The estimated radius of Earth in miles
        /// </summary>
        public const double EarthRadiusInMiles = 3956;

        /// <summary>
        /// The estimated radius of Earth in kilometers
        /// </summary>
        public const double EarthRadiusInKilometers = 6367;

        #endregion

        #region Public Properties

        /// <summary>
        /// The GeoCoordinate Latitude
        /// </summary>
        public Latitude Latitude { get; }

        /// <summary>
        /// The GeoCoordinate Longitude
        /// </summary>
        public Longitude Longitude { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new GeoCoordinate
        /// </summary>
        /// <param name="latitude">The coordinate latitude</param>
        /// <param name="longitude">The coordinate longitude</param>
        public GeoCoordinate(Latitude latitude, Longitude longitude)
        {
            Latitude = latitude ?? throw new ArgumentNullException(nameof(latitude));
            Longitude = longitude ?? throw new ArgumentNullException(nameof(longitude));
        }

        /// <summary>
        /// Creates a new GeoCoordinate
        /// </summary>
        /// <param name="latitude">The latitude in decimal degrees</param>
        /// <param name="longitude">The longitude in decimal degrees</param>
        public GeoCoordinate(double latitude, double longitude)
        {
            Latitude = new Latitude(latitude);
            Longitude = new Longitude(longitude);
        }

        #endregion

        #region Public Methods

        #region Interface Methods

        /// <summary>
        /// Gets the latitude
        /// </summary>
        /// <returns>Latitude</returns>
        public Latitude GetLatitude()
        {
            return Latitude;
        }

        /// <summary>
        /// Gets the longitude
        /// </summary>
        /// <returns>Longitude</returns>
        public Longitude GetLongitude()
        {
            return Longitude;
        }

        #endregion

        #region Haversine Bearing

        /// <summary>
        /// Gets the initial bearing in degrees to another geographic point from this 
        /// geographic coordinate based on the Haversine formula
        /// </summary>
        /// <param name="latitude">The latitude of the destination in degrees</param>
        /// <param name="longitude">The longitude of the destination in degrees</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public double InitialBearingTo(double latitude, double longitude)
        {
            return GetInitialBearing(this, new GeoCoordinate(latitude, longitude));
        }

        /// <summary>
        /// Gets the initial bearing in degrees to another geographic point from this 
        /// geographic coordinate based on the Haversine formula
        /// </summary>
        /// <param name="destination">The geographic coordinate of the destination</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public double InitialBearingTo(ICoordinate destination)
        {
            return GetInitialBearing(this, destination);
        }

        /// <summary>
        /// Gets the initial bearing in degrees to another geographic point from this 
        /// geographic coordinate based on the Haversine formula
        /// </summary>
        /// <param name="latitude">The latitude of the destination</param>
        /// <param name="longitude">The longitude of the destination</param>
        /// <returns></returns>
        public double InitialBearingTo(Latitude latitude, Longitude longitude)
        {
            return GetInitialBearing(this, new GeoCoordinate(latitude, longitude));
        }

        /// <summary>
        /// Gets the initial bearing from one latitude/longitude coordinate to another based on the Haversine formula
        /// </summary>
        /// <param name="lat1">The source latitude in degrees</param>
        /// <param name="lon1">The source longitude in degrees</param>
        /// <param name="lat2">The destination latitude in degrees</param>
        /// <param name="lon2">The destination longitude in degrees</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public static double GetInitialBearing(double lat1, double lon1, double lat2, double lon2)
        {
            return GetInitialBearing(new GeoCoordinate(lat1, lon1), new GeoCoordinate(lat2, lon2));
        }

        /// <summary>
        /// Gets the initial bearing from one latitude/longitude coordinate to another based on the Haversine formula
        /// </summary>
        /// <param name="lat1">The source latitude</param>
        /// <param name="lon1">The source longitude</param>
        /// <param name="lat2">The destination latitude</param>
        /// <param name="lon2">The destination longitude</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public static double GetInitialBearing(Latitude lat1, Longitude lon1, Latitude lat2, Longitude lon2)
        {
            return GetInitialBearing(new GeoCoordinate(lat1, lon1), new GeoCoordinate(lat2, lon2));
        }

        /// <summary>
        /// Gets the initial bearing from one geocoordinate to another based on the Haversine formula
        /// </summary>
        /// <param name="source">The source coordinate</param>
        /// <param name="destination">The destination coordinate</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public static double GetInitialBearing(ICoordinate source, ICoordinate destination)
        {
            double lat1 = DegreeToRadian(source.GetLatitude().DecimalDegrees);
            double lat2 = DegreeToRadian(destination.GetLatitude().DecimalDegrees);
            double deltaLongitude = DegreeToRadian(destination.GetLongitude().DecimalDegrees) - DegreeToRadian(source.GetLongitude().DecimalDegrees);

            double y = Math.Cos(lat2) * Math.Sin(deltaLongitude);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLongitude);

            // Multiply degrees per radian
            return (RadianToDegree(Math.Atan2(y, x)) + 360) % 360;
        }

        #endregion

        #region Rhumb Bearing

        /// <summary>
        /// Gets the bearing in degrees to another geographic point from this 
        /// geographic coordinate based on the Rhumb formula
        /// </summary>
        /// <param name="latitude">The latitude of the destination in degrees</param>
        /// <param name="longitude">The longitude of the destination in degrees</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public double RhumbBearingTo(double latitude, double longitude)
        {
            return GetRhumbBearing(this, new GeoCoordinate(latitude, longitude));
        }

        /// <summary>
        /// Gets the bearing in degrees to another geographic point from this 
        /// geographic coordinate based on the Rhumb formula
        /// </summary>
        /// <param name="latitude">The destination latitude</param>
        /// <param name="longitude">The destination longitude</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public double RhumbBearingTo(Latitude latitude, Longitude longitude)
        {
            return GetRhumbBearing(this, new GeoCoordinate(latitude, longitude));
        }

        /// <summary>
        /// Gets the bearing in degrees to another geographic point from this 
        /// geographic coordinate based on the Rhumb formula
        /// </summary>
        /// <param name="destination">The geographic coordinate of the destination</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public double RhumbBearingTo(ICoordinate destination)
        {
            return GetRhumbBearing(this, destination);
        }

        /// <summary>
        /// Gets the bearing from one latitude/longitude coordinate to another based on the Rhumb formula
        /// </summary>
        /// <param name="lat1">The source latitude in degrees</param>
        /// <param name="lon1">The source longitude in degrees</param>
        /// <param name="lat2">The destination latitude in degrees</param>
        /// <param name="lon2">The destination longitude in degrees</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public static double GetRhumbBearing(double lat1, double lon1, double lat2, double lon2)
        {
            return GetRhumbBearing(new GeoCoordinate(lat1, lon1), new GeoCoordinate(lat2, lon2));
        }

        /// <summary>
        /// Gets the bearing from one latitude/longitude coordinate to another based on the Rhumb formula
        /// </summary>
        /// <param name="lat1">The source latitude</param>
        /// <param name="lon1">The source longitude</param>
        /// <param name="lat2">The destination latitude</param>
        /// <param name="lon2">The destination longitude</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public static double GetRhumbBearing(Latitude lat1, Longitude lon1, Latitude lat2, Longitude lon2)
        {
            return GetRhumbBearing(new GeoCoordinate(lat1, lon1), new GeoCoordinate(lat2, lon2));
        }

        /// <summary>
        /// Gets the bearing from one latitude/longitude coordinate to another based on the Rhumb formula
        /// </summary>
        /// <param name="source">The source coordinate</param>
        /// <param name="destination">The destination coordinate</param>
        /// <returns>The bearing from source to destination in degrees</returns>
        public static double GetRhumbBearing(ICoordinate source, ICoordinate destination)
        {
            double lat1 = DegreeToRadian(source.GetLatitude().DecimalDegrees);
            double lat2 = DegreeToRadian(destination.GetLatitude().DecimalDegrees); 
            double deltaLongitude = DegreeToRadian(destination.GetLongitude().DecimalDegrees - source.GetLongitude().DecimalDegrees);

            double x = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));

            if (Math.Abs(deltaLongitude) > Math.PI)
            {
                deltaLongitude = (deltaLongitude > 0) ? -(2 * Math.PI - deltaLongitude) : (2 * Math.PI + deltaLongitude);
            }

            double bearing = Math.Atan2(deltaLongitude, x);

            return (RadianToDegree(bearing) + 360) % 360;
        }

        #endregion

        #region Haversine Distance

        /// <summary>
        /// Gets the distance between two coordinates using the Haversine formula
        /// </summary>
        /// <param name="lat1">The source latitude in degrees</param>
        /// <param name="lon1">The source longitude in degrees</param>
        /// <param name="lat2">The destination latitude in degrees</param>
        /// <param name="lon2">The destination longitude in degrees</param>
        /// <param name="distanceType">The unit of measure for the distance, this defaults to KILOMETERS</param>
        /// <returns>The distance between the two coordinates</returns>
        public static double GetDistance(double lat1, double lon1, double lat2, double lon2, DistanceType distanceType = DistanceType.Kilometers)
        {
            return GetDistance(new GeoCoordinate(lat1, lon1), new GeoCoordinate(lat2, lon2), distanceType);
        }

        /// <summary>
        /// Gets the distance between two coordinates using the Haversine formula
        /// </summary>
        /// <param name="lat1">The source latitude</param>
        /// <param name="lon1">The source longitude</param>
        /// <param name="lat2">The destination latitude</param>
        /// <param name="lon2">The destination longitude</param>
        /// <returns>The distance between the two coordinates</returns>
        public static double GetDistance(Latitude lat1, Longitude lon1, Latitude lat2, Longitude lon2)
        {
            return GetDistance(new GeoCoordinate(lat1, lon1), new GeoCoordinate(lat2, lon2));
        }

        /// <summary>
        /// Gets the distance between two coordinates using the Haversine formula
        /// </summary>
        /// <param name="source">The source coordinate</param>
        /// <param name="destination">The destination coordinate</param>
        /// <param name="distanceType">The unit of measure for the distance, this defaults to KILOMETERS</param>
        /// <returns>The distance between the two coordinates</returns>
        public static double GetDistance(ICoordinate source, ICoordinate destination, DistanceType distanceType = DistanceType.Kilometers)
        {
            double radius = 0;

            switch (distanceType)
            {
                case DistanceType.Kilometers:
                    {
                        radius = EarthRadiusInKilometers;
                        break;
                    }
                case DistanceType.Meters:
                    {
                        radius = EarthRadiusInKilometers * 1000;
                        break;
                    }
                case DistanceType.Miles:
                    {
                        radius = EarthRadiusInMiles;
                        break;
                    }
            }

            double deltaLatitude = DegreeToRadian(destination.GetLatitude().DecimalDegrees - source.GetLatitude().DecimalDegrees);
            double deltaLongitude = DegreeToRadian(destination.GetLongitude().DecimalDegrees - source.GetLongitude().DecimalDegrees);
            double a = Math.Pow(Math.Sin(deltaLatitude / 2), 2) +
                Math.Cos(DegreeToRadian(source.GetLatitude().DecimalDegrees)) * Math.Cos(DegreeToRadian(destination.GetLatitude().DecimalDegrees)) *
                Math.Pow(Math.Sin(deltaLongitude / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = c * radius;

            return distance;
        }

        /// <summary>
        /// Gets the distance from this coordinate to another geocoordinate using the Haversine formula
        /// </summary>
        /// <param name="latitude">The destination latitude in degrees</param>
        /// <param name="longitude">The destination longitude in degrees</param>
        /// <param name="distanceType">The distance type the results will be returned in, i.e. kilometers</param>
        /// <returns>The distance to the destination</returns>
        public double DistanceTo(double latitude, double longitude, DistanceType distanceType)
        {
            return GetDistance(this, new GeoCoordinate(latitude, longitude), distanceType);
        }

        /// <summary>
        /// Gets the distance from this coordinate to another geocoordinate using the Haversine formula
        /// </summary>
        /// <param name="latitude">The destination latitude</param>
        /// <param name="longitude">The destination longitude</param>
        /// <param name="distanceType">The distance type the results will be returned in, i.e. kilometers</param>
        /// <returns>The distance to the destination</returns>
        public double DistanceTo(Latitude latitude, Longitude longitude, DistanceType distanceType)
        {
            return GetDistance(this, new GeoCoordinate(latitude, longitude), distanceType);
        }

        /// <summary>
        /// Gets the distance from this coordinate to another geocoordinate using the Haversine formula
        /// </summary>
        /// <param name="destination">The destination</param>
        /// <param name="distanceType">The distance type the results will be returned in, i.e. kilometers</param>
        /// <returns>The distance to the destination</returns>
        public double DistanceTo(ICoordinate destination, DistanceType distanceType)
        {
            return GetDistance(this, destination, distanceType);
        }

        #endregion

        #region Rhumb Distance

        /// <summary>
        /// Gets the distance from this coordinate to another geocoordinate using a rhumb line
        /// </summary>
        /// <param name="latitude">The destination latitude in degrees</param>
        /// <param name="longitude">The destination longitude in degrees</param>
        /// <param name="distanceType">The distance type the results will be returned in, i.e. kilometers</param>
        /// <returns>The distance to the destination</returns>
        public double RhumbDistanceTo(double latitude, double longitude, DistanceType distanceType)
        {
            return GetRhumbDistance(this, new GeoCoordinate(latitude, longitude), distanceType);
        }

        /// <summary>
        /// Gets the distance from this coordinate to another geocoordinate using a rhumb line
        /// </summary>
        /// <param name="latitude">The destination latitude</param>
        /// <param name="longitude">The destination longitude</param>
        /// <param name="distanceType">The distance type the results will be returned in, i.e. kilometers</param>
        /// <returns>The distance to the destination</returns>
        public double RhumbDistanceTo(Latitude latitude, Longitude longitude, DistanceType distanceType)
        {
            return GetRhumbDistance(this, new GeoCoordinate(latitude, longitude), distanceType);
        }

        /// <summary>
        /// Gets the distance from this coordinate to another geocoordinate using a rhumb line
        /// </summary>
        /// <param name="destination">The destination coordinate</param>
        /// <param name="distanceType">The distance type the results will be returned in, i.e. kilometers</param>
        /// <returns>The distance to the destination</returns>
        public double RhumbDistanceTo(ICoordinate destination, DistanceType distanceType)
        {
            return GetRhumbDistance(this, destination, distanceType);
        }

        /// <summary>
        /// Gets the distance between two coordinates using a rhumb line
        /// </summary>
        /// <param name="lat1">The source latitude in degrees</param>
        /// <param name="lon1">The source longitude in degrees</param>
        /// <param name="lat2">The destination latitude in degrees</param>
        /// <param name="lon2">The destination longitude in degrees</param>
        /// <param name="distanceType">The unit of measure for the distance, this defaults to KILOMETERS</param>
        /// <returns>The distance between the two coordinates</returns>
        public static double GetRhumbDistance(double lat1, double lon1, double lat2, double lon2, DistanceType distanceType = DistanceType.Kilometers)
        {
            return GetRhumbDistance(new GeoCoordinate(lat1, lon1), new GeoCoordinate(lat2, lon2), distanceType);
        }

        /// <summary>
        /// Gets the distance between two coordinates using a rhumb line
        /// </summary>
        /// <param name="lat1">The source latitude</param>
        /// <param name="lon1">The source longitude</param>
        /// <param name="lat2">The destination latitude</param>
        /// <param name="lon2">The destination longitude</param>
        /// <param name="distanceType">The unit of measure for the distance, this defaults to KILOMETERS</param>
        /// <returns>The distance between the two coordinates</returns>
        public static double GetRhumbDistance(Latitude lat1, Longitude lon1, Latitude lat2, Longitude lon2, DistanceType distanceType)
        {
            return GetRhumbDistance(new GeoCoordinate(lat1, lon1), new GeoCoordinate(lat2, lon2), distanceType);
        }

        /// <summary>
        /// Gets the distance between two coordinates using a rhumb line
        /// </summary>
        /// <param name="source">The source coordinate</param>
        /// <param name="destination">The destination coordinate</param>
        /// <param name="distanceType">The unit of measure for the distance, this defaults to KILOMETERS</param>
        /// <returns>The distance between the two coordinates</returns>
        public static double GetRhumbDistance(ICoordinate source, ICoordinate destination, DistanceType distanceType = DistanceType.Kilometers)
        {
            double radius = 0;

            switch (distanceType)
            {
                case DistanceType.Kilometers:
                    {
                        radius = EarthRadiusInKilometers;
                        break;
                    }
                case DistanceType.Meters:
                    {
                        radius = EarthRadiusInKilometers * 1000;
                        break;
                    }
                case DistanceType.Miles:
                    {
                        radius = EarthRadiusInMiles;
                        break;
                    }
            }

            double lat1 = DegreeToRadian(source.GetLatitude().DecimalDegrees);
            double lat2 = DegreeToRadian(destination.GetLatitude().DecimalDegrees);
            double deltaLatitude = DegreeToRadian(destination.GetLatitude().DecimalDegrees - source.GetLatitude().DecimalDegrees);
            double deltaLongitude = DegreeToRadian(Math.Abs(destination.GetLongitude().DecimalDegrees - source.GetLongitude().DecimalDegrees));

            double deltaPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
            double q = Math.Cos(lat1);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (deltaPhi != 0)
            {
                q = deltaLatitude / deltaPhi;  // E-W line gives DeltaPhi=0
            }

            // if DeltaLongitude over 180° take shorter rhumb across 180° meridian
            if (deltaLongitude > Math.PI)
            {
                deltaLongitude = 2 * Math.PI - deltaLongitude;
            }

            double distance = Math.Sqrt(Math.Pow(deltaLatitude, 2) + Math.Pow(q, 2) * Math.Pow(deltaLongitude, 2)) * radius;

            return distance;
        }

        #endregion

        #region Haversine Destination

        /// <summary>
        /// Finds the destination geocoordinate given a source coordinate, bearing, and distance.
        /// </summary>
        /// <param name="latitude">The source latitude in degrees</param>
        /// <param name="longitude">The source longitude in degrees</param>
        /// <param name="bearing">The direction of travel from the source in degrees</param>
        /// <param name="distance">The distance traveled</param>
        /// <param name="distanceType">The unit of measure that the distance is provided in</param>
        /// <returns>The destination geocoordinate</returns>
        public static ICoordinate FindDestination(double latitude, double longitude, double bearing, double distance, DistanceType distanceType)
        {
            return FindDestination(new GeoCoordinate(latitude, longitude), bearing, distance, distanceType);
        }

        /// <summary>
        /// Finds the destination geocoordinate given a source coordinate, bearing, and distance.
        /// </summary>
        /// <param name="latitude">The source latitude</param>
        /// <param name="longitude">The source longitude</param>
        /// <param name="bearing">The direction of travel from the source in degrees</param>
        /// <param name="distance">The distance traveled</param>
        /// <param name="distanceType">The unit of measure that the distance is provided in</param>
        /// <returns>The destination geocoordinate</returns>
        public static ICoordinate FindDestination(Latitude latitude, Longitude longitude, double bearing, double distance, DistanceType distanceType)
        {
            return FindDestination(new GeoCoordinate(latitude, longitude), bearing, distance, distanceType);
        }

        /// <summary>
        /// Finds the destination geocoordinate given a source coordinate, bearing, and distance
        /// </summary>
        /// <param name="source">The source coordinate</param>
        /// <param name="bearing">The direction of travel from the source in degrees</param>
        /// <param name="distance">The distance traveled</param>
        /// <param name="distanceType">The unit of measure that the distance is provided in</param>
        /// <returns>The destination geocoordinate</returns>
        public static ICoordinate FindDestination(ICoordinate source, double bearing, double distance, DistanceType distanceType)
        {
            double radius = GetRadius(distanceType);

            double latitude = DegreeToRadian(source.GetLatitude().DecimalDegrees);
            double longitude = DegreeToRadian(source.GetLongitude().DecimalDegrees);
            bearing = DegreeToRadian(bearing);

            double destinationLatitude = RadianToDegree(Math.Asin(
                    Math.Sin(latitude) * Math.Cos(distance / radius) +
                    Math.Cos(latitude) * Math.Sin(distance / radius) * Math.Cos(bearing)
                ));

            double destinationLongitude = RadianToDegree(longitude + Math.Atan2(
                    Math.Sin(bearing) * Math.Sin(distance / radius) * Math.Cos(latitude),
                    Math.Cos(distance / radius) - Math.Sin(latitude) * Math.Sin(DegreeToRadian(destinationLatitude))
                ));

            return new GeoCoordinate(destinationLatitude, ((destinationLongitude + 540) % 360) - 180);
        }

        /// <summary>
        /// Finds the destination geocoordinate using the bearing and distance travelled from this coordinate
        /// </summary>
        /// <param name="bearing">The direction of travel from the source in degrees</param>
        /// <param name="distance">The distance traveled</param>
        /// <param name="distanceType">The unit of measure that the distance is provided in</param>
        /// <returns>The destination geocoordinate</returns>
        public ICoordinate FindDestination(double bearing, double distance, DistanceType distanceType)
        {
            return FindDestination(this, bearing, distance, distanceType);
        }

        #endregion

        #region Rhumb Destination

        /// <summary>
        /// Finds the destination from a source using a rhumb line
        /// </summary>
        /// <param name="latitude">The source latitude in degrees</param>
        /// <param name="longitude">The source longitude in degrees</param>
        /// <param name="bearing">The bearing to the destination in degrees</param>
        /// <param name="distance">The distance away from the source</param>
        /// <param name="distanceType">The unit of measure that the distance is provided in</param>
        /// <returns>The destination</returns>
        public static ICoordinate FindRhumbDestination(double latitude, double longitude, double bearing, double distance, DistanceType distanceType)
        {
            return FindRhumbDestination(new GeoCoordinate(latitude, longitude), bearing, distance, distanceType);
        }

        /// <summary>
        /// Finds the destination from a source using a rhumb line
        /// </summary>
        /// <param name="latitude">The source latitude</param>
        /// <param name="longitude">The source longitude</param>
        /// <param name="bearing">The bearing to the destination</param>
        /// <param name="distance">The distance away from the source</param>
        /// <param name="distanceType">The unit of measure that the distance is provided in</param>
        /// <returns>The destination</returns>
        public static ICoordinate FindRhumbDestination(Latitude latitude, Longitude longitude, double bearing, double distance, DistanceType distanceType)
        {
            return FindRhumbDestination(new GeoCoordinate(latitude, longitude), bearing, distance, distanceType);
        }

        /// <summary>
        /// Finds the destination from a source using a rhumb line
        /// </summary>
        /// <param name="source">The source coordinate</param>
        /// <param name="bearing">The bearing to the destination in degrees</param>
        /// <param name="distance">The distance away from the source</param>
        /// <param name="distanceType">The unit of measure that the distance is provided in</param>
        /// <returns>The destination</returns>
        public static ICoordinate FindRhumbDestination(ICoordinate source, double bearing, double distance, DistanceType distanceType)
        {
            double distanceRatio = distance / GetRadius(distanceType);

            double sourceLatitude = DegreeToRadian(source.GetLatitude().DecimalDegrees);
            double sourceLongitude = DegreeToRadian(source.GetLongitude().DecimalDegrees);
            bearing = DegreeToRadian(bearing);

            double destinationLatitude = sourceLatitude + (distanceRatio * Math.Cos(bearing));

            double delta = Math.Log(Math.Tan((Math.PI / 4) + (destinationLatitude / 2)) / Math.Tan((Math.PI / 4) + (sourceLatitude / 2)));

            double q = (destinationLatitude - sourceLatitude) / delta;

            double shortestRoute = distanceRatio * Math.Sin(bearing) / q;

            double destinationLongitude = sourceLongitude + shortestRoute;

            return new GeoCoordinate(RadianToDegree(destinationLatitude), RadianToDegree(destinationLongitude));
        }

        /// <summary>
        /// Finds the destination from this coordinate using a rhumb line
        /// </summary>
        /// <param name="bearing">The bearing to the destination in degrees</param>
        /// <param name="distance">The distance away from the source</param>
        /// <param name="distanceType">The unit of measure that the distance is provided in</param>
        /// <returns>The destination</returns>
        public ICoordinate FindRhumbDestination(double bearing, double distance, DistanceType distanceType)
        {
            return FindRhumbDestination(this, bearing, distance, distanceType);
        }

        #endregion

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Converts a degree to radians
        /// </summary>
        /// <param name="degree">The degree to convert to radians</param>
        /// <returns>Radians</returns>
        public static double DegreeToRadian(double degree)
        {
            return degree * (Math.PI / 180);
        }

        /// <summary>
        /// Converts a radian to a degree measurement
        /// </summary>
        /// <param name="radian">The radian to convert to degrees</param>
        /// <returns>Degrees</returns>
        public static double RadianToDegree(double radian)
        {
            return radian * (180 / Math.PI);
        }

        /// <summary>
        /// Converts a degree minute second object to radians
        /// </summary>
        /// <param name="dms">The degree minute second coordinate to convert to radians</param>
        /// <returns>Radians</returns>
        public static double DmsToRadian(Coordinate dms)
        {
            return DegreeToRadian(dms.DecimalDegrees);
        }

        /// <summary>
        /// Gets the radius of the earth in the specified distance type
        /// </summary>
        /// <param name="distanceType">The distance unit to return the results in</param>
        /// <returns>The earth's radius</returns>
        protected static double GetRadius(DistanceType distanceType)
        {
            double radius = 0;

            switch (distanceType)
            {
                case DistanceType.Kilometers:
                    {
                        radius = EarthRadiusInKilometers;
                        break;
                    }
                case DistanceType.Meters:
                    {
                        radius = EarthRadiusInKilometers * 1000;
                        break;
                    }
                case DistanceType.Miles:
                    {
                        radius = EarthRadiusInMiles;
                        break;
                    }
            }

            return radius;
        }

        #endregion
    }
}
