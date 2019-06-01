
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlightGearWebApp.Models;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace FlightGearWebApp.Models
{
    public class InfoModel
    {
        private static InfoModel s_instace = null;

        public static InfoModel Instance
        {
            get
            {
                if (s_instace == null)
                {
                    Debug.WriteLine("network in info model was null");
                    s_instace = new InfoModel();
                }
                Debug.WriteLine("Returning network - from the GetNetwrok retquest from js");
                return s_instace;
            }
        }

        public int Time { get; set; }
        public int Timeout { get; set; }
        public string FilePath { get; set; }
        public float Lat { get; private set; }
        public float Lon { get; private set; }
        public bool isMoreFileLines { get; private set; }
        private StreamWriter streamWriter;
        private StreamReader streamReader;

        public NetworkConnection NetworkConnection { get; private set; }

        private InfoModel()
        {
            NetworkConnection = new NetworkConnection();
        }

        public void ConnectNetwork()
        {
            NetworkConnection.Connect();
        }

        public void OpenFileWrite(string filePath)
        {
            this.streamWriter = new StreamWriter(filePath);
        }

        public void WriteToFile(string filePath)
        {
            string toWrite = this.NetworkConnection.Lon.ToString() + "," + this.NetworkConnection.Lat.ToString();
            this.streamWriter.WriteLineAsync(toWrite); // the writing needs to be done in another func.
        }

        public void CloseFileWrite(string filePath)
        {
            this.streamWriter.Close();
        }



        public void OpenFileRead(string filePath)
        {
            this.isMoreFileLines = true;
            this.streamReader = new StreamReader(filePath);
        }

        public void ReadFileValues()
        {
            string line = streamReader.ReadLine();
            if (!String.IsNullOrEmpty(line))
            {
                string[] values = line.Split(',');

                this.Lon = float.Parse(values[0]);
                this.Lat = float.Parse(values[1]);
            } else
            {
                this.isMoreFileLines = false;
            }
        }

        public void CloseFileRead(string filePath)
        {
            this.streamReader.Close();
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("InfoModel");
            writer.WriteElementString("Lat", this.Lat.ToString());
            writer.WriteElementString("Lon", this.Lon.ToString());
            writer.WriteEndElement();
        }
    }
}