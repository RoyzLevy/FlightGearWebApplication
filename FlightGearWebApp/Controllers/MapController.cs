using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlightGearWebApp.Models;
using System.Xml;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace FlightGearWebApp.Controllers
{
    public class MapController : Controller
    {
        // GET: Map Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Map draw
        public ActionResult draw(string ip, int port, int time = 0)
        {
            InfoModel.Instance.NetworkConnection.Ip = ip;
            InfoModel.Instance.NetworkConnection.Port = port;
            InfoModel.Instance.Time = time;
            InfoModel.Instance.ConnectNetwork(); // connect to server for reading.

            Session["time"] = time;
            Debug.WriteLine("Bringing draw view back");
            return View();
        }

        // GET: Map display
        public ActionResult display(string ip, int port, int time = 0)
        {
            Debug.WriteLine("Hello from MapController with network display unknown!");
            IPAddress ipAddress;
            if (IPAddress.TryParse(ip, out ipAddress))
            {
                // if the route action should be simple display.
                // return the normal display with default time.
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                    || ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    InfoModel.Instance.NetworkConnection.Ip = ip;
                    InfoModel.Instance.NetworkConnection.Port = port;
                    InfoModel.Instance.Time = time;
                    InfoModel.Instance.ConnectNetwork(); // connect to server for reading.

                    Session["time"] = time;
                    Session["isNetworkDisplay"] = "1";
                    return View();
                }
            }
            // else, the ip is a file name thus the port is the display-rate
            // and will return the display from the given file.
                InfoModel.Instance.FilePath = AppDomain.CurrentDomain.BaseDirectory + ip + ".csv";
                InfoModel.Instance.Time = port;
            
                Session["time"] = port;
                Session["isNetworkDisplay"] = "0";
                InfoModel.Instance.OpenFileRead(InfoModel.Instance.FilePath);
                
            return View();
        }

        public ActionResult save(string ip, int port, int time, int timeout, string filePath)
        {
            InfoModel.Instance.NetworkConnection.Ip = ip;
            InfoModel.Instance.NetworkConnection.Port = port;
            InfoModel.Instance.Time = time;
            InfoModel.Instance.Timeout = timeout;
            InfoModel.Instance.FilePath = AppDomain.CurrentDomain.BaseDirectory + filePath + ".csv";
            InfoModel.Instance.ConnectNetwork(); // connect to server for reading.

            Session["time"] = time;
            Session["timeout"] = timeout;

            return View();
        }

        [HttpPost]
        public string OpenNewFile()
        {
            Debug.WriteLine("creating a file");
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.OpenFileWrite(fileName);

            return fileName;
        }

        [HttpPost]
        public string WriteToFile()
        {
            Debug.WriteLine("start writing");
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.WriteToFile(fileName);

            return fileName;
        }

        [HttpPost]
        public string CloseFileRead()   //NEW
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CloseFileRead(fileName);

            return fileName;
        }

        [HttpPost]
        public string CloseFileWrite()   //NEW
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CloseFileWrite(fileName);

            return fileName;
        }

        [HttpPost]
        public string GetInfoModelXML() //NEW
        {
            var model = InfoModel.Instance;

            // if there are more file lines, update the model info from file.
            if (model.isMoreFileLines)
            {
                model.ReadFileValues();
            }
            return this.InfoModelToXML(model);
        }

        // These function initializes an XML format of the Network object.
        //[HttpPost]
        //public string GetNetwork()          
        //{
        //    var network = InfoModel.Instance.NetworkConnection;
        //    Debug.WriteLine("about to write");
        //    network.Write(); // read lat and lon from server to network object.

        //    return ToXml(network);
        //}


        [HttpPost]
        public string Disconnect()
        {
            Debug.WriteLine("Hello from MapController in pre network disconnect");
            var network = InfoModel.Instance.NetworkConnection;
            network.Disconnect();
            return "Simply-The-Best";
        }
        // These function initializes an XML format of the Network object.
        [HttpPost]
        public string GetNetworkXML()   //NEW
        {
            var network = InfoModel.Instance.NetworkConnection;

            network.Write(); // read lat and lon from server to network object.

            return this.NetworkToXML(network);
        }
        private string NetworkToXML(NetworkConnection network)  //NEW
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("NetworkConnections");

            network.ToXml(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

        //private string ToXml(NetworkConnection network)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    XmlWriterSettings settings = new XmlWriterSettings();
        //    XmlWriter writer = XmlWriter.Create(sb, settings);

        //    writer.WriteStartDocument();
        //    writer.WriteStartElement("NetworkConnections");

        //    network.ToXml(writer);

        //    writer.WriteEndElement();
        //    writer.WriteEndDocument();
        //    writer.Flush();
        //    return sb.ToString();
        //}

        public string InfoModelToXML(InfoModel model)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("InfoModels");

            model.ToXml(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

    }

}