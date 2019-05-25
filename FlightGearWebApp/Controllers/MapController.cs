using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlightGearWebApp.Models;
using System.Xml;
using System.Text;

namespace FlightGearWebApp.Controllers
{
    public class MapController : Controller
    {
        // GET: Map Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Map display
        public ActionResult display(string ip, int port, int time = 1000000)
        {
            InfoModel.Instance.NetworkConnection.Ip = ip;
            InfoModel.Instance.NetworkConnection.Port = port;
            InfoModel.Instance.Time = time;
            InfoModel.Instance.Start(); // connect to server for reading.

            Session["time"] = time;

            return View();
        }

        public ActionResult save(string ip, int port, int time, int timeout, string fileName )
        {
            InfoModel.Instance.NetworkConnection.Ip = ip;
            InfoModel.Instance.NetworkConnection.Port = port;
            InfoModel.Instance.Time = time;
            InfoModel.Instance.Timeout = timeout;
            InfoModel.Instance.FileName = fileName;
            InfoModel.Instance.Start(); // connect to server for reading.

            Session["time"] = time;

            return View();
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