using System;
using System.IO;
using Models;
using UnityEngine;

namespace Services
{
    public class CsvWriterService
    {
        public CsvWriterService()
        {
            _path = Application.dataPath + "/Log/";
            _filename = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".csv";
        }
        
        /**
         * <summary>Create a Packet Log CSV File</summary>
         */
        public void CreateFile()
        {
            Debug.Log("Creating: " + _path + _filename);
            WriteLine("Message,Direction,Size,OpCode,Sender,Request ID,"
                + "Lag,Round Trip Time,kbits per Second,Ping,Pong,Created At\r");
        }

        /**
         * <summary>Writes a LogEntry to the current file</summary>
         * <param name="l">LogEntry</param>
         */
        public void WriteLogEntry(LogEntry l)
        {
            WriteLine(CreateCsvEntry(l));
        }

        private void WriteLine(string l)
        {
            if (string.IsNullOrEmpty(_path)) return;
            if (string.IsNullOrEmpty(_filename)) return;
            if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
            var writer = new StreamWriter(_path + _filename, true);
            writer.WriteLine(l);
            writer.Close();
        }
        
        private static string CreateCsvEntry(LogEntry l)
        {
            var s = l.Message + ",";
            s += l.Direction + ",";
            s += l.PacketDetail.Size + ",";
            s += l.PacketDetail.OpCode + ",";
            s += l.PacketDetail.Sender + ",";
            s += l.PacketDetail.RequestId + ",";
            if (l.LatencyDetail == null) return s + ",,,,," + l.CreatedAt + "\r";
            s += l.LatencyDetail.Lag + ",";
            s += l.LatencyDetail.RoundTrip + ",";
            s += l.LatencyDetail.Speed + ",";
            s += l.LatencyDetail.PingTime + ",";
            s += l.LatencyDetail.PongTime + ",";
            return s + l.CreatedAt + "\r";
        }

        private readonly string _path;
        private readonly string _filename;
    }
}