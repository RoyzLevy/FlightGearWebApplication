using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace FlightGearWebApp.Models
{
    /// <summary>
    /// Class NetworkConnection is responsible for connection to server and
    /// receiving values back from it by sending commands in TCP protocol.
    /// The class can also be written to XML file for later sending data to the
    /// client side for display.
    /// </summary>
    public class NetworkConnection
    {
        Thread connectThread;
        private const int tryPulse = 500;
        public volatile bool stop = true;
        private TcpClient myTcpClient;
        public string Ip { get; set; }
        public int Port { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public double Throttle { get; set; }
        public double Rudder { get; set; }

        /// <summary>
        /// This function connects to flight gear server for sending data.
        /// </summary>
        public void Connect()
        {
            this.Disconnect();
            stop = false;

            this.myTcpClient = new TcpClient();
            this.connectThread = new Thread(() =>
            {
                // Keep trying to connect to server.
                while (!myTcpClient.Connected)
                {
                    try
                    {
                        myTcpClient.Connect(Ip, Port);
                        Thread.Sleep(tryPulse);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Trying to connect to FG");
                    }
                }
                // Upon reaching here program has been connected.
            });
            this.connectThread.Start();
        }


        /// <summary>
        /// This function disconnects client from server.
        /// </summary>
        public void Disconnect()
        {
            // if already disconnected - do nothing.
            if (stop)
            {
                return;
            }
            // else - abort running connections.
            connectThread.Abort();
            this.myTcpClient.Close();
            stop = true;
        }

        /// <summary>
        /// This function receives a string and parses it 
        /// </summary>
        /// <param name="toBeParsed">string for the parsing.</param>
        /// <returns>parsed value that was received from server.</returns>
        public string ParseValue(string toBeParsed)
        {
            string[] result = toBeParsed.Split('=');
            result = result[1].Split('\'');
            result = result[1].Split('\'');

            return result[0];
        }

        /// <summary>
        /// This function sends a command to the server through TCP protocol.
        /// </summary>
        public void Write()
        {
            // if the connection was stopped - no send is needed.
            if (stop)
            {
                return;
            }
            string command = "";
            NetworkStream writeStream = this.myTcpClient.GetStream();  //creates a network stream

            // Send Lon command in order to get it's value from server.
            command = "get /position/longitude-deg\r\n";
            int byteCount = Encoding.ASCII.GetByteCount(command); //how many bytes
            byte[] sendData = new byte[byteCount];  //create a buffer
            sendData = Encoding.ASCII.GetBytes(command);   //puts the message in the buffer

            writeStream.Write(sendData, 0, sendData.Length); //network stream to transfer what's in buffer
            StreamReader STR = new StreamReader(writeStream);
            //Debug.WriteLine("Recieved from server " + STR.ReadLine());
            string lon = ParseValue(STR.ReadLine());
            Lon = double.Parse(lon);

            // Send Lat command in order to get it's value from server.
            command = "get /position/latitude-deg\r\n";
            byteCount = Encoding.ASCII.GetByteCount(command); //how many bytes
            sendData = new byte[byteCount];  //create a buffer
            sendData = Encoding.ASCII.GetBytes(command);   //puts the message in the buffer

            writeStream.Write(sendData, 0, sendData.Length); //network stream to transfer what's in buffer
            STR = new StreamReader(writeStream);
            //Debug.WriteLine("Recieved from server " + STR.ReadLine());
            string lat = ParseValue(STR.ReadLine());
            Lat = double.Parse(lat);

            // Send Throttle command in order to get it's value from server.
            command = "get /controls/engines/engine/throttle\r\n";
            byteCount = Encoding.ASCII.GetByteCount(command); //how many bytes
            sendData = new byte[byteCount];  //create a buffer
            sendData = Encoding.ASCII.GetBytes(command);   //puts the message in the buffer

            writeStream.Write(sendData, 0, sendData.Length); //network stream to transfer what's in buffer
            STR = new StreamReader(writeStream);
            //Debug.WriteLine("Recieved from server " + STR.ReadLine());
            string throttle = ParseValue(STR.ReadLine());
            Throttle = double.Parse(throttle);

            // Send Rudder command in order to get it's value from server.
            command = "get /controls/flight/rudder\r\n";
            byteCount = Encoding.ASCII.GetByteCount(command); //how many bytes
            sendData = new byte[byteCount];  //create a buffer
            sendData = Encoding.ASCII.GetBytes(command);   //puts the message in the buffer

            writeStream.Write(sendData, 0, sendData.Length); //network stream to transfer what's in buffer
            STR = new StreamReader(writeStream);
            //Debug.WriteLine("Recieved from server " + STR.ReadLine());
            string rudder = ParseValue(STR.ReadLine());
            Rudder = double.Parse(rudder);
        }

        /// <summary>
        /// This function receives an XML writer and writes the details of the Network Connection
        /// to it. This is useful for later sending the XML to the client side for displaying values.
        /// </summary>
        /// <param name="writer"></param>
        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("NetworkConnection");
            writer.WriteElementString("Ip", this.Ip);
            writer.WriteElementString("Port", this.Port.ToString());
            writer.WriteElementString("Lon", this.Lon.ToString());
            writer.WriteElementString("Lat", this.Lat.ToString());
            writer.WriteElementString("Throttle", this.Throttle.ToString());
            writer.WriteElementString("Rudder", this.Rudder.ToString());
            writer.WriteEndElement();
        }
    }
}