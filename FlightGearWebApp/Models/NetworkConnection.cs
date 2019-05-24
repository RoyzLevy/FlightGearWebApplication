using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace FlightGearWebApp.Models
{
    public class NetworkConnection
    {
        private static Random rnd = new Random();
        public string Ip{ get; set;}
        public int Port { get; set; }
        public int Lat { get; set; }
        public int Lon { get; set; }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("NetworkConnection");
            writer.WriteElementString("Ip", this.Ip);
            writer.WriteElementString("Port", this.Port.ToString());
            writer.WriteElementString("Lat", this.Lat.ToString());
            writer.WriteElementString("Lon", this.Lon.ToString());
            writer.WriteEndElement();
        }





        public void Connect()
        {
            //connect to FG server.
        }
        public void read()
        {
            //read from FG server.
            this.Lat = rnd.Next(1000);
            this.Lon = rnd.Next(1000);
        }

    }
}