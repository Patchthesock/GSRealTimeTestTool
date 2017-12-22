using System.IO;
using Models;
using UnityEngine;

namespace Services
{
    public class CsvWriterService
    {
        public void CreateFile(string filepath)
        {
            _filepath = filepath;
            Debug.Log("Writing to path: " + filepath);
            WriteLine("Message,Direction,Size,OpCode,Sender,Stream Length,"
                + "Lag,Round Trip Time,Ping Time,Pong Time,Created At\r");
        }

        public void WriteLogEntry(LogEntry l)
        {
            WriteLine(CreateCsvEntry(l));
        }

        private void WriteLine(string l)
        {
            if (string.IsNullOrEmpty(_filepath)) return;
            var writer = new StreamWriter(_filepath, true);
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

        private string _filepath;
    }
}