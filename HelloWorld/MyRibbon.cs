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
            ObjModel.GetSelection().Value = Utilities.wsFunction.Norm_Inv(.3, 0, 1);    //use a worksheetFunction

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
            InflationCalculator.SerializeTables();
            var table = Serializer.ReadXML<InflationTable>(@"C:\Users\grins\source\repos\HelloWorld\HelloWorld\test_xml.xml");
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
