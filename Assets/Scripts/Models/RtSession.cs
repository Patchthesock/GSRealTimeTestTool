using System.Collections.Generic;
using GameSparks.Api.Messages;

namespace Models
{
    public class RtSession
    {
        public int PortId { get; private set; }
        public string HostUrl { get; private set; }
        public string MatchId { get; private set; }
        public string AcccessToken { get; private set; }
        public List<RtPlayer> PlayerList { get; private set; }

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
            public string DisplayName { get; private set; }
            public string Id { get; private set; }
            public int PeerId { get; private set; }

            public RtPlayer(string displayName, string id, int peerId)
            {
                Id = id;
                PeerId = peerId;
                DisplayName = displayName;
            }
        }
    }
}
