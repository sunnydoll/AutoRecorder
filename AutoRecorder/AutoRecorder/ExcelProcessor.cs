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
            string path = "Powelton.xlsx";
            string tempPath = "Powelton_temp.xlsx";
            FileInfo tempFile = new FileInfo(tempPath);
            if (tempFile.Exists)
            {
                tempFile.Delete();
            }
            var holdingStream = new MemoryStream();
            var package = new ExcelPackage(new FileInfo(path));
            int sumTabs = package.Workbook.Worksheets.Count;
            for (int h = 1; h <= sumTabs; h++)
            {
                //h <= package.Workbook.Worksheets.Count
                ExcelWorksheet workSheet = package.Workbook.Worksheets[h];
                Console.Out.WriteLine("This is hh " + h + " out of " + sumTabs);
                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    StringBuilder sbAddr = new StringBuilder();
                    HttpHelper httpHelper = new HttpHelper();
                    //Console.Out.WriteLine("This is iii " + i);
                    for (int j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
                    {
                        object cellValue = workSheet.Cells[i, j].Value;
                        //Console.Out.WriteLine("This is jjjj " + j);
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
                            if (httpHelper.OPA != "" && httpHelper.addr != "")
                            {
                                httpHelper.OPACall();
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            if(j == 4)
                                workSheet.Cells[i, j].Value = httpHelper.prop.Owner;
                            else if (j == 5)
                                workSheet.Cells[i, j].Value = httpHelper.prop.MailingAddress;
                            else if(j == 6)
                                workSheet.Cells[i, j].Value = httpHelper.prop.MailingAddressCity;
                            else if (j == 7)
                                workSheet.Cells[i, j].Value = httpHelper.prop.MailingAddressZipCode;
                            else if (j == 8)
                                workSheet.Cells[i, j].Value = httpHelper.prop.LatestMarketValue;
                            else if (j == 9)
                                workSheet.Cells[i, j].Value = httpHelper.prop.ExemptLand;
                            else if (j == 10)
                                workSheet.Cells[i, j].Value = httpHelper.prop.ExemptImprovement;
                            else if (j == 11)
                                workSheet.Cells[i, j].Value = httpHelper.prop.HomesteadExemption;
                            else if (j == 12)
                                workSheet.Cells[i, j].Value = httpHelper.prop.Zoning;

                            //break;
                        }
                    }
                    //Console.WriteLine(httpHelper.prop.Owner);
                    //Console.WriteLine(httpHelper.prop.MailingAddress);
                }
                package.SaveAs(tempFile);
                holdingStream.SetLength(0);
                package.Stream.Position = 0;
                package.Stream.CopyTo(holdingStream);
                package.Load(holdingStream);
            }
            package.Save();
            package.Dispose();
            holdingStream.Dispose();

        }
    }
}
