﻿using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using Tools = Microsoft.Office.Tools.Excel;
using System.Drawing;
using HelloWorld.Properties;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new MyRibbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace HelloWorld
{
    [ComVisible(true)]
    public class MyRibbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;
        //private string GetContent { get; set; }

        public MyRibbon()
        {
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("HelloWorld.MyRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
            
        }
        public void btnHello_Click(IRibbonControl e)
        {
            MessageBox.Show("Hello World!");
        }

        public void btnAddFormulas_Click(IRibbonControl e)
        {
            ObjModel.SetFormulas("A1:B2", "=C3");       //relative references
            ObjModel.SetFormulas("A4:B5", "=$C$3");     //absolute references

            ObjModel.SetCell(5, "A6");
            ObjModel.SetFormulas("A7:A10", "=A5+RandBetween(5,100)+Norm.Inv(.3,0,1)"); //add a formula to the sheet
            ObjModel.SetFormulas("E1:H1", "=Column()+2000");
            ObjModel.SetArrayFormulas("E2:H2", "=INFL(2002, 2000,1,1,4)*E1:H1");

            
        }

        public void btnWorksheetFunction_Click(IRibbonControl e)
        {
            //ObjModel.GetSelection().Value = Utilities.wsFunction.Norm_Inv(.3, 0, 1);    //use a worksheetFunction

        }

        public void btnCopyFormats_Click(IRibbonControl e)
        {
            Utilities.CopyFormats();
        }

        public void btnRefEdit_Click(IRibbonControl e)
        {
            TestForm tf = new TestForm();
            tf.ShowDialog();
        }

        public void RightClickTest_Click(IRibbonControl e)      //how to feed in the cell reference?
        {
            string selectionReference = ObjModel.GetSelection().Address;
            MessageBox.Show(selectionReference);
        }
        public string GetMenuContent(IRibbonControl e)      //Need to determine which section the user is in here and return the right menu
        {
            MenuBuilder mb = new MenuBuilder();
            return mb.Build();
        }

        public void btnExperimental_Click(IRibbonControl e)
        {
            ChartBuilder cb = new ChartBuilder();
            Tools.Worksheet worksheet = ObjModel.GetActiveSheet();
            Excel.Range cells = worksheet.Range["A1", "J20"];
            cb.AddChart(worksheet, cells, ChartBuilder.Template.Chart1);
        }

        public void btnCopySheet_Click(IRibbonControl e)
        {
            //open template sheet
            Excel.Workbook book = Utilities.OpenWorkbook(@"C:\Users\grins\Documents\Custom Office Templates\TestTemplate.xlsx");
            Excel.Worksheet sheet = book.Worksheets["ABC"];
            Excel.Worksheet copyTo = book.Worksheets["DEF"];
            Excel.Worksheet copyTwo = ThisAddIn.MyApp.Worksheets["Sheet1"];
            sheet.Copy(copyTwo);
            book.Save();
            book.Close();
        }

        public void btnSerialize_Click(IRibbonControl e)
        {
            //InflationCalculator.SerializeTables();
            //var table = Serializer.ReadXML<InflationTable>(@"C:\Users\grins\source\repos\HelloWorld\HelloWorld\test_xml.xml");
        }

        public void btnProgressBar_Click(IRibbonControl e)
        {
            ProgressWindow progWin = new ProgressWindow();
            progWin.ShowDialog();
        }

        public void btnFormatCorrel_Click(IRibbonControl e)
        {
            Excel.Range template = ThisAddIn.MyApp.Worksheets["Template"].range["C3"];
            Excel.Range target = ThisAddIn.MyApp.ActiveCell;
            Formatter former = new Formatter(template, target, ThisAddIn.MyApp.ActiveSheet);
            former.FormatRange();
        }

        public void btnClear_Click(IRibbonControl e)
        {
            Formatter.ResetSheet();
        }

        public void btnBetaArray_Click(IRibbonControl e)
        {
            this.btnClear_Click(e);
            ObjModel.SetCell(10, "A1");
            ObjModel.SetCell(2, "B1");
            ObjModel.SetCell(3, "C1");
            ObjModel.SetCell(15, "D1");
            ObjModel.SetArrayFormulas("A2:D2", "=Beta2M(A1,B1,C1,D1)");
        }

        public void btnCreateNewModel_Click(IRibbonControl e)
        {
            ThisAddIn.Model = new CASE_Model();
            ThisAddIn.Model.SetupModel();
        }

        public void btnBuildEstimate_Click(IRibbonControl e)
        {
            //check if "Correlation" sheet exists & create if not. Otherwise just grab it.
            EstimateSheet estimateSheet = ThisAddIn.Model.EstimateSheets[0];
            CorrelationSheet correlSheet = ThisAddIn.Model.correlationSheet;
            correlSheet.ClearSheet();
            var estimate = new Estimate(estimateSheet, correlSheet, 50);     //example estimate with 50 inputs
            correlSheet.Correlates = estimate;
            correlSheet.Correlates.CorrelMatrix.PrintCorrelationMatrix();
        }

        public void btnTestMMult_Click(IRibbonControl e)
        {
            double[] m1 = { 1.1, 2.3, 4.0, 3.5, 1.8 };
            double[] m2 = { 1.1, 2.3, 4.0, 3.5, 1.8 };
            List<long> ticks1 = new List<long>();
            List<long> ticks2 = new List<long>();
            List<long> ticks3 = new List<long>();
            
            for (int i = 0; i < 100; i++)
            {
                DiagnosticsMenu.StartStopwatch();
                double result1 = MatrixOps.MMult(m1, m2);
                DiagnosticsMenu.StopStopwatch(TimeUnit.ticks);
                ticks1.Add(DiagnosticsMenu.stopwatch.ElapsedTicks);
                DiagnosticsMenu.StartStopwatch();
                double result2 = MatrixOps.Accord_MMult(m1, m2);
                DiagnosticsMenu.StopStopwatch(TimeUnit.ticks);
                ticks2.Add(DiagnosticsMenu.stopwatch.ElapsedTicks);
                DiagnosticsMenu.StartStopwatch();
                dynamic result3 = ThisAddIn.MyApp.WorksheetFunction.MMult(m1, ThisAddIn.MyApp.WorksheetFunction.Transpose(m2));
                DiagnosticsMenu.StopStopwatch(TimeUnit.ticks);
                ticks3.Add(DiagnosticsMenu.stopwatch.ElapsedTicks);
                result1 = 0;
                result2 = 0;
                result3 = 0;
            }
            MessageBox.Show($"{ticks1.Average().ToString()} Manual");
            MessageBox.Show($"{ticks2.Average().ToString()} Accord");
            MessageBox.Show($"{ticks3.Average().ToString()} VBA");
        }

        public void btnTestInverse_Click(IRibbonControl e)
        {
            Random rando = new Random();
            double[,] TwoArray = new double[1000, 1000];
            for(int i=0; i<TwoArray.GetLength(0); i++)
            {
                for (int j = 0; j < TwoArray.GetLength(1); j++)
                {
                    TwoArray[i,j] = rando.NextDouble();
                }
            }

            List<double> ticks1 = new List<double>();
            List<double> ticks2 = new List<double>();

            for (int i = 0; i < 3; i++)
            {
                DiagnosticsMenu.StartStopwatch();
                var result1 = MatrixOps.MatrixInverse(TwoArray);
                ticks1.Add(DiagnosticsMenu.StopStopwatch(TimeUnit.seconds));
                DiagnosticsMenu.StartStopwatch();
                var result2 = ThisAddIn.MyApp.WorksheetFunction.MInverse(TwoArray);
                ticks2.Add(DiagnosticsMenu.StopStopwatch(TimeUnit.seconds));
                result1 = null;
                result2 = null;

            }
            MessageBox.Show($"{ticks1.Average().ToString()} Accord");
            MessageBox.Show($"{ticks2.Average().ToString()} VBA");

        }

        public void btnTestTranspose_Click(IRibbonControl e)
        {
            Random rando = new Random();
            double[,] TwoArray = new double[1000, 1000];
            for (int i = 0; i < TwoArray.GetLength(0); i++)
            {
                for (int j = 0; j < TwoArray.GetLength(1); j++)
                {
                    TwoArray[i, j] = rando.NextDouble();
                }
            }

            List<double> ticks1 = new List<double>();
            List<double> ticks2 = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                DiagnosticsMenu.StartStopwatch();
                var result1 = MatrixOps.Transpose(TwoArray);
                ticks1.Add(DiagnosticsMenu.StopStopwatch(TimeUnit.milliseconds));
                DiagnosticsMenu.StartStopwatch();
                var result2 = ThisAddIn.MyApp.WorksheetFunction.Transpose(TwoArray);
                ticks2.Add(DiagnosticsMenu.StopStopwatch(TimeUnit.milliseconds));
                result1 = null;
                result2 = null;
            }
            MessageBox.Show($"{ticks1.Average().ToString()} Accord");
            MessageBox.Show($"{ticks2.Average().ToString()} VBA");
        }

        public void btnTestDeterminant_Click(IRibbonControl e)
        {
            Random rando = new Random();
            double[,] TwoArray = new double[100, 100];
            for (int i = 0; i < TwoArray.GetLength(0); i++)
            {
                for (int j = 0; j < TwoArray.GetLength(1); j++)
                {
                    TwoArray[i, j] = rando.NextDouble();
                }
            }

            List<double> ticks1 = new List<double>();
            List<double> ticks2 = new List<double>();

            for (int i = 0; i < 100; i++)
            {
                DiagnosticsMenu.StartStopwatch();
                double result1 = MatrixOps.Determinant(TwoArray);
                ticks1.Add(DiagnosticsMenu.StopStopwatch(TimeUnit.ticks));
                DiagnosticsMenu.StartStopwatch();
                double result2 = ThisAddIn.MyApp.WorksheetFunction.MDeterm(TwoArray);
                ticks2.Add(DiagnosticsMenu.StopStopwatch(TimeUnit.ticks));
                result1 = 0;
                result2 = 0;
            }
            MessageBox.Show($"{ticks1.Average().ToString()} Accord");
            MessageBox.Show($"{ticks2.Average().ToString()} VBA");
        }

        public void btnSerializeFile_Click(IRibbonControl e)
        {
            //Serialize the FileSheet object to xml for the purpose of saving basic defaults
            ThisAddIn.Model.fileSheet.Settings.SerializeSettings();     //export Settings object to xml
        }

        public Bitmap GetImage(IRibbonControl control)
        {
            switch(control.Id)
            {
                case "btnSerialize":
                    return Resources.bitmap;
                case "btnCopySheet":
                    return Resources.bitmap;
                case "MyMenu":
                    return Resources.bitmap;
                default:
                    return Resources.HideousIcon;
            }
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
