using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


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
            FileInfo resFile = new FileInfo(path);
            if (tempFile.Exists)
            {
                tempFile.Delete();
            }
            var holdingStream = new MemoryStream();
            var package = new ExcelPackage(new FileInfo(path));
            int sumTabs = package.Workbook.Worksheets.Count;
            for (int h = 1; h <= package.Workbook.Worksheets.Count; h++)
            {
                //h <= package.Workbook.Worksheets.Count
                ExcelWorksheet workSheet = package.Workbook.Worksheets[h];
                Console.Out.WriteLine("Start processing the No. " + h + " tab out of " + sumTabs);
                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    StringBuilder sbAddr = new StringBuilder();
                    HttpHelper httpHelper = new HttpHelper();
                    for (int j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
                    {
                        object cellValue = workSheet.Cells[i, j].Value;
                        if (cellValue == null && j < 4)
                        {
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
                            //if (workSheet.Cells[i, j + 1].Value != null)
                            //{
                            //    Console.WriteLine(httpHelper.addr);
                            //    Console.WriteLine("Already Have Value.");
                            //    break;
                            //}
                            httpHelper.OPA = cellValue.ToString();
                            if (httpHelper.OPA != "" && httpHelper.addr != "")
                            {
                                httpHelper.TaxCall();
                                //Console.Write("Press any key to continue... ");
                                //Console.ReadKey();
                                //Console.WriteLine("Finishing tax call for 3 seconds....");
                                //System.Threading.Thread.Sleep(3000);
                                //Console.WriteLine("Tax Call Done");
                                //httpHelper.OPACall();
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (j == 4)
                            {
                                //workSheet.Cells[i, j].Value = httpHelper.prop.Owner;
                                //workSheet.Cells[i, j + 1].Value = httpHelper.prop.MailingAddress;
                                //workSheet.Cells[i, j + 2].Value = httpHelper.prop.MailingAddressCity;
                                //workSheet.Cells[i, j + 3].Value = httpHelper.prop.MailingAddressZipCode;
                                //workSheet.Cells[i, j + 4].Value = httpHelper.prop.SalesDate.ToString();
                                //workSheet.Cells[i, j + 5].Value = httpHelper.prop.SalesPrice;
                                //workSheet.Cells[i, j + 6].Value = httpHelper.prop.LatestMarketValue;
                                //workSheet.Cells[i, j + 7].Value = httpHelper.prop.ExemptLand;
                                //workSheet.Cells[i, j + 8].Value = httpHelper.prop.ExemptImprovement;
                                //workSheet.Cells[i, j + 9].Value = httpHelper.prop.HomesteadExemption;
                                //workSheet.Cells[i, j + 10].Value = httpHelper.prop.Zoning;
                                workSheet.Cells[i, j + 11].Value = httpHelper.prop.TaxOwed;
                                break;
                            }
                        }
                    }
                    Console.WriteLine("Finishing one property for 3 seconds....");
                    System.Threading.Thread.Sleep(3000);
                    Console.WriteLine("One property Done");
                }
                package.SaveAs(tempFile);
                holdingStream.SetLength(0);
                package.Stream.Position = 0;
                package.Stream.CopyTo(holdingStream);
                package.Load(holdingStream);
                Console.WriteLine("Finishing one tab for 5 seconds....");
                System.Threading.Thread.Sleep(5000);
                Console.WriteLine("Finish holding");
            }
            package.Save();
            package.Dispose();
            holdingStream.Dispose();
            Console.Write("Press any key to exit... ");
            Console.ReadKey();
        }
    }
}
