﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;


namespace HelloWorld
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class COM_Visibles
    {
        public void FollowLink()
        {
            MessageBox.Show("Run converted code to follow the link.");
        }
        //public double INFL2(int year1, int year2, int mode, int category, int agency)     //outdated
        //{
        //    return DNA_Test.InflationCalculator.Calculate(year1, year2, mode, category, agency);
        //}
        public double[] Beta2M(double mean, double stdev, double min, double max)
        {
            var beta = new BetaDistribution();
            return beta.Beta2M(mean, stdev, min, max);
        }

    }
}
