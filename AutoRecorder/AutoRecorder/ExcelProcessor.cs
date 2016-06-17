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
            var package = new ExcelPackage(new FileInfo("Powelton_Updated.xlsx"));
            for (int h = 1; h <= 1; h++)
            {
                //h <= package.Workbook.Worksheets.Count
                ExcelWorksheet workSheet = package.Workbook.Worksheets[h];
                Console.Out.WriteLine("This is hh " + h);
                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    StringBuilder sbAddr = new StringBuilder();
                    HttpHelper httpHelper = new HttpHelper();
                    Console.Out.WriteLine("This is iii " + i);
                    for (int j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
                    {
                        object cellValue = workSheet.Cells[i, j].Value;
                        Console.Out.WriteLine("This is jjjj " + j);
                        if(cellValue == null && j < 4) {
                            break;
                        }
                        else if (j == 1)
                        {
                            sbAddr.Append(cellValue.ToString() + " ");
                        }
                        else if (j == 2)
                        {
                            sbAddr.Append(cellValue.ToString());
                            httpHelper.addr = sbAddr.ToString();
                        }
                        else if (j == 3)
                        {
                            httpHelper.OPA = cellValue.ToString();
                        }
                        else if (httpHelper.OPA == "" || httpHelper.addr == "")
                        {
                            break;
                        }
                        else
                        {
                            Console.Out.WriteLine(httpHelper.addr);
                            Console.Out.WriteLine(httpHelper.OPA);

                            //break;
                        }
                    }

                }
            }
            
            //HttpHelper helper = new HttpHelper("409 N 36TH ST");
            //helper.firstCall();
        }
    }
}
