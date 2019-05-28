using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlightGearWebApp.Models;
using System.IO;
using System.Text;
using System.Diagnostics;

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

        public NetworkConnection NetworkConnection { get; private set; }

        private InfoModel()
        {
            NetworkConnection = new NetworkConnection();
        }

        public void Start()
        {
            NetworkConnection.Connect();
        }

        public void CreateFile(string filePath)
        {
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.WriteLineAsync(NetworkConnection.Lon.ToString()); // the writing needs to be done in another func.
            streamWriter.WriteLineAsync(NetworkConnection.Lon.ToString()); // the writing needs to be done in another func.
            streamWriter.WriteLineAsync("\n"); // the writing needs to be done in another func.
            streamWriter.Close(); // closing will also be in it's own func.
        }
    }
}