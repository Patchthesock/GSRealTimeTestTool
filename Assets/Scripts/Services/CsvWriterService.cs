using System;
using System.IO;
using Models.LogEntry;
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
            WriteLine("Created At,Log Type,Direction,Information,Size,OpCode,Sender,Request ID,"
                + "Lag,Round Trip Time,kbit/s,Ping,Pong\r");
        }

        /**
         * <summary>Writes a LogEntry to the current file</summary>
         * <param name="l">LogEntry</param>
         */
        public void WriteLogEntry(ILogEntry l)
        {
            WriteLine(GetCsvEntry(l));
        }

        private void WriteLine(string l)
        {
            if (string.IsNullOrEmpty(l)) return;
            if (string.IsNullOrEmpty(_path)) return;
            if (string.IsNullOrEmpty(_filename)) return;
            if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
            var writer = new StreamWriter(_path + _filename, true);
            writer.WriteLine(l);
            writer.Close();
        }
        
        private static string GetCsvEntry(ILogEntry l)
        {
            switch (l.GetLogEntryType())
            {
                // Match Log Entries
                case LogEntryTypes.MatchFound: return GetSimpleLogEntryCsvEntry(l);
                case LogEntryTypes.MatchNotFound: return GetSimpleLogEntryCsvEntry(l);
                case LogEntryTypes.MatchMakingRequest: return GetSimpleLogEntryCsvEntry(l);
                
                // Session Control Entries
                case LogEntryTypes.OnSessionJoin: return GetSimpleLogEntryCsvEntry(l);
                case LogEntryTypes.OnSessionReady: return GetSimpleLogEntryCsvEntry(l);
                case LogEntryTypes.OnSessionLeave: return GetSimpleLogEntryCsvEntry(l);
                case LogEntryTypes.OnPlayerConnect: return GetSimpleLogEntryCsvEntry(l);
                case LogEntryTypes.OnPlayerDisconnect: return GetSimpleLogEntryCsvEntry(l);
                
                // Packet Entries
                case LogEntryTypes.PingPacket: 
                    break;
                case LogEntryTypes.PongPacket:
                    break;
                case LogEntryTypes.BlankPacket:
                    break;
                case LogEntryTypes.UnstructuredPacket:
                    break;
                
                // Quality of Service Test Result Entry
                case LogEntryTypes.QualityOfServiceTestResult:
                    break;
                default: return GetSimpleLogEntryCsvEntry(l);
            }
            //var s = l.Message + ",";
            //s += l.Direction + ",";
            //s += l.PacketDetail.Size + ",";
            //s += l.PacketDetail.OpCode + ",";
            //s += l.PacketDetail.Sender + ",";
            //s += l.PacketDetail.RequestId + ",";
            //if (l.LatencyDetail == null) return s + ",,,,," + l.CreatedAt + "\r";
            //s += l.LatencyDetail.Lag + ",";
            //s += l.LatencyDetail.RoundTrip + ",";
            //s += l.LatencyDetail.Speed + ",";
            //s += l.LatencyDetail.PingTime + ",";
            //s += l.LatencyDetail.PongTime + ",";
            //return s + l.CreatedAt + "\r";
            return string.Empty;
        }

        private static string GetSimpleLogEntryCsvEntry(ILogEntry l)
        {
            var s = l.GetCreatedAt() + ",";
            s += l.GetLogEntryType() + ",";
            s += l.GetFullInfo() + ",";
            s += l.GetDirection() + ",";
            return s;
        }

        private readonly string _path;
        private readonly string _filename;
    }
}