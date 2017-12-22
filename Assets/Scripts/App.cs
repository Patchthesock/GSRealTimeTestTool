using Controllers;
using Services;
using Zenject;

public class App : IInitializable
{
    /**
     * Steve Callaghan (stephen.callaghan@gamesparks.com)
     * https://github.com/Patchthesock/GSRealTimeTestTool
     * Created: 2017/09 (September, 2017)
     *
     * Features to add
     * * Write Log to file
     * * Rapid packet test
     * * Throughput Limiter
     * * Check if already authenticated
     **/
    
    public App(
        GuiController guiController,
        SparkRtService sparkRtService)
    {
        _guiController = guiController;
        _sparkRtService = sparkRtService;
    }

    /**
     * <summary>Application Initialization</summary>
     **/
    public void Initialize()
    {
        _sparkRtService.SubscribeToOnRtReady(_guiController.SetRealTimeActive);
        _guiController.SubscribeToOnStopSession(_sparkRtService.LeaveSession);
        _guiController.SubscribeToOnSendBlankPacket(_sparkRtService.SendBlankPacket);
        _guiController.SubscribeToOnStartSession(r => { _sparkRtService.ConnectSession(r); });
        _guiController.SubscribeToOnSendTimestampPacket(_sparkRtService.SendTimestampPingPacket);
        _sparkRtService.SubscribeToOnLogEntryReceived(_guiController.OnLogEntryReceived);
        
        _guiController.Initialize();
    }

    private readonly GuiController _guiController;
    private readonly SparkRtService _sparkRtService;
}