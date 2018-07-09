using System.Linq;
using System.Text;
using GameSparks.Api.Messages;

namespace Models
{
    public class RtSession
    {
        public readonly int PortId;
        public readonly string HostUrl;
        public readonly string MatchId;
        public readonly string AccessToken;
        private readonly string _toString;

        public RtSession(MatchFoundMessage message)
        {
            HostUrl = message.Host;
            MatchId = message.MatchId;
            PortId = message.Port ?? 0;
            AccessToken = message.AccessToken;
            _toString = CreateToString(message);
        }

        public override string ToString()
        {
            return _toString;
        }

        private static string CreateToString(MatchFoundMessage m)
        {
            var s = new StringBuilder()
                .AppendLine($"Port: {m.Port}")
                .AppendLine($"Host URL: {m.Host}")
                .AppendLine("---   ---   ---")
                .AppendLine($"MatchId: {m.MatchId}")
                .AppendLine($"Players: {m.Participants.Count()}");
            foreach (var p in m.Participants) s.AppendLine(CreatePlayer(p).ToString());
            s.AppendLine($"Access Token: {m.AccessToken}");
            return s.ToString();
        }
        
        private static RtPlayer CreatePlayer(MatchFoundMessage._Participant p)
        {
            return new RtPlayer(p.DisplayName, p.Id, p.PeerId ?? 0);
        }
        
        private class RtPlayer
        {
            public RtPlayer(string displayName, string id, int peerId)
            {
                _id = id;
                _peerId = peerId;
                _displayName = displayName;
            }

            public override string ToString()
            {
                return $"{_displayName} - PeerId: {_peerId} - {_id}";
            }
            
            private readonly string _id;
            private readonly int _peerId;
            private readonly string _displayName;
        }
    }
}
