using System;
using System.Collections.Generic;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using Models;
using UnityEngine;

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
             * In order to create a new RtSession we need a 'FindMatchResponse'
             * In our case, we wanted to capture these details and have them passed in
             * this offers us greater flexibility.
            **/
            
            _gameSparksRtUnity.Configure(
                new FindMatchResponse(new GSRequestData()  // Construct a FindMatchResponse 
                .AddNumber("port", (double)s.PortId)       // that we can then us to configure
                .AddString("host", s.HostUrl)              // a Real Time Session from
                .AddString("accessToken", s.AcccessToken)),

                (peerId) => // OnPlayerConnected Callback
                {
                    Debug.Log("Player " + peerId + " Connected");
                    foreach (var l in _onPlayerConnectedListeners) l(peerId);
                },
                (peerId) => // OnPlayerDisconnected Callback
                {
                    Debug.Log("Player " + peerId + " Disconnected");
                    foreach (var l in _onPlayerDisconnectedListeners) l(peerId);
                },
                (state) => // OnRtReady Callback
                {
                    foreach (var l in _onRtReady) l(state);
                },
                (packet) => // OnPacketReceived Callback
                {
                    foreach (var l in _onPacketReceivedListeners) l(packet);
                });
            _gameSparksRtUnity.Connect(); // Connect
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

        private readonly GameSparksRTUnity _gameSparksRtUnity;
        private readonly List<Action<bool>> _onRtReady = new List<Action<bool>>();
        private readonly List<Action<int>> _onPlayerConnectedListeners = new List<Action<int>>();
        private readonly List<Action<int>> _onPlayerDisconnectedListeners = new List<Action<int>>();
        private readonly List<Action<RTPacket>> _onPacketReceivedListeners = new List<Action<RTPacket>>();
    }
}