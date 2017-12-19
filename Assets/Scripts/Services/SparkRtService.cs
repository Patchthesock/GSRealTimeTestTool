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
        
        /**
         * <summary>Given RtSession details will establish the Real Time Session connection</summary>
         * <param name="s">RtSession details of the Real Time session to connect to</param>
         **/
        public void ConnectSession(RtSession s)
        {
            /**
             * In order to create a new RT game we need a 'FindMatchResponse'
             * This would usually come from the server directly after a sucessful MatchRequest
             * However, in our case, we want the game to be created only when the first player decides using a button
             * therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse
             * is passed in. In normal operation this mock-response may not be needed
             **/
            var mockedResponse = new FindMatchResponse(new GSRequestData()  // construct a dataset from the game-details
            .AddNumber("port", (double)s.PortId)
            .AddString("host", s.HostUrl)
            .AddString("accessToken", s.AcccessToken));
            /**
             * Create a match-response from that data and pass it into the game-config
             * So in the game-config method we pass in the response which gives the instance its connection settings
             * In this example i use a lambda expression to pass in actions for 
             * OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions
             * These methods are self-explanitory, but the important one is the OnPacket Method
             * this gets called when a packet is received
            **/
            _gameSparksRtUnity.Configure(mockedResponse, OnPlayerConnected, OnPlayerDisconnected, OnRtReady, OnPacketReceived);
            _gameSparksRtUnity.Connect(); // when the config is set, connect the game
        }

        /**
         * <summary>Send Packet will send a packet to the Real Time Session</summary>
         * <param name="opCode">The OpCode of the packet being sent</param>
         * <param name="intent">The Protocol of the packet (TCP/UDP) being sent</param>
         * <param name="data">The RTData of the packet being sent</param>
         **/
        public void SendPacket(int opCode, GameSparksRT.DeliveryIntent intent, RTData data)
        {
            _gameSparksRtUnity.SendData(opCode, intent, data);
        }

        /**
         * <summary>Leave the Real Time Session</summary>
         **/
        public void LeaveSession()
        {
            _gameSparksRtUnity.Disconnect();
        }

        /**
         * <summary>Subscribe to on Real Time Session ready</summary>
         * <param name="onRtReady">Delegate Action with a bool state param</param>
         **/
        public void SubscribeToOnRtReady(Action<bool> onRtReady)
        {
            if (_onRtReady.Contains(onRtReady)) return;
            _onRtReady.Add(onRtReady);
        }

        /**
         * <summary>Subscribe to on Packet Received</summary>
         * <param name="onPacketReceived">Delegate Action with an RTPacket param</param>
         **/
        public void SubscribeToOnPacketReceived(Action<RTPacket> onPacketReceived)
        {
            if (_onPacketReceivedListeners.Contains(onPacketReceived)) return;
            _onPacketReceivedListeners.Add(onPacketReceived);
        }
        
        /**
         * <summary>Subscribe to on Player Connected</summary>
         * <param name="onPlayerConnected">Delegate Action with an int player peerId param</param>
         **/
        public void SubscribeToOnPlayerConnected(Action<int> onPlayerConnected)
        {
            if (_onPlayerConnectedListeners.Contains(onPlayerConnected)) return;
            _onPlayerConnectedListeners.Add(onPlayerConnected);
        }

        /**
         * <summary>Subscribe to on Player Disconnected</summary>
         * <param name="onPlayerDisconnected">Delegate Aciton with an int player peerId param</param>
         **/
        public void SubscribeToOnPlayerDisconnected(Action<int> onPlayerDisconnected)
        {
            if (_onPlayerDisconnectedListeners.Contains(onPlayerDisconnected)) return;
            _onPlayerDisconnectedListeners.Add(onPlayerDisconnected);
        }

        private void OnRtReady(bool state)
        {
            foreach (var l in _onRtReady) l(state);
        }
        
        private void OnPacketReceived(RTPacket p)
        {
            foreach (var l in _onPacketReceivedListeners) l(p);
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
        private readonly List<Action<RTPacket>> _onPacketReceivedListeners = new List<Action<RTPacket>>();
    }
}