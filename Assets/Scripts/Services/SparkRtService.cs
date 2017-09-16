using System;
using System.Collections.Generic;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using Models;

namespace Services
{
    public class SparkRtService
    {
        public SparkRtService(GameSparksRTUnity gameSparksRtUnity)
        {
            _gameSparksRtUnity = gameSparksRtUnity;
        }
        
        public void ConnectSession(RtSession info, Action<RTPacket> onPacketReceived)
        {
            // In order to create a new RT game we need a 'FindMatchResponse' //
            // This would usually come from the server directly after a sucessful FindMatchRequest //
            // However, in our case, we want the game to be created only when the first player decides using a button //
            // therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse //
            // is passed in. In normal operation this mock-response may not be needed //
            var mockedResponse = new GSRequestData()  // construct a dataset from the game-details
            .AddNumber("port", (double)info.PortId)
            .AddString("host", info.HostUrl)
            .AddString("accessToken", info.AcccessToken);
            // create a match-response from that data and pass it into the game-config
            // So in the game-config method we pass in the response which gives the instance its connection settings //
            // In this example i use a lambda expression to pass in actions for 
            // OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions //
            // These methods are self-explanitory, but the important one is the OnPacket Method //
            // this gets called when a packet is received //
            _gameSparksRtUnity.Configure(new FindMatchResponse(mockedResponse), OnPlayerConnected, OnPlayerDisconnected, OnRtReady, onPacketReceived);
            _gameSparksRtUnity.Connect(); // when the config is set, connect the game
        }

        public void SendPacket(int opCode, GameSparksRT.DeliveryIntent intent, RTData data)
        {
            _gameSparksRtUnity.SendData(opCode, intent, data);
        }

        public void LeaveSession()
        {
            _gameSparksRtUnity.Disconnect();
        }

        public void SubscribeToOnRtReady(Action<bool> onRtReady)
        {
            if (_onRtReady.Contains(onRtReady)) return;
            _onRtReady.Add(onRtReady);
        }
        
        public void SubscribeToOnPlayerConnected(Action<int> onPlayerConnected)
        {
            if (_onPlayerConnectedListeners.Contains(onPlayerConnected)) return;
            _onPlayerConnectedListeners.Add(onPlayerConnected);
        }

        public void SubscribeToOnPlayerDisconnected(Action<int> onPlayerDisconnected)
        {
            if (_onPlayerDisconnectedListeners.Contains(onPlayerDisconnected)) return;
            _onPlayerDisconnectedListeners.Add(onPlayerDisconnected);
        }

        private void OnRtReady(bool state)
        {
            foreach (var l in _onRtReady) l(state);
        }

        private void OnPlayerConnected(int peerId)
        {
            foreach (var l in _onPlayerConnectedListeners) l(peerId);
        }

        private void OnPlayerDisconnected(int peerId)
        {
            foreach (var l in _onPlayerDisconnectedListeners) l(peerId);
        }

        private readonly GameSparksRTUnity _gameSparksRtUnity;
        private readonly List<Action<bool>> _onRtReady = new List<Action<bool>>();
        private readonly List<Action<int>> _onPlayerConnectedListeners = new List<Action<int>>();
        private readonly List<Action<int>> _onPlayerDisconnectedListeners = new List<Action<int>>();
    }
}