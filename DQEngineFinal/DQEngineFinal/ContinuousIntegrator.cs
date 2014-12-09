using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQEngineFinal
{
    class ContinuousIntegrator
    {


        public SensorTest_dbEntities context;
        public List<string> idListDeviceData;
        public List<string> idListDQIndicator;

        public List<string> deviceidList;
        public List<string> deviceinfoList;

        public List<Double> accelerometer_eList;
        public List<double> gravity_eList;
        public List<double> actualGravity_List;


        public List<double> tempList;
        public List<double> pressureDiff;

        public List<System.DateTimeOffset> timestampList;

        public List<double> orientation;

        public List<Double> magneticField_XList;
        public List<Double> magneticField_YList;
        public List<double> magneticField_ZList;
        public List<double> gyroscope_XList;
        public List<double> gyroscope_YList;
        public List<double> gyroscope_ZList;


        /*
         * Default Constrcutor 
         */
        public ContinuousIntegrator()
        {

            idListDeviceData = new List<string>();
            deviceidList = new List<string>();
            deviceinfoList = new List<string>();

            accelerometer_eList = new List<double>();
            gravity_eList = new List<double>();
            actualGravity_List = new List<double>();

            tempList = new List<double>();


            pressureDiff = new List<double>();

            timestampList = new List<DateTimeOffset>();

            orientation = new List<double>();

            magneticField_XList = new List<double>();
            magneticField_YList = new List<double>();
            magneticField_ZList = new List<double>();
            gyroscope_XList = new List<double>(); ;
            gyroscope_YList = new List<double>(); ;
            gyroscope_ZList = new List<double>(); ;


        }

        public void initializeContext()
        {
            double actualGravity;
            //This will initialize data context
            using (context = new SensorTest_dbEntities())
            {
                var tuple1 = from c in context.DeviceDatas
                             select c;


                //Loading the complete list
                foreach (var d in tuple1)
                {
                    idListDeviceData.Add(d.id);
                    deviceidList.Add(d.deviceid);
                    deviceinfoList.Add(d.deviceinfo);

                    magneticField_XList.Add((double)d.magneticfeild_x);
                    magneticField_YList.Add((double)d.magneticfeild_y);
                    magneticField_ZList.Add((double)d.magneticfeild_z);
                    gyroscope_XList.Add((double)d.gyroscope_x);
                    gyroscope_YList.Add((double)d.gyroscope_y);
                    gyroscope_ZList.Add((double)d.gyroscope_z);

                    actualGravity = getActualGravity((double)d.latitutde);
                    actualGravity_List.Add(actualGravity);

                    double sensorgravity = gravityCalculation((double)d.gravity_x, (double)d.gravity_y, (double)d.gravity_z);
                    double accelerometerGravity = gravityCalculation((double)d.accelerometer_x, (double)d.accelerometer_y, (double)d.accelerometer_z);

                    gravity_eList.Add(sensorgravity - actualGravity);

                    if (d.battery_temperature != null)
                    {
                        tempList.Add((double)(d.battery_temperature - d.temperature));
                    }

                    if(d.error_pressure != 0){
                        pressureDiff.Add((double)(d.error_pressure));
                    }
                    
                    accelerometer_eList.Add(sensorgravity - accelerometerGravity);



                    timestampList.Add((DateTimeOffset)d.timestamp);


                }

                //Adding already present tuples in the device id data set, these will updated, new 
                //values would we inserted


                var tuple2 = from c in context.DQIndicators
                             select c;
                foreach (var d in tuple2.ToList())
                {
                    idListDQIndicator.Add(d.id);
                }

            }


        }

        public double getPitch(double x, double y, double z)
        {
            return Math.Atan(x / (Math.Pow(x, 2) + Math.Pow(z, 2)));
        }

        public double getRoll(double x, double y, double z)
        {
            return Math.Atan(y / (Math.Pow(y, 2) + Math.Pow(z, 2)));
        }

        public double gravityCalculation(double x, double y, double z)
        {
            return Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2.0) + Math.Pow(z, 2));
        }


        /*
         * Calculating gravity at exact latitude from this formula
         * For ref: http://geophysics.ou.edu/solid_earth/notes/potential/igf.htm
         */
        public double getActualGravity(double latitude)
        {
            return 9.7803267714 * (1 + 0.00193185138639 * Math.Pow(Math.Sin(latitude), 2.0))
                / (Math.Sqrt(1 - 0.00669437999013 * Math.Pow(Math.Sin(latitude), 2.0)));
        }


 
        


        /*
         * Calculates sensor fusion from givem input variables
         */
        public void calculateSensorFusion()
        {
            using (var context = new SensorTest_dbEntities())
            {
                var tuple = from c in context.DeviceDatas
                            select c;


                foreach (var d in tuple)
                {

                    //update for a old entity and insert for a new entity

                    if (d.azimuth != null)
                    {

                        //DeviceData deviceData = new DeviceData();

                        Console.WriteLine("Pitch " + getPitch((double)d.accelerometer_x, (double)d.accelerometer_y, (double)d.accelerometer_z));
                        Console.WriteLine("Roll " + getRoll((double)d.accelerometer_x, (double)d.accelerometer_y, (double)d.accelerometer_z));
                        Console.WriteLine("Sensor Pitch " + d.pitch);
                        Console.WriteLine("Sensor Roll " + d.roll);

                        /*
                        SensorFusion sf = new SensorFusion((double)d.accelerometer_x, (double)d.accelerometer_y, (double)d.accelerometer_z
                            , (double)d.azimuth, (double)d.pitch, (double)d.roll,
                            (double)d.gyroscope_x, (double)d.gyroscope_y, (double)d.gyroscope_z);
                         */
                    }
                }
                Console.ReadLine();
            }
        }


        /*
         * This will insert for new data 
         * update old data
         */

        public void calculateDQ()
        {
            //stp = Stopwatch.StartNew();

            using (var context = new SensorTest_dbEntities())  //new SensorTest_dbEntities())
            {
                var tuple1 = from dd in context.DeviceDatas
                             select dd;

                foreach (var d in tuple1.ToList())
                {

                    DQIndicator dqindicator = new DQIndicator();
                    double actualG = getActualGravity((double)d.latitutde);
                    double sensorgravity = gravityCalculation((double)d.gravity_x, (double)d.gravity_y, (double)d.gravity_z);
                    double accelerometerGravity = gravityCalculation((double)d.accelerometer_x, (double)d.accelerometer_y, (double)d.accelerometer_z);

                    dqindicator.id = d.id;
                    dqindicator.deviceid = d.deviceid;
                    dqindicator.deviceinfo = d.deviceinfo;

                    dqindicator.gravity = getActualGravity(actualG);
                    dqindicator.gravity_e = sensorgravity - actualG;
                    dqindicator.percentile_g = Percentiler.percentile(gravity_eList, (double)dqindicator.gravity_e);
                    dqindicator.score_g = Percentiler.giveRelativeError(gravity_eList, (double)dqindicator.gravity_e);

                    dqindicator.percentile_accg = Percentiler.percentile(accelerometer_eList, (double) (sensorgravity - accelerometerGravity));
                    dqindicator.accel_gravity_e = sensorgravity - accelerometerGravity;
                    dqindicator.score_ga = Percentiler.giveRelativeError(accelerometer_eList, (double) (sensorgravity - accelerometerGravity));

                    dqindicator.percentile_p = Percentiler.percentile(pressureDiff, (double)(d.error_pressure));
                    dqindicator.score_p = Percentiler.giveRelativeError(pressureDiff, (double)(d.error_pressure));

                    if (d.error_pressure != 0)
                    {
                        dqindicator.pressure_e = (double)(d.error_pressure);
                    }
                    

                    
                    if ((d.battery_temperature != 0) && (d.battery_voltage != 0))
                    {
                        //For those devices which have battery temperature recorded
                        if((d.error_pressure != 0) && (d.battery_temperature != 0) )
                        { 
                            dqindicator.percentile_t = Percentiler.percentile(tempList, (double)(d.battery_temperature - d.temperature));
                            dqindicator.score_t = Percentiler.giveRelativeError(tempList, (double)(d.battery_temperature - d.temperature));
                            dqindicator.temp_e = (double)(d.battery_temperature - d.temperature);

                            dqindicator.cumulative_percentile = (double) ((dqindicator.percentile_accg + dqindicator.percentile_g + dqindicator.percentile_p + dqindicator.percentile_t) )/ 4;
                            dqindicator.score = (double)((dqindicator.score_g + dqindicator.score_p + dqindicator.score_ga + dqindicator.score_t)) / 4;
                        }
                        //For those devices which dont have pressure sesnor but temperature sensor
                        else if ((d.battery_temperature != 0) & (d.error_pressure == 0))
                        {
                            dqindicator.percentile_t = Percentiler.percentile(tempList, (double)(d.battery_temperature - d.temperature));
                            dqindicator.score_t = Percentiler.giveRelativeError(tempList, (double)(d.battery_temperature - d.temperature));
                            dqindicator.temp_e = (double)(d.battery_temperature - d.temperature);
                            
                            dqindicator.cumulative_percentile = (double)((dqindicator.percentile_accg + dqindicator.percentile_g  + dqindicator.percentile_t)) / 3;
                            dqindicator.score = (double)((dqindicator.score_g + dqindicator.score_ga + dqindicator.score_t)) / 3;
                        }
                        //For those device which neither have pressure nor temperature sensor
                        else if ((d.error_pressure == 0) && (d.battery_temperature == 0))
                        {
                            dqindicator.cumulative_percentile = (double)((dqindicator.percentile_accg + dqindicator.percentile_g )) / 2;
                            dqindicator.score = (double)((dqindicator.score_g + dqindicator.score_ga )) / 2;

                        }
                    }
                    else
                    {
                        dqindicator.cumulative_percentile =  (double) ((dqindicator.percentile_accg + dqindicator.percentile_g + dqindicator.percentile_p)) / 3;
                        dqindicator.score = (double) ( (dqindicator.score_g + dqindicator.score_p + dqindicator.score_ga)) / 3;
                    }
                    

                    context.DQIndicators.Add(dqindicator);
                    context.SaveChanges();
                }


            }

            //Thread.Sleep(1000);
            //stp.Stop();

            Console.WriteLine("Press Any Key to Continue. The operation is finished");
            Console.ReadLine();
        }
    }
}
