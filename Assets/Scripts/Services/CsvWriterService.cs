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
        
        public void CreateFile()
        {
            Debug.Log("Creating: " + _path + _filename);
            WriteLine("Message,Direction,Size,OpCode,Sender,Stream Length,"
                + "Lag,Round Trip Time,Ping Time,Pong Time,Created At\r");
        }

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
            s += l.PacketDetail.StreamLength + ",";
            if (l.LatencyDetail == null) return s + ",,,," + l.CreatedAt + "\r";
            s += l.LatencyDetail.Lag + ",";
            s += l.LatencyDetail.RoundTrip + ",";
            s += l.LatencyDetail.PingTime + ",";
            s += l.LatencyDetail.PongTime + ",";
            return s + l.CreatedAt + "\r";
        }

        private readonly string _path;
        private readonly string _filename;
    }
}