using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQEngineFinal
{
    class SensorFusion
    {


        // angular speeds from gyro
        private double[] gyro = new double[3];

        // rotation matrix from gyro data
        private double[] gyroMatrix = new double[9];

        // orientation angles from gyro matrix
        private double[] gyroOrientation = new double[3];

        // magnetic field vector
        private double[] magnet = new double[3];

        // accelerometer vector
        private double[] accel = new double[3];

        // orientation angles from accel and magnet
        private double[] accMagOrientation = new double[3];

        // final orientation angles from sensor fusion
        private double[] fusedOrientation = new double[3];

        // accelerometer and magnetometer based rotation matrix
        private double[] rotationMatrix = new double[9];

        public static double EPSILON = 0.000000001f;
        private static double NS2S = 1.0f / 1000000000.0f;
        private int timestamp;
        private bool initState = true;

        public static int TIME_CONSTANT = 30;
        public static double FILTER_COEFFICIENT = 0.98f;

        //private Timer fuseTimer = new Timer();


        //DecimalFormat d = new DecimalFormat("#.##");

        /*
         * Default Constructor 
         */
        public SensorFusion(double accel_x, double accel_y, double accel_z,
            double magnet_x, double magnet_y, double magnet_z, double gyro_x, double gyro_y, double gyro_z)
        {

            accel[0] = accel_x;
            accel[1] = accel_y;
            accel[2] = accel_z;

            gyro[0] = gyro_x;
            gyro[1] = gyro_y;
            gyro[2] = gyro_z;

            magnet[0] = magnet_x;
            magnet[1] = magnet_y;
            magnet[2] = magnet_z;

            intiliazer();
            gyroFunction();
            calculateSensorFusionOrientation();
            calculateSensorFusionOrientation();
        }


        public void PrintMatrix(double[] matrix)
        {
            Console.WriteLine("Yaw " + " x: " + matrix[0] + " y: " + matrix[1] + " z: " + matrix[2]);
            Console.WriteLine("Pitch " + " x: " + matrix[3] + " y: " + matrix[4] + " z: " + matrix[5]);
            Console.WriteLine("Roll " + " x: " + matrix[6] + " y: " + matrix[7] + " z: " + matrix[8]);
        }

        /*
         * This is intiliazer function 
         */
        public void intiliazer()
        {

            gyroOrientation[0] = 0.0f;
            gyroOrientation[1] = 0.0f;
            gyroOrientation[2] = 0.0f;

            // initialise gyroMatrix with identity matrix
            gyroMatrix[0] = 1.0f; gyroMatrix[1] = 0.0f; gyroMatrix[2] = 0.0f;
            gyroMatrix[3] = 0.0f; gyroMatrix[4] = 1.0f; gyroMatrix[5] = 0.0f;
            gyroMatrix[6] = 0.0f; gyroMatrix[7] = 0.0f; gyroMatrix[8] = 1.0f;


            // wait for one second until gyroscope and magnetometer/accelerometer
            // data is initialised then scedule the complementary filter task

        }


        // This function is borrowed from the Android reference
        // at http://developer.android.com/reference/android/hardware/SensorEvent.html#values
        // It calculates a rotation vector from the gyroscope angular speed values.
        private void getRotationVectorFromGyro(double[] gyroValues,
                double[] deltaRotationVector,
                double timeFactor)
        {
            double[] normValues = new double[3];

            // Calculate the angular speed of the sample
            double omegaMagnitude =
            (double)Math.Sqrt(gyroValues[0] * gyroValues[0] +
            gyroValues[1] * gyroValues[1] +
            gyroValues[2] * gyroValues[2]);

            // Normalize the rotation vector if it's big enough to get the axis
            if (omegaMagnitude > EPSILON)
            {
                normValues[0] = gyroValues[0] / omegaMagnitude;
                normValues[1] = gyroValues[1] / omegaMagnitude;
                normValues[2] = gyroValues[2] / omegaMagnitude;
            }

            // Integrate around this axis with the angular speed by the timestep
            // in order to get a delta rotation from this sample over the timestep
            // We will convert this axis-angle representation of the delta rotation
            // into a quaternion before turning it into the rotation matrix.
            double thetaOverTwo = omegaMagnitude * timeFactor;
            double sinThetaOverTwo = (double)Math.Sin(thetaOverTwo);
            double cosThetaOverTwo = (double)Math.Cos(thetaOverTwo);
            deltaRotationVector[0] = sinThetaOverTwo * normValues[0];
            deltaRotationVector[1] = sinThetaOverTwo * normValues[1];
            deltaRotationVector[2] = sinThetaOverTwo * normValues[2];
            deltaRotationVector[3] = cosThetaOverTwo;
        }

        private double[] getRotationMatrixFromOrientation(double[] o)
        {
            double[] xM = new double[9];
            double[] yM = new double[9];
            double[] zM = new double[9];

            double sinX = (double)Math.Sin(o[1]);
            double cosX = (double)Math.Cos(o[1]);
            double sinY = (double)Math.Sin(o[2]);
            double cosY = (double)Math.Cos(o[2]);
            double sinZ = (double)Math.Sin(o[0]);
            double cosZ = (double)Math.Cos(o[0]);

            // rotation about x-axis (pitch)
            xM[0] = 1.0f; xM[1] = 0.0f; xM[2] = 0.0f;
            xM[3] = 0.0f; xM[4] = cosX; xM[5] = sinX;
            xM[6] = 0.0f; xM[7] = -sinX; xM[8] = cosX;

            // rotation about y-axis (roll)
            yM[0] = cosY; yM[1] = 0.0f; yM[2] = sinY;
            yM[3] = 0.0f; yM[4] = 1.0f; yM[5] = 0.0f;
            yM[6] = -sinY; yM[7] = 0.0f; yM[8] = cosY;

            // rotation about z-axis (azimuth)
            zM[0] = cosZ; zM[1] = sinZ; zM[2] = 0.0f;
            zM[3] = -sinZ; zM[4] = cosZ; zM[5] = 0.0f;
            zM[6] = 0.0f; zM[7] = 0.0f; zM[8] = 1.0f;

            // rotation order is y, x, z (roll, pitch, azimuth)
            double[] resultMatrix = matrixMultiplication(xM, yM);
            resultMatrix = matrixMultiplication(zM, resultMatrix);
            return resultMatrix;
        }

        public double[] matrixMultiplication(double[] A, double[] B)
        {
            double[] result = new double[9];

            result[0] = A[0] * B[0] + A[1] * B[3] + A[2] * B[6];
            result[1] = A[0] * B[1] + A[1] * B[4] + A[2] * B[7];
            result[2] = A[0] * B[2] + A[1] * B[5] + A[2] * B[8];

            result[3] = A[3] * B[0] + A[4] * B[3] + A[5] * B[6];
            result[4] = A[3] * B[1] + A[4] * B[4] + A[5] * B[7];
            result[5] = A[3] * B[2] + A[4] * B[5] + A[5] * B[8];

            result[6] = A[6] * B[0] + A[7] * B[3] + A[8] * B[6];
            result[7] = A[6] * B[1] + A[7] * B[4] + A[8] * B[7];
            result[8] = A[6] * B[2] + A[7] * B[5] + A[8] * B[8];

            return result;
        }

        // This function performs the integration of the gyroscope data.
        // It writes the gyroscope based orientation into gyroOrientation.
        public void gyroFunction()
        {

            if (accMagOrientation == null)
                return;

            // initialisation of the gyroscope based rotation matrix
            if (initState)
            {
                double[] initMatrix = new double[9];
                initMatrix = getRotationMatrixFromOrientation(accMagOrientation);
                double[] test = new double[3];
                //SensorManager.getOrientation(initMatrix, test);
                gyroMatrix = matrixMultiplication(gyroMatrix, initMatrix);
                initState = false;
            }

            // copy the new gyro values into the gyro array
            // convert the raw gyro data into a rotation vector
            double[] deltaVector = new double[4];
            /*
            if(timestamp != 0) {
            
            System.arraycopy(event.values, 0, gyro, 0, 3);
        
             */
            double dT = (0.1) * NS2S;
            getRotationVectorFromGyro(gyro, deltaVector, dT / 2.0f);



            // convert rotation vector into rotation matrix
            double[] deltaMatrix = new double[9];

            // apply the new rotation interval on the gyroscope based rotation matrix
            gyroMatrix = matrixMultiplication(gyroMatrix, deltaMatrix);

        }

        public void calculateSensorFusionOrientation()
        {
            double oneMinusCoeff = 1.0f - FILTER_COEFFICIENT;



            // azimuth
            if (gyroOrientation[0] < -0.5 * Math.PI && accMagOrientation[0] > 0.0)
            {
                fusedOrientation[0] = (double)(FILTER_COEFFICIENT * (gyroOrientation[0] + 2.0 * Math.PI) + oneMinusCoeff * accMagOrientation[0]);
                fusedOrientation[0] -= (fusedOrientation[0] > Math.PI) ? 2.0 * Math.PI : 0;
            }
            else if (accMagOrientation[0] < -0.5 * Math.PI && gyroOrientation[0] > 0.0)
            {
                fusedOrientation[0] = (double)(FILTER_COEFFICIENT * gyroOrientation[0] + oneMinusCoeff * (accMagOrientation[0] + 2.0 * Math.PI));
                fusedOrientation[0] -= (fusedOrientation[0] > Math.PI) ? 2.0 * Math.PI : 0;
            }
            else
            {
                fusedOrientation[0] = FILTER_COEFFICIENT * gyroOrientation[0] + oneMinusCoeff * accMagOrientation[0];
            }

            // pitch
            if (gyroOrientation[1] < -0.5 * Math.PI && accMagOrientation[1] > 0.0)
            {
                fusedOrientation[1] = (double)(FILTER_COEFFICIENT * (gyroOrientation[1] + 2.0 * Math.PI) + oneMinusCoeff * accMagOrientation[1]);
                fusedOrientation[1] -= (fusedOrientation[1] > Math.PI) ? 2.0 * Math.PI : 0;
            }
            else if (accMagOrientation[1] < -0.5 * Math.PI && gyroOrientation[1] > 0.0)
            {
                fusedOrientation[1] = (double)(FILTER_COEFFICIENT * gyroOrientation[1] + oneMinusCoeff * (accMagOrientation[1] + 2.0 * Math.PI));
                fusedOrientation[1] -= (fusedOrientation[1] > Math.PI) ? 2.0 * Math.PI : 0;
            }
            else
            {
                fusedOrientation[1] = FILTER_COEFFICIENT * gyroOrientation[1] + oneMinusCoeff * accMagOrientation[1];
            }

            // roll
            if (gyroOrientation[2] < -0.5 * Math.PI && accMagOrientation[2] > 0.0)
            {
                fusedOrientation[2] = (double)(FILTER_COEFFICIENT * (gyroOrientation[2] + 2.0 * Math.PI) + oneMinusCoeff * accMagOrientation[2]);
                fusedOrientation[2] -= (fusedOrientation[2] > Math.PI) ? 2.0 * Math.PI : 0;
            }
            else if (accMagOrientation[2] < -0.5 * Math.PI && gyroOrientation[2] > 0.0)
            {
                fusedOrientation[2] = (double)(FILTER_COEFFICIENT * gyroOrientation[2] + oneMinusCoeff * (accMagOrientation[2] + 2.0 * Math.PI));
                fusedOrientation[2] -= (fusedOrientation[2] > Math.PI) ? 2.0 * Math.PI : 0;
            }
            else
            {
                fusedOrientation[2] = FILTER_COEFFICIENT * gyroOrientation[2] + oneMinusCoeff * accMagOrientation[2];
            }

            // overwrite gyro matrix and orientation with fused orientation
            // to comensate gyro drift
            gyroMatrix = getRotationMatrixFromOrientation(fusedOrientation);

            PrintMatrix(gyroMatrix);
            //System.arraycopy(fusedOrientation, 0, gyroOrientation, 0, 3);

        }


    }  
}
