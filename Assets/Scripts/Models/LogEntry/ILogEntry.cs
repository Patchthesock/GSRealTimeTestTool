using System;

namespace Models.LogEntry
{
    public interface ILogEntry
    {
        string GetTitle();
        string GetFullInfo();
        DateTime GetCreatedAt();
        Directions GetDirection();
        LogEntryTypes GetLogEntryType();
    }

    public enum LogEntryTypes
    {
        // Match Messages
        MatchFound,
        MatchNotFound,
        MatchMakingRequest,
        
        // Real Time Session Messages
        OnSessionJoin,
        OnSessionReady,
        OnSessionLeave,
        OnPlayerConnect,
        OnPlayerDisconnect,
        
        // Packets
        PingPacket,
        PongPacket,
        BlankPacket,
        UnstructuredPacket,
        
        // Quality Of Service
        QualityOfServiceTestResult
    }
    
    public enum Directions
    {
        Inbound,
        Outbound
    }
}