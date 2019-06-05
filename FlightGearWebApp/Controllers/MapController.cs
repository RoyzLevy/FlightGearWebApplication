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
    /// <summary>
    /// Map Controller is responible for the policy of coordination between the model and the view
    /// </summary>
    public class MapController : Controller
    {
        /// <summary>
        /// Get the Index view
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get the Display View
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public ActionResult Display(string ip, int port, int time = 0)
        {
            Debug.WriteLine("Hello from MapController with network display unknown!");
            IPAddress ipAddress;
            if (IPAddress.TryParse(ip, out ipAddress))
            {
                // if the route action should be simple Display.
                // return the normal Display with default time.
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
            // else, the ip is a file name thus the port is the Display-rate
            // and will return the Display from the given file.
                InfoModel.Instance.FilePath = AppDomain.CurrentDomain.BaseDirectory + ip + ".csv";
                InfoModel.Instance.Time = port;
            
                Session["time"] = port;
                Session["isNetworkDisplay"] = "0";
                InfoModel.Instance.OpenFileRead(InfoModel.Instance.FilePath);
                
            return View();
        }

        /// <summary>
        /// Get the Save View
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="time"></param>
        /// <param name="timeout"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ActionResult Save(string ip, int port, int time, int timeout, string filePath)
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

        /// <summary>
        /// A request from the view to the model to open a new file
        /// </summary>
        /// <returns>The file name</returns>
        [HttpPost]
        public string OpenNewFile()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.OpenFileWrite(fileName);
            return fileName;
        }

        /// <summary>
        /// A request from the view to the model to write to the currently opened for writing file
        /// </summary>
        /// <returns>The file name</returns>
        [HttpPost]
        public string WriteToFile()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.WriteToFile(fileName);

            return fileName;
        }

        /// <summary>
        /// A request from the view to the model to close the currently opened for reading file
        /// </summary>
        /// <returns>The file name</returns>
        [HttpPost]
        public string CloseFileRead()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CloseFileRead(fileName);

            return fileName;
        }

        /// <summary>
        /// A request from the view to the model to close the currently opened for writing file
        /// </summary>
        /// <returns>The file name</returns>
        [HttpPost]
        public string CloseFileWrite()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CloseFileWrite(fileName);

            return fileName;
        }

        /// <summary>
        /// A request from the view to the model to get an xml representation of the model
        /// </summary>
        /// <returns>An xml representation of the model</returns>
        [HttpPost]
        public string GetInfoModelXML()
        {
            var model = InfoModel.Instance;

            // if there are more file lines, update the model info from file.
            if (model.isMoreFileLines)
            {
                model.ReadFileValues();
            }
            return this.InfoModelToXML(model);
        }

        /// <summary>
        /// A request from the view to the model to disconnect from Flight Gear
        /// </summary>
        [HttpPost]
        public void Disconnect()        //TODO: check if it can be void
        {
            Debug.WriteLine("Hello from MapController in pre network disconnect");
            var network = InfoModel.Instance.NetworkConnection;
            network.Disconnect();
        }

        /// <summary>
        /// A request from the view to the model to get an xml representation of the network class
        /// </summary>
        /// <returns>An xml representation of the network class</returns>
        [HttpPost]
        public string GetNetworkXML()
        {
            var network = InfoModel.Instance.NetworkConnection;

            network.Write(); // read lat and lon from server to network object.

            return this.NetworkToXML(network);
        }

        /// <summary>
        /// This function wraps the network class to an xml form 
        /// </summary>
        /// <param name="network">The network class to be wrapped</param>
        /// <returns>A string representing the network class</returns>
        private string NetworkToXML(NetworkConnection network)
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

        /// <summary>
        /// This function wraps the model class to an xml form 
        /// </summary>
        /// <param name="model">The model class to be wrapped</param>
        /// <returns>A string representing the model class</returns>
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