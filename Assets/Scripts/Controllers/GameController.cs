using System;
using Models;
using Services;
using Zenject;

namespace Controllers
{
    public class GameController : IInitializable
    {
        public GameController(
            SparkService sparkService,
            GuiController guiController,
            SparkRtService sparkRtService,
            PacketController packetController)
        {
            _sparkService = sparkService;
            _guiController = guiController;
            _sparkRtService = sparkRtService;
            _packetController = packetController;
        }

        public void Initialize()
        {
            SetUpListeners();
            _sparkService.Initialize();
            _guiController.Initialize();
        }

        private void SetUpListeners()
        {
            SetUpRtListeners();
            SetUpGuiListeners();
            SetUpPacketListeners();
            SetUpMatchFoundListeners();
            SetUpGsAvailableListeners();
            SetUpMatchNotFoundListeners();
        }
        
        private void SetUpGsAvailableListeners()
        {
            _sparkService.SubscribeOnGsAvailable(_guiController.OnGsAvailable);
        }
        
        private void SetUpMatchFoundListeners()
        {
            _sparkService.SubscribeOnMatchFound(_guiController.OnMatchFound);
            _sparkService.SubscribeOnMatchFound(m => { _rtSession = ModelFactory.CreateRtSession(m); });
        }
        
        private void SetUpMatchNotFoundListeners()
        {
            _sparkService.SubscribeOnMatchNotFound(_guiController.OnMatchNotFound);   
        }
        
        private void SetUpRtListeners()
        {
            _sparkRtService.SubscribeToOnRtReady(_guiController.OnRtReady);
            _sparkRtService.SubscribeToOnPlayerConnected(_guiController.OnPlayerConnected);
            _sparkRtService.SubscribeToOnPlayerDisconnected(_guiController.OnPlayerDisconnected);
        }

        private void SetUpGuiListeners()
        {
            _guiController.SubscribeToOnStopSession(_sparkRtService.LeaveSession);
            _guiController.SubscribeToOnSendBlankPacketA(_packetController.SendBlankPacket);
            _guiController.SubscribeToOnSendTimestampPacket(_packetController.SendTimestampPingPacket);
            _guiController.SubscribeToOnStartSession(() => { _sparkRtService.ConnectSession(_rtSession, _packetController.OnPacketReceived); });
        }

        private void SetUpPacketListeners()
        {
            _packetController.SubscribeToOnTimestampPongReceived(_guiController.OnLatencyReceived);
            _packetController.SubscribeToOnBlankPacketReceived(_guiController.OnBlankPacketReceived);
            _packetController.SubscribeToOnTimestampPingReceived((pingTime) => { _packetController.SendTimestampPongpacket(pingTime); });
        }

        private RtSession _rtSession;
        private readonly SparkService _sparkService;
        private readonly GuiController _guiController;
        private readonly SparkRtService _sparkRtService;
        private readonly PacketController _packetController;
    }
}