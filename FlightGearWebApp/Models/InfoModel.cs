
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlightGearWebApp.Models;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
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
                    s_instace = new InfoModel();
                }
                return s_instace;
            }
        }

        public int Time { get; set; }
        public int Timeout { get; set; }
        public string FilePath { get; set; }
        public float Lat { get; private set; }
        public float Lon { get; private set; }
        public bool isMoreFileLines { get; private set; }
        public string isEOF { get; set; }
        private StreamWriter streamWriter;
        private StreamReader streamReader;

        public NetworkConnection NetworkConnection { get; private set; }
        public static Mutex WriteStreaMutex = new Mutex();
        public static Mutex WriteFileMutex = new Mutex();

        private InfoModel()
        {
            NetworkConnection = new NetworkConnection();
        }

        public void ConnectNetwork()
        {
            NetworkConnection.Connect();
        }

        public void CreateFile(string filePath)
        {
            Debug.WriteLine("creates a new string writer");
            this.streamWriter = new StreamWriter(filePath);
        }

        public void OpenFileWrite(string filePath)
        {
            this.streamWriter = new StreamWriter(filePath);
        }

        public void WriteToFile(string filePath)
        {
            string toWrite = this.NetworkConnection.Lon.ToString() + "," + this.NetworkConnection.Lat.ToString();
            this.streamWriter.WriteLine(toWrite); // the writing needs to be done in another func.
        }

        public void OpenFileRead(string filePath)   //NEW
        {
            this.isMoreFileLines = true;
            this.streamReader = new StreamReader(filePath);
            if (File.Exists(filePath))
            {
                Debug.WriteLine("file exist!");
            }
            else
            {
                Debug.WriteLine("file not exist!");
            }
        }

        public void ReadFileValues()    //NEW
        {
            string line = streamReader.ReadLine();
            string[] values = line.Split(',');
            if (values[0].Equals("$"))
            {
                this.isEOF = "1";
                this.isMoreFileLines = false;
                this.CloseFileRead(this.FilePath);
            }
            else
            {
                this.Lon = float.Parse(values[0]);
                this.Lat = float.Parse(values[1]);
                Debug.WriteLine(Lon);
                Debug.WriteLine(Lat);
            }
        }

        public void CloseFileRead(string filePath)  //NEW
        {
            this.streamReader.Close();
        }

        public void CloseFileWrite(string filePath) //NEW
        {
            this.streamWriter.Close();
        }
        public void ToXml(XmlWriter writer) //NEW. why should a ToXml be in model?
        {
            writer.WriteStartElement("InfoModel");
            writer.WriteElementString("Lat", this.Lat.ToString());
            writer.WriteElementString("Lon", this.Lon.ToString());
            writer.WriteElementString("isEOF",this.isEOF);
            writer.WriteEndElement();
        }
    }
}