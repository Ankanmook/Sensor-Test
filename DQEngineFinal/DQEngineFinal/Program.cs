using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DQEngineFinal
{
    class Program
    {
        static void Main(string[] args)
        {
            //extractData();
            ContinuousIntegrator ci = new ContinuousIntegrator();
            ci.initializeContext();
            //ci.calculateSensorFusion();
            ci.calculateDQ();
           
        }
    }
}
