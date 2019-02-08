using System;
using System.Collections.Generic;

namespace SALUSUAV_Demo.Extensions
{

    public class Tools
    {

        public static List<GeoCoordinate> NewLatLon(int riskNumberInRaw, double latIn, double lonIn)
        {

            List<GeoCoordinate> myGeoList = new List<GeoCoordinate>();
            GeoCoordinate source = new GeoCoordinate(latIn, lonIn);
            myGeoList.Add(source);
            return myGeoList;



        }

        public static List<GeoCoordinate> BuildListOfCoordinates(int radius, double latIn, double lonIn)
        {
            int len = radius / 25;
            List<GeoCoordinate> myGeoList = new List<GeoCoordinate>();
            GeoCoordinate source = new GeoCoordinate(latIn, lonIn);
            List<int> degrees = new List<int> { 0, 90, 180, 270 };

            foreach (var deg in degrees)
            {
                ICoordinate destination = source.FindDestination(deg, len, DistanceType.Meters);
                GeoCoordinate newCord = new GeoCoordinate(destination.GetLatitude(), destination.GetLongitude());
                double distance = newCord.DistanceTo(latIn, lonIn, DistanceType.Meters);
                if (distance <= radius)
                {
                    myGeoList.Add(newCord);
                }

            }

            return myGeoList;

        }

        public static List<int> AddToIntList(List<int> outGoingList, List<int> listIn, int repeat)
        {
            for (int i = 1; i <= repeat; i++)
            {
                listIn.Add(i);
            }
            int additional = listIn[0];
            foreach (int degIn in listIn)
            {
                additional = additional + degIn;
                outGoingList.Add(additional);
            }
            return outGoingList;
        }


    }
    class LatLonPair
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }



}
