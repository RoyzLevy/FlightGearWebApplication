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
    public class NetworkConnection
    {
        private static Random rnd = new Random();
        Thread connectThread; //thread will try and reach a server
        int tryPulse = 500;
        public volatile bool stop = true;
        private TcpClient myTcpClient;
        public static Mutex mutex = new Mutex();

        public string Ip{ get; set;}
        public int Port { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        /// <summary>
        /// Connect opens program as server and looks for a server to reach
        /// </summary>
        /// <param name="ip">adress of ip</param>
        /// <param name="receivePort"></param>
        /// <param name="sendPort"></param>
        public void Connect()
        {
            Debug.WriteLine("NC - Mutex was claimed");
            if (!stop)
            {
                Debug.WriteLine("NC - Mutex was released from !stop -> return");
                return;
            }
            stop = false;

            this.myTcpClient = new TcpClient();
            this.connectThread = new Thread(() =>
            {
                mutex.WaitOne();
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
                        /** Keep trying */
                    }
                }
                /** Upon reaching here program has been connected */
                Debug.WriteLine("Connected!");
                mutex.ReleaseMutex();
                Debug.WriteLine("NC - Mutex was released from connect");
            });
            this.connectThread.Start();
        }
        /// <summary>
        /// This function closes program as a server and a client.
        /// </summary>
        public void Disconnect()
        {
            if (stop)
            {
                return;
            }
            connectThread.Abort();
            this.myTcpClient.Close();
            stop = true;
        }


        public string ParseValue(string toBeParsed)
        {
            string[] result = toBeParsed.Split('=');
            result = result[1].Split('\'');
            result = result[1].Split('\'');

            return result[0];
        }

        /// <summary>
        /// This function Writes a command to server
        /// </summary>
        /// <param name="command">command to server</param>
        public void Write()   //later make it with no args
        {
            string command = "";
            Debug.WriteLine("In NC - Trying to write");
            mutex.WaitOne(); //one command at a time
            Debug.WriteLine("Write claimed mutex");
            NetworkStream writeStream = this.myTcpClient.GetStream();  //creates a network stream
            command = "get /position/longitude-deg\r\n";
            int byteCount = Encoding.ASCII.GetByteCount(command); //how many bytes
            byte[] sendData = new byte[byteCount];  //create a buffer
            sendData = Encoding.ASCII.GetBytes(command);   //puts the message in the buffer

            writeStream.Write(sendData, 0, sendData.Length); //network stream to transfer what's in buffer
            Debug.WriteLine("Just before printing what got back from the server");
            StreamReader STR = new StreamReader(writeStream);
            //Debug.WriteLine("Recieved from server " + STR.ReadLine());
            string lon = ParseValue(STR.ReadLine());
            Lon = double.Parse(lon);


            //Lat
            command = "get /position/latitude-deg\r\n";
            byteCount = Encoding.ASCII.GetByteCount(command); //how many bytes
            sendData = new byte[byteCount];  //create a buffer
            sendData = Encoding.ASCII.GetBytes(command);   //puts the message in the buffer

            writeStream.Write(sendData, 0, sendData.Length); //network stream to transfer what's in buffer
            Debug.WriteLine("Just before printing what got back from the server");
            STR = new StreamReader(writeStream);
            //Debug.WriteLine("Recieved from server " + STR.ReadLine());
            string lat = ParseValue(STR.ReadLine());
            Lat = double.Parse(lat);



            mutex.ReleaseMutex();
            Debug.WriteLine("Write released mutex");
        }

    public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("NetworkConnection");
            writer.WriteElementString("Ip", this.Ip);
            writer.WriteElementString("Port", this.Port.ToString());
            writer.WriteElementString("Lat", this.Lat.ToString());
            writer.WriteElementString("Lon", this.Lon.ToString());
            writer.WriteEndElement();
        }
    }
}