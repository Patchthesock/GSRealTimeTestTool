using System.IO;
using Models.LogEntry;

namespace Services
{
    public class CsvWriterService
    {
        private string _currentPath;
        private string _currentFilename;
        
        /**
         * <summary>Create a Packet Log CSV File</summary>
         */
        public void CreateFile(string path, string filename)
        {
            _currentPath = path;
            _currentFilename = filename;
            WriteLine(path, filename, GetFileTitle());
        }

        /**
         * <summary>Writes a LogEntry to the current file</summary>
         * <param name="l">LogEntry</param>
         */
        public void WriteLogEntry(ILogEntry l)
        {
            if (string.IsNullOrEmpty(_currentFilename)) return;
            WriteLine(_currentPath, _currentFilename, GetCsvEntry(l));
        }

        private static string GetFileTitle()
        {
            return "Created At,Log Type,Direction,Information,Size,OpCode,Sender,Request ID,"
                   + "Lag,Round Trip Time,kbit/s,Ping,Pong\r\n";
        }
        
        private static string GetSimpleLogEntryCsvEntry(ILogEntry l)
        {   
            return string.Format(
                $"{l.GetCreatedAt()},{l.GetLogEntryType()},{l.GetFullInfo()},{l.GetDirection()}" +
                ',' * 11 + "\r\n");
        }

        private static string GetPongPacketCsvEntry(ILogEntry l)
        {
            var p = (PongPacketLog) l;
            return string.Format(
                $"{p.GetCreatedAt()},{p.GetLogEntryType()},{p.GetFullInfo()},{p.GetDirection()},{p.GetFullInfo()}" +
                ',' * 10 + "\r\n");
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
                case LogEntryTypes.OnSessionNotReady: return GetSimpleLogEntryCsvEntry(l);
                case LogEntryTypes.OnSessionLeave: return GetSimpleLogEntryCsvEntry(l);
                case LogEntryTypes.OnPlayerConnect: return GetSimpleLogEntryCsvEntry(l);
                case LogEntryTypes.OnPlayerDisconnect: return GetSimpleLogEntryCsvEntry(l);
                
                // Packet Entries
                case LogEntryTypes.PingPacket: return string.Empty;
                case LogEntryTypes.PongPacket: return GetPongPacketCsvEntry(l);
                case LogEntryTypes.BlankPacket: return string.Empty;
                case LogEntryTypes.UnstructuredPacket: return string.Empty;
                
                // Quality of Service Test Result Entry
                case LogEntryTypes.QualityOfServiceTestResult: return string.Empty;
                default: return GetSimpleLogEntryCsvEntry(l);
            }
        }
        
        private static void WriteLine(string path, string filename, string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            if (string.IsNullOrEmpty(path)) return;
            if (string.IsNullOrEmpty(filename)) return;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!Directory.Exists(path)) return;
            
            var writer = new StreamWriter($"{path}{filename}", true);
            writer.WriteLine(line);
            writer.Close();
        }
    }
}