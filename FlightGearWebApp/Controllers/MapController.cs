using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlightGearWebApp.Models;
using System.Xml;
using System.Text;
using System.Net;

namespace FlightGearWebApp.Controllers
{
    public class MapController : Controller
    {
        const int MaxTimeInterval = 1000000;
        // GET: Map Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Map display
        public ActionResult display(string ip, int port, int time = MaxTimeInterval)
        {
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
                    InfoModel.Instance.StartNetWorkRead(); // connect to server for reading.

                    Session["time"] = time;
                    Session["isNetworkDisplay"] = "1";
                    return View();
                }
            }
            // else, the ip is file name and the port is display-rate
            // and will return the display from the given file.
                InfoModel.Instance.FilePath = ip;
                InfoModel.Instance.Time = time;
            
                Session["time"] = time;
                Session["isNetworkDisplay"] = "0";

            return View();
        }

        public ActionResult save(string ip, int port, int time, int timeout, string filePath )
        {
            InfoModel.Instance.NetworkConnection.Ip = ip;
            InfoModel.Instance.NetworkConnection.Port = port;
            InfoModel.Instance.Time = time;
            InfoModel.Instance.Timeout = timeout;
            InfoModel.Instance.FilePath = AppDomain.CurrentDomain.BaseDirectory + filePath + ".csv";
            InfoModel.Instance.StartNetWorkRead(); // connect to server for reading.

            Session["time"] = time;
            Session["timeout"] = timeout;

            return View();
        }

        [HttpPost]
        public string OpenNewFile()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CreateFile(fileName);

            return fileName;
        }

        [HttpPost]
        public string WriteToFile()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.WriteToFile(fileName);

            return fileName;
        }

        [HttpPost]
        public string CloseFile()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CloseFile(fileName);

            return fileName;
        }

        // These function initializes an XML format of the Network object.
        [HttpPost]
        public string GetNetwork()
        {
            var network = InfoModel.Instance.NetworkConnection;

            network.read(); // read lat and lon from server to network object.

            return ToXml(network);
        }
        private string ToXml(NetworkConnection network)
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

    }

}