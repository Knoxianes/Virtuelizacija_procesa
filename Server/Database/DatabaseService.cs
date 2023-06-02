using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Database
{
    public class DatabaseService : IDatabase, ICsvFunction
    {
        private static ushort idCounter = 1;
        string ret = "Send failed";
        public string ParseFile(MemoryStream csv, out List<Audit> errors)
        {
            errors = new List<Audit>();
            List<Load> newValues = new List<Load>();
            
            int line = 1;

            using (StreamReader csvStream = new StreamReader(csv))
            {
                string csvData = csvStream.ReadToEnd();
                string[] csvRows = csvData.Split('\n');
                string[] rows = csvRows.Take(csvRows.Length - 1).ToArray();

                foreach (var row in rows)
                {
                    string[] split = row.Split(',');

                    if (split.Length != 2)
                    {
                        errors.Add(new Audit(0, DateTime.Now, MessageType.WARNING, "Invalid FORMAT "));
                    }
                    else
                    {
                        if (!TimeSpan.TryParse(split[0], out TimeSpan vreme))
                        {
                            errors.Add(new Audit(0, DateTime.Now, MessageType.WARNING, "Invalid TIMESTAMP " ));
                        }
                        else
                        {
                            if (!double.TryParse(split[1], out double value))
                            {
                                errors.Add(new Audit(0, DateTime.Now, MessageType.WARNING, "Invalid VALUE " ));
                            }
                            else
                            {
                                if (value < 0.0)
                                    errors.Add(new Audit(0, DateTime.Now, MessageType.WARNING, "Invalid VALUE " ));

                                else
                                    newValues.Add(new Load(0, DateTime.Today + vreme, value));
                            }
                        }
                    }

                    line += 1;
                }
            }
            if (errors.Count == line - 1)
            {
                errors.Clear();
                errors.Add(new Audit(0, DateTime.Now, MessageType.ERROR, "Structure not valid " ));

                WriteAudit(errors, ConfigurationManager.AppSettings["DatabaseErrors"]);

                return ret;
            }

            int rowsNumber = WriteIntoBase(newValues, errors);           
            if (rowsNumber > 0)
                ret = "Send successfull";
            if (errors.Count > 0)
                ret += " with errors";
            return ret;
        }

        public bool ReadFromBase(out List<Load> data)
        {
            data = new List<Load>();

            using (IFileHandle file = new DatabaseService().OpenFile(ConfigurationManager.AppSettings["DatabaseFile"]))
            {
                XmlDocument xmlBase = new XmlDocument();
                xmlBase.Load(((FileHandle)file).MemoryStream);

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                XmlNodeList xmlData = xmlBase.SelectNodes("//row[TIME_STAMP[contains(., '" + date + "')]]");

                foreach (XmlNode row in xmlData)
                {
                    Load temp = new Load();
                    temp.Id1 = idCounter++;
                    temp.Timestamp1 = DateTime.Parse(row.SelectSingleNode("TIME_STAMP").InnerText);
                    temp.MeasuredValue1 = double.Parse(row.SelectSingleNode("MEASURED_VALUE").InnerText);

                    data.Add(temp);
                }

                file.Dispose();
            }

            return data.Count > 0;
        }


        public int WriteIntoBase(List<Load> data, List<Audit> errors)
        {
            WriteAudit(errors, ConfigurationManager.AppSettings["DatabaseErrors"]);
            return WriteLoad(data, ConfigurationManager.AppSettings["DatabaseFile"]);
        }

        private static int WriteLoad(List<Load> data, string path)
        {
            int writtenRows = 0;

            IFileHandle file = new DatabaseService().OpenFile(path);
            
               XmlDocument xmlLoad = new XmlDocument();
               xmlLoad.Load(((FileHandle)file).MemoryStream);
               ((FileHandle)file).MemoryStream.Position = 0;

               XDocument xmlFile = XDocument.Load(((FileHandle)file).MemoryStream);
               var items = xmlFile.Element("rows");

               foreach (Load temp in data)
               {
                   string search = "//row[TIME_STAMP='" + temp.Timestamp1.ToString("yyyy-MM-dd HH:mm") + "']";
                   XmlNode element = null;

                   try
                   {
                      element = xmlLoad.SelectSingleNode(search);
                   }
                   catch { }


                   if (element == null)
                   {
                       XElement newElement = new XElement("row",
                           new XElement("TIME_STAMP", temp.Timestamp1.ToString("yyyy-MM-dd HH:mm")),
                           new XElement("MEASURED_VALUE", temp.MeasuredValue1.ToString())
                       );

                       items.Add(newElement);
                       xmlFile.Save(ConfigurationManager.AppSettings["DatabaseFile"]);

                       writtenRows++;
                   }
                   else
                   {
                       element.SelectSingleNode("MEASURED_VALUE").InnerText = temp.MeasuredValue1.ToString();
                       xmlLoad.Save(ConfigurationManager.AppSettings["DatabaseFile"]);

                       writtenRows++;
                   }
               }

             file.Dispose();

            return writtenRows;
        }

        private static void WriteAudit(List<Audit> errors, string path)
        {
            using (IFileHandle file = new DatabaseService().OpenFile(path))
            {
                XDocument xmlFile = XDocument.Load(((FileHandle)file).MemoryStream);

                var elements = xmlFile.Descendants("ID");
                int maxRow = 0;

                foreach (var element in elements)
                {
                    int value;
                    if (int.TryParse(element.Value, out value))
                    {
                        if (value > maxRow)
                        {
                            maxRow = value;
                        }
                    }
                }

                if (errors.Count == 0)
                {
                    var items = xmlFile.Element("ITEMS");
                    var newElement = new XElement("row");

                    newElement.Add(new XElement("ID", ++maxRow));
                    newElement.Add(new XElement("TIME_STAMP", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                    newElement.Add(new XElement("MESSAGE_TYPE", "Info"));
                    newElement.Add(new XElement("MESSAGE", "Data sucessfully sent and updated"));

                    items.Add(newElement);
                    xmlFile.Save(ConfigurationManager.AppSettings["DatabaseErrors"]);
                }
                else
                {
                    foreach (var a in errors)
                    {
                        a.Id1 = ++maxRow;

                        var items = xmlFile.Element("ITEMS");
                        var newElement = new XElement("row");

                        newElement.Add(new XElement("ID", a.Id1));
                        newElement.Add(new XElement("TIME_STAMP", a.Timestamp1.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                        newElement.Add(new XElement("MESSAGE_TYPE", a.MessageType1));
                        newElement.Add(new XElement("MESSAGE", a.Message1));

                        items.Add(newElement);
                        xmlFile.Save(ConfigurationManager.AppSettings["DatabaseErrors"]);
                    }
                }

                file.Dispose();
            }
        }

        public IFileHandle OpenFile(string path)
        {
            MemoryStream memStream = new MemoryStream();

            if (!File.Exists(path))
            {
                string root = null;

                if (path.ToLower().Contains("audit"))
                    root = "ITEMS";
                else
                    root = "rows";

                XDeclaration xdec = new XDeclaration("1.0", "utf-8", "no");
                XElement xele = new XElement(root);
                XDocument newXml = new XDocument(xdec, xele);

                newXml.Save(path);
            }

            using (FileStream fStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fStream.CopyTo(memStream);
                fStream.Dispose();
            }

            memStream.Position = 0;

            return new FileHandle(memStream, Path.GetFileName(path));
        }
    }
}
