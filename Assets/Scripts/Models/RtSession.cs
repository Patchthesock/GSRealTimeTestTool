using System.Collections.Generic;
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
        public readonly List<RtPlayer> PlayerList;

        public RtSession(MatchFoundMessage message)
        {
            HostUrl = message.Host;
            MatchId = message.MatchId;
            PlayerList = new List<RtPlayer>();
            AccessToken = message.AccessToken;
            if (message.Port != null) PortId = (int) message.Port;
            foreach (var p in message.Participants)
                if (p.PeerId != null) PlayerList.Add(new RtPlayer(p.DisplayName, p.Id, (int) p.PeerId));
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendLine("Host URL: " + HostUrl);
            s.AppendLine("Port: " + PortId);
            s.AppendLine("MatchId: " + MatchId);
            s.AppendLine("Opponents: " + PlayerList.Count);
            foreach (var p in PlayerList) s.AppendLine(p.ToString());
            s.AppendLine("Access Token: " + AccessToken);
            return s.ToString();
        }

        public class RtPlayer
        {
            public readonly string Id;
            public readonly int PeerId;
            public readonly string DisplayName;
            
            public RtPlayer(string displayName, string id, int peerId)
            {
                Id = id;
                PeerId = peerId;
                DisplayName = displayName;
            }

            public override string ToString()
            {
                return DisplayName + " - PeerId: " + PeerId;
            }
        }
    }
}
