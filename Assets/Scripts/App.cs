using System;
using Controllers;
using Services;
using UnityEngine;
using Zenject;

public class App : IInitializable
{
    /**
     * Steve Callaghan (stephen.callaghan@gamesparks.com)
     * https://github.com/Patchthesock/GSRealTimeTestTool
     * Created: 2017/09 (September, 2017)
     *
     * Features to add
     * * Rapid packet test
     * * Throughput Limiter
     **/
    
    public App(
        Settings settings,
        GuiController guiController,
        SparkRtService sparkRtService,
        CsvWriterService csvWriterService)
    {
        _settings = settings;
        _guiController = guiController;
        _sparkRtService = sparkRtService;
        _csvWriterService = csvWriterService;
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
        if (Application.isEditor && _settings.WriteLogFile) WriteLog();
        _guiController.Initialize();
    }

    private void WriteLog()
    {
        _csvWriterService.CreateFile();
        _sparkRtService.SubscribeToOnLogEntryReceived(_csvWriterService.WriteLogEntry);
    }

    private readonly Settings _settings;
    private readonly GuiController _guiController;
    private readonly SparkRtService _sparkRtService;
    private readonly CsvWriterService _csvWriterService;
    
    [Serializable]
    public class Settings
    {
        public bool WriteLogFile = false;
    }
}