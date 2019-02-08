using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using What3Words;
using System.Security.Claims;
using Newtonsoft.Json;
using SALUSUAV_Demo.Extensions;
using SALUSUAV_Demo.Models.FlightData;
using SALUSUAV_DEMO.Data;

namespace SALUSUAV_Demo.Controllers
{
    public class FlightDatasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FlightDatasController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<JsonResult> Get3Ug(string id, string latitudeIn, string longitudeIn, string radiusIn)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            //Guid.TryParse(id, out Guid idGuid);

            //var flightData = await _context.FlightData.SingleOrDefaultAsync(m => m.Id == idGuid);
            string url = "http://www.3ugas.com/query_REV_server.php?lat=" + latitudeIn + "&lng=" + longitudeIn + "&r=" + radiusIn + "&UAV_type=2&op=3";
            string res = await GetuoBData(url);
            return Json(res);
        }

        public async Task<JsonResult> GetLatLong(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            Guid.TryParse(id, out Guid idGuid);

            var flightData = await _context.FlightData.SingleOrDefaultAsync(m => m.Id == idGuid);
            var flightDetails = await _context.FlightDetails.SingleOrDefaultAsync(m => m.FlightId == idGuid);
            string longStringGround = "";
            string longStringAir = "";
            if (flightDetails != null)
            {
                longStringGround = flightDetails.LatLonListGround;
                longStringAir = flightDetails.LatLonListAir;
            }

            List<LatLonPair> latLonPairListGround = new List<LatLonPair>();
            List<LatLonPair> latLonPairListAir = new List<LatLonPair>();
            if (!string.IsNullOrEmpty(longStringGround))
            {
                string[] latlonstr = longStringGround.Split('/');
                foreach (var latlonSub in latlonstr)
                {
                    string[] latlon = latlonSub.Split('|');
                    if (latlon.Length > 1)
                    {
                        LatLonPair pair = new LatLonPair
                        {
                            Lat = Convert.ToDouble(latlon[0]),
                            Lon = Convert.ToDouble(latlon[1])
                        };
                        latLonPairListGround.Add(pair);
                    }
                }
            }
            if (!string.IsNullOrEmpty(longStringAir))
            {
                string[] latlonstr = longStringAir.Split('/');
                foreach (var latlonSub in latlonstr)
                {
                    string[] latlon = latlonSub.Split('|');
                    if (latlon.Length > 1)
                    {
                        LatLonPair pair = new LatLonPair
                        {
                            Lat = Convert.ToDouble(latlon[0]),
                            Lon = Convert.ToDouble(latlon[1])
                        };
                        latLonPairListAir.Add(pair);
                    }
                }
            }

            var returnedData = new
            {
                id,
                latitude = flightData.Latitude,
                longitude = flightData.Longitude,
                latlonListObjectGround = latLonPairListGround,
                latlonListObjectAir = latLonPairListAir,
                radius = flightData.Radius
            };
            return Json(returnedData);
        }



        // GET: FlightDatas
        public async Task<IActionResult> Index()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View(await _context.FlightData.Where(t => t.UserId == userId).ToListAsync());
        }


        // Quote or Fly
        // GET: FlightDatas/Edit/5
        public async Task<IActionResult> SetLocation(Guid? id, double lat, double lon)
        {
            var flightData = await _context.FlightData.SingleOrDefaultAsync(m => m.Id == id);
            if (flightData == null)
            {
                return NotFound();
            }

            flightData.Latitude = lat;
            flightData.Longitude = lon;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit) + "/" + flightData.Id);
            //return View(flightData);
        }

        // GET: FlightDatas/Create
        public IActionResult Create()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var query =
                from x in _context.UavData
                join p in _context.Users on x.UserId equals p.Id
                where x.UserId == userId
                select new { ID = x.Id, x.Model };

            SelectList uavsTuff = new SelectList(query, "ID", "Model");
            ViewData["UAV"] = uavsTuff;
            return View();
        }

        // POST: FlightDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FlightName,FlightDate,FlightHours,Location,Elevation, Uav")] FlightData flightData)
        {
            _context.Add(flightData);
            flightData.UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            FlightDetails flightDetails = await _context.FlightDetails.SingleOrDefaultAsync(m => m.FlightId == flightData.Id);
            if (flightDetails != null)
            {
                flightDetails.FlightId = flightData.Id;
            }
            else
            {
                FlightDetails flightDetailsNew = new FlightDetails();
                _context.Add(flightDetailsNew);
                flightDetailsNew.FlightId = flightData.Id;
            }

            //sort location
            var latitude = Request.Cookies["latitude"];
            var longitude = Request.Cookies["longitude"];
            var elevation = Request.Cookies["elevation"];
            string locSent = flightData.Location;
            if (!string.IsNullOrEmpty(locSent))
            {
                What3WordsService w3WService = new What3WordsService("W59GIYRH", What3Words.Enums.LanguageCode.EN);
                What3Words.Models.GeocodingRoot myW3WRoot = await w3WService.GetForewordGeocodingAsync(locSent);
                double latitudeOut = myW3WRoot.Bounds.Northeast.Lat;
                double longitudeOut = myW3WRoot.Bounds.Northeast.Lng;
                flightData.Location = locSent;
                flightData.Latitude = latitudeOut;
                flightData.Longitude = longitudeOut;
            }
            else
            {
                What3WordsService w3WService = new What3WordsService("W59GIYRH", What3Words.Enums.LanguageCode.EN);
                What3Words.Models.GeocodingRoot myW3WRoot = await w3WService.GetReverseGeocodingAsync(Convert.ToDouble(latitude), Convert.ToDouble(longitude));
                string location = myW3WRoot.Words;
                flightData.Location = location;
                flightData.Latitude = Convert.ToDouble(latitude);
                flightData.Longitude = Convert.ToDouble(longitude);
            }

            flightData.Elevation = Convert.ToDouble(elevation);
            flightData.Radius = 500;

            //get weather
            WeatherDetailsObject myWeatherObject = await WeatherDetailsObject.GetWeather(latitude, longitude);
            flightData.WindSpeed = myWeatherObject.Wind;
            flightData.Temperature = myWeatherObject.Temperature;
            flightData.Sunrise = myWeatherObject.Sunrise;
            flightData.Sunset = myWeatherObject.Sunset;
            flightData.Visibility = myWeatherObject.Visibility;
            //
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit) + "/" + flightData.Id);

        }

        // GET: FlightDatas/Edit/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightData = await _context.FlightData.SingleOrDefaultAsync(m => m.Id == id);
            if (flightData == null)
            {
                return NotFound();
            }
            var flighDetail = await _context.FlightDetails.SingleOrDefaultAsync(x => x.FlightId == flightData.Id);
            FlighDataEdit flighDataEdit = new FlighDataEdit
            {
                Id = flightData.Id,
                FlightName = flightData.FlightName,
                FlightDate = flightData.FlightDate,
                FlightHours = flightData.FlightHours,
                Location = flightData.Location,
                Latitude = flightData.Latitude,
                Longitude = flightData.Longitude,
                Elevation = flightData.Elevation,
                WindSpeed = flightData.WindSpeed,
                Temperature = flightData.Temperature,
                Visibility = flightData.Visibility,
                Sunrise = flightData.Sunrise,
                Sunset = flightData.Sunset,
                Radius = flightData.Radius,
                MaxRisk = flightData.MaxRisk,
                AvgRisk = flightData.AvgRisk,
                LatLonListGround = flighDetail.LatLonListGround,
                LatLonListAir = flighDetail.LatLonListAir
            };
            return View(flighDataEdit);
        }


        // GET: FlightDatas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightData = await _context.FlightData.SingleOrDefaultAsync(m => m.Id == id);
            if (flightData == null)
            {
                return NotFound();
            }
            var flighDetail = await _context.FlightDetails.SingleOrDefaultAsync(x => x.FlightId == flightData.Id);
            FlighDataEdit flighDataEdit = new FlighDataEdit
            {
                Id = flightData.Id,
                FlightName = flightData.FlightName,
                FlightDate = flightData.FlightDate,
                FlightHours = flightData.FlightHours,
                Location = flightData.Location,
                Latitude = flightData.Latitude,
                Longitude = flightData.Longitude,
                Elevation = flightData.Elevation,
                WindSpeed = flightData.WindSpeed,
                Temperature = flightData.Temperature,
                Visibility = flightData.Visibility,
                Sunrise = flightData.Sunrise,
                Sunset = flightData.Sunset,
                Radius = flightData.Radius,
                MaxRisk = flightData.MaxRisk,
                AvgRisk = flightData.AvgRisk,
                LatLonListGround = flighDetail.LatLonListGround,
                LatLonListAir = flighDetail.LatLonListAir
            };
            return View(flighDataEdit);
        }


        // POST: FlightDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Location,Latitude,Longitude,Elevation,Radius,UserId, MaxRisk, AvgRisk")] FlighDataEdit flightDataEdit)
        {
            if (id != flightDataEdit.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(flightDataEdit);
            var flightData = await _context.FlightData.SingleOrDefaultAsync(m => m.Id == id);
            try
            {
                flightDataEdit.FlightName = flightData.FlightName;
                flightDataEdit.FlightDate = flightData.FlightDate;
                flightDataEdit.FlightHours = flightData.FlightHours;
                flightDataEdit.WindSpeed = flightData.WindSpeed;
                flightDataEdit.Temperature = flightData.Temperature;
                flightDataEdit.Visibility = flightData.Visibility;
                flightDataEdit.Sunrise = flightData.Sunrise;
                flightDataEdit.Sunset = flightData.Sunset;
                flightDataEdit.MapToModel(flightData);
                _context.Update(flightData);
                await _context.SaveChangesAsync();
                await QuoteFly(id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightDataExists(flightDataEdit.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Edit) + "/" + flightDataEdit.Id);
        }



        // POST: FlightDatas/QuoteFly/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuoteFly(Guid id)
        {

            var flightData = await _context.FlightData.SingleOrDefaultAsync(m => m.Id == id);
            if (flightData == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //just set some values for display
                flightData.Flown = true;
                flightData.FlownDate = DateTime.Now;
                var flighDetail = await _context.FlightDetails.SingleOrDefaultAsync(x => x.FlightId == flightData.Id);
                flighDetail.LatLonListGround = "";
                flighDetail.LatLonListAir = "";
                string url = "http://www.3ugas.com/query_REV_server.php?lat=" + flightData.Latitude + "&lng=" + flightData.Longitude + "&r=" + flightData.Radius + "&UAV_type=2&op=3";
                string res = await GetuoBData(url);


                //prepare a datatable to accept data
                DataTable riskNodesMapGround = new DataTable();
                riskNodesMapGround.Columns.Add("nodeLat", typeof(double));
                riskNodesMapGround.Columns.Add("nodeLon", typeof(double));
                riskNodesMapGround.Columns.Add("nodeRisk", typeof(int));
                riskNodesMapGround.Columns.Add("nodeWeight", typeof(int));

                //prepare a datatable to accept data
                DataTable riskNodesMapAir = new DataTable();
                riskNodesMapAir.Columns.Add("nodeLat", typeof(double));
                riskNodesMapAir.Columns.Add("nodeLon", typeof(double));
                riskNodesMapAir.Columns.Add("nodeRisk", typeof(int));
                riskNodesMapAir.Columns.Add("nodeWeight", typeof(int));


                if (!string.IsNullOrEmpty(res))
                {
                    dynamic results = JsonConvert.DeserializeObject<dynamic>(res);
                    var mapList = results["REV_map"];
                    foreach (var latLOnStrings in mapList)
                    {
                        double airRisk = (double)latLOnStrings["air"];
                        double groundRisk = (double)latLOnStrings["gnd"];
                        string lat = latLOnStrings["lat"];
                        string lng = latLOnStrings["lng"];
                        if (airRisk > 0.005)
                        {

                            DataRow row = riskNodesMapAir.NewRow();
                            row["nodeLat"] = (double)Decimal.Parse(lat);
                            row["nodeLon"] = (double)Decimal.Parse(lng);
                            row["nodeRisk"] = groundRisk;
                            row["nodeWeight"] = 1;
                            riskNodesMapAir.Rows.Add(row);
                        }
                        if (groundRisk > 0)
                        {
                            DataRow row = riskNodesMapGround.NewRow();
                            row["nodeLat"] = (double)Decimal.Parse(lat);
                            row["nodeLon"] = (double)Decimal.Parse(lng);
                            row["nodeRisk"] = groundRisk;
                            row["nodeWeight"] = 1;
                            riskNodesMapGround.Rows.Add(row);
                        }
                    }
                }






                if (riskNodesMapGround.Rows.Count > 0)
                {
                    List<string> latLonList = new List<string>();
                    //turn the node list pairs into a string
                    //would be better as not string but cant find a way to pass that to javascript....ideas?
                    foreach (DataRow item in riskNodesMapGround.Rows)
                    {

                        double latDbl = (double)item["nodeLat"];
                        double lonDbl = (double)item["nodeLon"];
                        string latInStr = latDbl.ToString(CultureInfo.InvariantCulture);
                        string lonInStr = lonDbl.ToString(CultureInfo.InvariantCulture);
                        string all = latInStr + "|" + lonInStr;
                        latLonList.Add(all);
                    }
                    flighDetail.LatLonListGround = String.Join("/", latLonList);
                }

                if (riskNodesMapAir.Rows.Count > 0)
                {
                    List<string> latLonList = new List<string>();
                    //turn the node list pairs into a string
                    //would be better as not string but cant find a way to pass that to javascript....ideas?
                    foreach (DataRow item in riskNodesMapAir.Rows)
                    {

                        double latDbl = (double)item["nodeLat"];
                        double lonDbl = (double)item["nodeLon"];
                        string latInStr = latDbl.ToString(CultureInfo.InvariantCulture);
                        string lonInStr = lonDbl.ToString(CultureInfo.InvariantCulture);
                        string all = latInStr + "|" + lonInStr;
                        latLonList.Add(all);
                    }
                    flighDetail.LatLonListAir = String.Join("/", latLonList);
                }

                Int32 maxRiskGround = 0;
                Int32 maxRiskAir = 0;

                if (riskNodesMapGround.Rows.Count > 0)
                {
                    maxRiskGround = Convert.ToInt32(riskNodesMapGround.Compute("max([nodeRisk])", string.Empty));
                }
                if (riskNodesMapAir.Rows.Count > 0)
                {
                    maxRiskAir = Convert.ToInt32(riskNodesMapAir.Compute("max([nodeRisk])", string.Empty));
                }

                Int32 avgRiskGround = 0;
                Int32 avgRiskAir = 0;

                if (riskNodesMapGround.Rows.Count > 0)
                {
                    avgRiskGround = Convert.ToInt32(riskNodesMapGround.Compute("avg([nodeRisk])", string.Empty));
                }
                if (riskNodesMapAir.Rows.Count > 0)
                {
                    avgRiskAir = Convert.ToInt32(riskNodesMapAir.Compute("avg([nodeRisk])", string.Empty));
                }

                flightData.MaxRisk = Math.Max(maxRiskAir, maxRiskGround);
                flightData.AvgRisk = Math.Max(avgRiskAir, avgRiskGround);
                riskNodesMapGround.Dispose();
                riskNodesMapAir.Dispose();

                _context.Update(flightData);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Edit) + "/" + flightData.Id);
            }
            return RedirectToAction(nameof(Edit) + "/" + flightData.Id);
        }




        public async Task<IActionResult> UpdateData(String id, string lattitude, string lonitude, string location)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            Guid.TryParse(id, out Guid idGuid);
            var flightData = await _context.FlightData.SingleOrDefaultAsync(m => m.Id == idGuid);
            if (flightData == null)
            {
                return NotFound();
            }

            flightData.Latitude = double.Parse(lattitude);
            flightData.Longitude = double.Parse(lonitude);
            flightData.Location = location;
            //just set some values for display
            flightData.Flown = true;
            flightData.FlownDate = DateTime.Now;
            var flighDetail = await _context.FlightDetails.SingleOrDefaultAsync(x => x.FlightId == flightData.Id);
            flighDetail.LatLonListGround = "";
            flighDetail.LatLonListAir = "";
            string url = "http://www.3ugas.com/query_REV_server.php?lat=" + flightData.Latitude + "&lng=" + flightData.Longitude + "&r=" + flightData.Radius + "&UAV_type=2&op=3";
            string res = await GetuoBData(url);

            //prepare a datatable to accept data
            DataTable riskNodesMapGround = new DataTable();
            riskNodesMapGround.Columns.Add("nodeLat", typeof(double));
            riskNodesMapGround.Columns.Add("nodeLon", typeof(double));
            riskNodesMapGround.Columns.Add("nodeRisk", typeof(int));
            riskNodesMapGround.Columns.Add("nodeWeight", typeof(int));

            //prepare a datatable to accept data
            DataTable riskNodesMapAir = new DataTable();
            riskNodesMapAir.Columns.Add("nodeLat", typeof(double));
            riskNodesMapAir.Columns.Add("nodeLon", typeof(double));
            riskNodesMapAir.Columns.Add("nodeRisk", typeof(int));
            riskNodesMapAir.Columns.Add("nodeWeight", typeof(int));

            if (!string.IsNullOrEmpty(res))
            {
                dynamic results = JsonConvert.DeserializeObject<dynamic>(res);
                var mapList = results["REV_map"];
                foreach (var latLOnStrings in mapList)
                {
                    double airRisk = (double)latLOnStrings["air"];
                    double groundRisk = (double)latLOnStrings["gnd"];
                    string lat = latLOnStrings["lat"];
                    string lng = latLOnStrings["lng"];
                    if (airRisk > 0.005)
                    {

                        DataRow row = riskNodesMapAir.NewRow();
                        row["nodeLat"] = (double)Decimal.Parse(lat);
                        row["nodeLon"] = (double)Decimal.Parse(lng);
                        row["nodeRisk"] = groundRisk;
                        row["nodeWeight"] = 1;
                        riskNodesMapAir.Rows.Add(row);
                    }
                    if (groundRisk > 0)
                    {
                        DataRow row = riskNodesMapGround.NewRow();
                        row["nodeLat"] = (double)Decimal.Parse(lat);
                        row["nodeLon"] = (double)Decimal.Parse(lng);
                        row["nodeRisk"] = groundRisk;
                        row["nodeWeight"] = 1;
                        riskNodesMapGround.Rows.Add(row);
                    }
                }
            }

            if (riskNodesMapGround.Rows.Count > 0)
            {
                List<string> latLonList = new List<string>();
                //turn the node list pairs into a string
                //would be better as not string but cant find a way to pass that to javascript....ideas?
                foreach (DataRow item in riskNodesMapGround.Rows)
                {

                    double latDbl = (double)item["nodeLat"];
                    double lonDbl = (double)item["nodeLon"];
                    string latInStr = latDbl.ToString(CultureInfo.InvariantCulture);
                    string lonInStr = lonDbl.ToString(CultureInfo.InvariantCulture);
                    string all = latInStr + "|" + lonInStr;
                    latLonList.Add(all);
                }
                flighDetail.LatLonListGround = String.Join("/", latLonList);
            }

            if (riskNodesMapAir.Rows.Count > 0)
            {
                List<string> latLonList = new List<string>();
                //turn the node list pairs into a string
                //would be better as not string but cant find a way to pass that to javascript....ideas?
                foreach (DataRow item in riskNodesMapAir.Rows)
                {

                    double latDbl = (double)item["nodeLat"];
                    double lonDbl = (double)item["nodeLon"];
                    string latInStr = latDbl.ToString(CultureInfo.InvariantCulture);
                    string lonInStr = lonDbl.ToString(CultureInfo.InvariantCulture);
                    string all = latInStr + "|" + lonInStr;
                    latLonList.Add(all);
                }
                flighDetail.LatLonListAir = String.Join("/", latLonList);
            }

            Int32 maxRiskGround = 0;
            Int32 maxRiskAir = 0;

            if (riskNodesMapGround.Rows.Count > 0)
            {
                maxRiskGround = Convert.ToInt32(riskNodesMapGround.Compute("max([nodeRisk])", string.Empty));
            }
            if (riskNodesMapAir.Rows.Count > 0)
            {
                maxRiskAir = Convert.ToInt32(riskNodesMapAir.Compute("max([nodeRisk])", string.Empty));
            }

            Int32 avgRiskGround = 0;
            Int32 avgRiskAir = 0;

            if (riskNodesMapGround.Rows.Count > 0)
            {
                avgRiskGround = Convert.ToInt32(riskNodesMapGround.Compute("avg([nodeRisk])", string.Empty));
            }
            if (riskNodesMapAir.Rows.Count > 0)
            {
                avgRiskAir = Convert.ToInt32(riskNodesMapAir.Compute("avg([nodeRisk])", string.Empty));
            }

            flightData.MaxRisk = Math.Max(maxRiskAir, maxRiskGround);
            flightData.AvgRisk = Math.Max(avgRiskAir, avgRiskGround);
            riskNodesMapGround.Dispose();
            riskNodesMapAir.Dispose();

            _context.Update(flightData);
            await _context.SaveChangesAsync();

            var returnedData = new
            {
                id,
                latitude = flightData.Latitude,
                longitude = flightData.Longitude,
                latlonListObjectGround = flighDetail.LatLonListGround,
                latlonListObjectAir = flighDetail.LatLonListAir,
                radius = flightData.Radius
            };
            return Json(returnedData);
        }

        public static async Task<string> GetuoBData(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                var response = await request.GetResponseAsync();
                Stream responseStream;
                using (responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                        return reader.ReadToEnd();
                    }
                    return "";
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                Stream responseStream;
                using (responseStream = errorResponse.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                        reader.ReadToEnd();
                    }
                    // log errorText
                }
                throw;
            }
        }
        // GET: FlightDatas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightData = await _context.FlightData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (flightData == null)
            {
                return NotFound();
            }

            return View(flightData);
        }

        // POST: FlightDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var flightData = await _context.FlightData.SingleOrDefaultAsync(m => m.Id == id);
            FlightDetails flightDetails = await _context.FlightDetails.SingleOrDefaultAsync(m => m.FlightId == id);
            if (flightDetails != null)
            {
                _context.FlightDetails.Remove(flightDetails);
            }
            _context.FlightData.Remove(flightData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightDataExists(Guid id)
        {
            return _context.FlightData.Any(e => e.Id == id);
        }
    }
}
