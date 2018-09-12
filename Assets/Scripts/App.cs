using System;
using Controllers;
using GameSparks.Core;
using Services;
using UnityEngine;
using Zenject;

public class App : IInitializable
{
    /**
     * Steve Callaghan (scalla@amazon.com)
     * Created: 2017/09 (September, 2017)
     * https://github.com/Patchthesock/GSRealTimeTestTool
     */

    public App(
        Settings settings,
        RtQosService rtQosService,
        EnvironmentHooks envHooks,
        GuiController guiController,
        SparkRtService sparkRtService,
        CsvWriterService csvWriterService)
    {
        _settings = settings;
        _envHooks = envHooks;
        _rtQosService = rtQosService;
        _guiController = guiController;
        _sparkRtService = sparkRtService;
        _csvWriterService = csvWriterService;
    }

    /**
     * <summary>Application Initialization</summary>
     */
    public void Initialize()
    {
        SetupEnvHooks();
        SetupGuiController();
        SetupSparkRtService();
        
        if (Application.isEditor && _settings.WriteLog) WriteLog();
        _guiController.Initialize();
    }

    private void SetupSparkRtService()
    {
        _sparkRtService.SubscribeToOnRtReady(_guiController.SetRealTimeActive);
        _sparkRtService.SubscribeToOnLogEntryReceived(_rtQosService.OnLogEntryReceived);
        _sparkRtService.SubscribeToOnLogEntryReceived(_guiController.OnLogEntryReceived);
        _rtQosService.SubscribeToPingTestResults(_guiController.OnLogEntryReceived);
    }

    private void SetupGuiController()
    {
        _guiController.SubscribeToOnSendPingPacket(_sparkRtService.SendPing);
        _guiController.SubscribeToOnStopSession(_sparkRtService.LeaveSession);
        _guiController.SubscribeToOnStartSession(_sparkRtService.ConnectSession);
        _guiController.SubscribeToOnSendBlankPacket(_sparkRtService.SendBlankPacket);
        _guiController.SubscribeToOnSendUnstructuredPacket(_sparkRtService.SendUnstructuredDataPacket);
        _guiController.SubscribeToOnStartPingTest((p, s) =>
        {
            _rtQosService.StartPingTest(p, s, _sparkRtService.SendPing);
        });
    }

    private void WriteLog()
    {
        _csvWriterService.CreateFile($"{Application.dataPath}/Log/", $"{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        _rtQosService.SubscribeToPingTestResults(_csvWriterService.WriteLogEntry);
        _sparkRtService.SubscribeToOnLogEntryReceived(_csvWriterService.WriteLogEntry);
    }
    
    private void SetupEnvHooks()
    {
        _envHooks.SubscribeToOnApplicationFocusChange(hasFocus =>
        {
            GS.Reset();
            Debug.Log($"App Focus: {hasFocus}");
        });
        _envHooks.SubscribeToOnApplicationPauseChange(pauseStatus =>
        {
            GS.Reset();
            Debug.Log($"App Pause: {pauseStatus}");
        });
    }

    private readonly Settings _settings;
    private readonly RtQosService _rtQosService;
    private readonly EnvironmentHooks _envHooks;
    private readonly GuiController _guiController;
    private readonly SparkRtService _sparkRtService;
    private readonly CsvWriterService _csvWriterService;

    [Serializable]
    public class Settings
    {
        public bool WriteLog = false;
    }
}