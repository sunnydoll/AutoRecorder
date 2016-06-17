using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AutoRecorder
{
    class ExcelProcessor
    {
        //static string[] addrList = new string[2] { "300 N. 33rd St.", "400 N. 33rd St." }; 
        static void Main(string[] args)
        {
            var package = new ExcelPackage(new FileInfo("C:\\workspace\\AutoRecorder\\AutoRecorder\\Powelton.xlsx"));
            for (int h = 1; h <= package.Workbook.Worksheets.Count; h++)
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[h];
                Console.Out.WriteLine("This is hh " + h);
                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    StringBuilder sbAddr = new StringBuilder();
                    string address = "";
                    string OPA = "";
                    Console.Out.WriteLine("This is iii " + i);
                    for (int j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
                    {
                        object cellValue = workSheet.Cells[i, j].Value;
                        Console.Out.WriteLine("This is jjjj " + j);
                        if(cellValue == null) {
                            break;
                        }
                        else if (j == 1)
                        {
                            sbAddr.Append(cellValue.ToString() + " ");
                        }
                        else if (j == 2)
                        {
                            sbAddr.Append(cellValue.ToString());
                            address = sbAddr.ToString();
                        }
                        else if (j == 3)
                        {
                            OPA = cellValue.ToString();
                        }
                        else
                        {
                            break;
                        }
                    }
                    Console.Out.WriteLine(address);
                    Console.Out.WriteLine(OPA);
                    HttpHelper httpHelper = new HttpHelper(address, OPA);

                }
            }
            
            //HttpHelper helper = new HttpHelper("409 N 36TH ST");
            //helper.firstCall();
        }
    }
}
