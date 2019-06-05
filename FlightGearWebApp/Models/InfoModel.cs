
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
    /// <summary>
    /// The info model is responsible for aquiring info from the simulator.
    /// </summary>
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
        public bool isOpenForWriting { get; private set; }
        public bool isOpenForReading { get; private set; }
        public string isEOF { get; set; }
        private StreamWriter streamWriter;
        private StreamReader streamReader;

        public NetworkConnection NetworkConnection { get; private set; }
        public static Mutex WriteStreaMutex = new Mutex();
        public static Mutex WriteFileMutex = new Mutex();

        /// <summary>
        /// InfoModel constructor 
        /// </summary>
        private InfoModel()
        {
            NetworkConnection = new NetworkConnection();
            isOpenForWriting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ConnectNetwork()
        {
            NetworkConnection.Connect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public void CreateFile(string filePath)
        {
            Debug.WriteLine("creates a new string writer");
            this.streamWriter = new StreamWriter(filePath);
        }

        public void OpenFileWrite(string filePath)
        {
            if (isOpenForWriting) { Debug.WriteLine("Can't open an already opened file to write"); return; }
            this.streamWriter = new StreamWriter(filePath);
            isOpenForWriting = true;
        }

        public void WriteToFile(string filePath)
        {
            if (!isOpenForWriting) { Debug.WriteLine("Can't write to closed file"); return; }
            string toWrite = this.NetworkConnection.Lon.ToString() + "," + this.NetworkConnection.Lat.ToString() + "," + 
                this.NetworkConnection.Throttle.ToString() + "," + this.NetworkConnection.Rudder.ToString();
            this.streamWriter.WriteLine(toWrite); // the writing needs to be done in another func.
            
        }

        public void OpenFileRead(string filePath)   //NEW
        {
            if (isOpenForReading) { Debug.WriteLine("Can't open an already opened file!"); return; }
            this.isMoreFileLines = true;
            isEOF = "0";
            this.streamReader = new StreamReader(filePath);
            isOpenForReading = true;
        }

        public void ReadFileValues()    //NEW
        {
            if (!isOpenForReading) { Debug.WriteLine("Can't read from a closed file!");  return; }
            string line = streamReader.ReadLine();
            if (line == null)
            {
                this.isEOF = "1";
                this.isMoreFileLines = false;
                this.CloseFileRead(this.FilePath);
            }
            else
            {
                string[] values = line.Split(',');
                this.Lon = float.Parse(values[0]);
                this.Lat = float.Parse(values[1]);
                Debug.WriteLine(Lon);
                Debug.WriteLine(Lat);
            }
        }

        public void CloseFileRead(string filePath)  //NEW
        {
            if (!isOpenForReading) { Debug.WriteLine("Can't close a closed file!"); return; }
            this.streamReader.Close();
            isOpenForReading = false;
            Debug.WriteLine("Read File has been closed");
        }

        public void CloseFileWrite(string filePath) //NEW
        {
            if (!isOpenForWriting) { return; }
            Debug.WriteLine("Closed File to write");
            this.streamWriter.Close();
            isOpenForWriting = false;
            Debug.WriteLine("Write to File has been closed");
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