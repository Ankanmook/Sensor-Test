using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcIntesityChart.Models
{
    public class SensorData
    {
        
        public SensorTest_dbEntities context;

        public List<string> deviceidList;
        public List<string> deviceinfoList;


        public List<double> gravity_ePerList;
        public List<double> pressure_ePerList;
        public List<Double> accelerometer_ePerList;
        public List<double> temp_ePerList;
        public List<double> cum_ePerList;

        public List<double> gravity_eScrList;
        public List<double> pressure_eScrList;
        public List<Double> accelerometer_eScrList;
        public List<double> temp_eScrList;
        public List<double> cum_eScrList;


        public Dictionary<String,List<double>> gravity_score;
        public Dictionary<String, List<double>> pressure_score;
        public Dictionary<String, List<double>> accelerometer_score;
        public Dictionary<String, List<double>> temp_score;
        public Dictionary<String, List<double>> cum_score;


        public Dictionary<String, List<double>> gravity_percentile;
        public Dictionary<String, List<double>> pressure_percentile;
        public Dictionary<String, List<double>> accelerometer_percentile;
        public Dictionary<String, List<double>> temp_percentile;
        public Dictionary<String, List<double>> cum_percentle;

        public HashSet< Dictionary<String, List<double>>> objLine;

        //This caputres average score of devices for percentile graph
        public Dictionary<String, List<double>> averagePercentile;
        public Dictionary<String, List<double>> averageScore;
        

        public double avg_gravity_score;
        public double avg_pressure_score;
        public double avg_accelerometer_score;
        public double avg_temp_score;
        public double avg_cum_score;


        public double avg_gravity_percentile;
        public double avg_pressure_percentile;
        public double avg_accelerometer_percentile;
        public double avg_temp_percentile;
        public double avg_cum_percentle;

        public HashSet<String> deviceid ;
        public HashSet<String> deviceinfo;


        List<double> per = new List<double>();
        List<double> score = new List<double>();


        public string DeviceTitle { get;set;}

        public string Device { get; set; }
        public string Gravity { get; set; }
        public string Acceleration { get; set; }
        
        public string Pressure { get; set; }
        public string Temperature { get; set; }

        public string Cumulative { get; set; }


        /*
         * Default Constrcutor 
         */
        public SensorData()
        {


            deviceid = new HashSet<string>();
            deviceinfo = new HashSet<string>();


            gravity_score = new Dictionary<String,List<double>>();
            pressure_score = new Dictionary<String, List<double>>();
            accelerometer_score = new Dictionary<String, List<double>>();
            temp_score = new Dictionary<String, List<double>>();
            cum_score = new Dictionary<String, List<double>>();


            gravity_percentile = new Dictionary<String, List<double>>();
            pressure_percentile = new Dictionary<String, List<double>>();
            accelerometer_percentile = new Dictionary<String, List<double>>();
            temp_percentile = new Dictionary<String, List<double>>();
            cum_percentle = new Dictionary<String, List<double>>();

            averagePercentile = new Dictionary<string, List<double>>();
            averageScore = new Dictionary<string, List<double>>();

            objLine = new HashSet<Dictionary<string, List<double>>>();

            loadDeviceInfo();
        }

        public void getDeviceInfo(String deviceInformation)
        {

            //This will initialize data context
            using (context = new SensorTest_dbEntities())
            {
                var tuple1 = from c in context.DQIndicators
                             where c.deviceinfo == deviceInformation
                             select c;

                
                accelerometer_ePerList = new List<double>();
                gravity_ePerList = new List<double>();
                pressure_ePerList = new List<double>();
                temp_ePerList = new List<Double>() ;
                cum_ePerList = new List<double>();

                accelerometer_eScrList = new List<double>();
                gravity_eScrList = new List<double>();
                pressure_eScrList = new List<double>();
                temp_eScrList = new List<double>();
                cum_eScrList = new List<double>();

                //Loading the complete list
                foreach (var d in tuple1.ToList())
                {

                    accelerometer_ePerList.Add( (double)d.percentile_accg);
                    accelerometer_eScrList.Add((double) d.score_ga);

                    gravity_ePerList.Add((double) d.percentile_g) ;
                    gravity_eScrList.Add((double) d.score_g);

                    if (d.pressure_e != null)
                    {
                        pressure_ePerList.Add((double)d.percentile_p);
                        pressure_eScrList.Add((double)d.score_p);
                    }
                    else if(d.pressure_e == null)
                    {
                        pressure_ePerList.Add(0);
                        pressure_eScrList.Add(0);
                    }

                    if (d.temp_e != null)
                    {
                        temp_ePerList.Add((double)d.percentile_t);
                        temp_eScrList.Add((double)d.score_t);
                    }
                    else if (d.temp_e == null)
                    {
                        temp_ePerList.Add(0);
                        temp_eScrList.Add(0);
                    }
                   
                    cum_ePerList.Add((double)d.cumulative_percentile);
                    cum_eScrList.Add((double)d.score);
                }

            gravity_percentile.Add(deviceInformation, gravity_ePerList);
            pressure_percentile.Add(deviceInformation,pressure_ePerList);
            accelerometer_percentile.Add(deviceInformation,accelerometer_ePerList);
            temp_percentile.Add(deviceInformation,temp_ePerList);
            cum_percentle.Add(deviceInformation,cum_ePerList);

            gravity_score.Add(deviceInformation, gravity_eScrList);
            pressure_score.Add(deviceInformation, pressure_eScrList);
            accelerometer_score.Add(deviceInformation, accelerometer_ePerList);
            temp_score.Add(deviceInformation, temp_ePerList);
            cum_score.Add(deviceInformation, cum_eScrList);

            //Calculating average score for one device
            List<double> lstP = new List<double>();
            List<double> lstS = new List<double>();


            lstP.Add(getAvg(gravity_ePerList));
            lstP.Add(getAvg(accelerometer_ePerList));
            lstP.Add(getAvg(pressure_ePerList));
            lstP.Add(getAvg(temp_ePerList));
            lstP.Add(getAvg(cum_ePerList));
           
            lstS.Add(getAvg(gravity_eScrList));
            lstS.Add(getAvg(accelerometer_ePerList));
            lstS.Add(getAvg(pressure_eScrList));
            lstS.Add(getAvg(temp_ePerList));
            lstS.Add(getAvg(cum_eScrList));
            
            averagePercentile.Add(deviceInformation, lstS) ;
            averageScore.Add(deviceInformation,lstP);
                        
            }
            
            //Adding dictionaries to line for line graph
            objLine.Add(gravity_percentile);
            objLine.Add(accelerometer_percentile);
            objLine.Add(pressure_score);
            objLine.Add(temp_percentile);
            objLine.Add(cum_percentle);

            objLine.Add(gravity_score);
            objLine.Add(accelerometer_score);
            objLine.Add(pressure_score);
            objLine.Add(temp_score);
            objLine.Add(cum_score);


        }


        /*
         * Computes the average of a list 
         */
        public double getAvg(List<double> sequence)
        {
            return sequence.Average();
        }


        /*
         * Loading all the information for the development of the 
         * chart
         */
        public void loadDeviceInfo()
        {
            using (SensorTest_dbEntities context = new SensorTest_dbEntities())
            {
                var device = from d in context.DQIndicators
                             select d.deviceinfo;

                foreach (var s in device.ToList())
                {
                    deviceinfo.Add(s);
                }


                foreach(var d in deviceinfo.ToList())
                {
                    getDeviceInfo(d);
                }
            }

            //deviceDataView = new Dictionary<string, List<Dictionary<string, List<double>>>>();
            //deviceDataView.Add()

            
        }

        /*
        public dynamic GetSmartPhoneInfo()
        {
            using (SensorTest_dbEntities context = new SensorTest_dbEntities())
            {
                var smartPhone = from d in context.DQIndicators
                                 select d.deviceid;

                foreach (var s in smartPhone.ToList())
                {
                    deviceid.Add(s);
                }

            }

        }
         */ 

    }
}