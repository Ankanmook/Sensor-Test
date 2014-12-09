using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcIntesityChart.Models;
using System.Web.Helpers;
using System.Web.UI.DataVisualization.Charting;

namespace MvcIntesityChart.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/



        public ActionResult Index()
        {

            SensorData snData = GetChartData();

            snData.DeviceTitle = "Device";

            /*
            var chart = new Chart(width: 300, height: 200)
                .AddSeries(
                            chartType: "bar",
                            xValue: new[] { "10 Records", "20 Records", "30 Records", "40 Records" },
                            yValues: new[] { "50", "60", "78", "80" })
                            .GetBytes("png");
            //return File(chart, "image/bytes");
            

            
            
            ProductModel objProductModel = new ProductModel();
            objProductModel.ProductData = new Product();
            objProductModel.ProductData = GetChartData();
            objProductModel.YearTitle = "Year";
            objProductModel.SaleTitle = "Sale";
            objProductModel.PurchaseTitle = "Purchase";
            return View(objProductModel);
             */

            return View(snData);

        }
        /// <summary>
        /// Code to get the data which we will pass to chart
        /// </summary>
        /// <returns></returns>
        public SensorData GetChartData()
        {
            SensorData snData = new SensorData();
            snData.DeviceTitle = "Device";

            snData.Device = "";

            foreach (string s in snData.deviceinfo)
            {
                snData.Device = snData.Device + "," + s;
            }

            snData.Gravity = "";
            snData.Acceleration = "";
            snData.Pressure = "";
            snData.Temperature = "";
            snData.Cumulative = "";


            foreach (var d in snData.deviceinfo)
            {
                snData.Gravity = snData.Gravity + "," + snData.averagePercentile[d].ElementAt(0);
                snData.Acceleration = snData.Acceleration + "," + snData.averagePercentile[d].ElementAt(1);
                snData.Pressure = snData.Pressure + "," + snData.averagePercentile[d].ElementAt(2);
                snData.Temperature = snData.Temperature + "," + snData.averagePercentile[d].ElementAt(3);
                snData.Cumulative = snData.Cumulative + "," + snData.averagePercentile[d].ElementAt(4);
            }

            snData.Gravity = snData.Gravity.Substring(1);
            snData.Acceleration = snData.Acceleration.Substring(1);
            snData.Pressure = snData.Pressure.Substring(1);
            snData.Temperature = snData.Temperature.Substring(1);
            snData.Cumulative = snData.Cumulative.Substring(1);

            return snData;
        }

    }
}


    

