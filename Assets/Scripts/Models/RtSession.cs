using System.Collections.Generic;
using GameSparks.Api.Messages;

namespace Models
{
    public class RtSession
    {
        public readonly int PortId;
        public readonly string HostUrl;
        public readonly string MatchId;
        public readonly string AcccessToken;
        public readonly List<RtPlayer> PlayerList;

        public RtSession(MatchFoundMessage message)
        {
            HostUrl = message.Host;
            MatchId = message.MatchId;
            PlayerList = new List<RtPlayer>();
            AcccessToken = message.AccessToken;
            if (message.Port != null) PortId = (int) message.Port;
            foreach (var p in message.Participants) if (p.PeerId != null) PlayerList.Add(new RtPlayer(p.DisplayName, p.Id, (int) p.PeerId));
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
        }
    }
}
