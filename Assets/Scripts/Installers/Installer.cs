using System;
using Controllers;
using Gui;
using Services;
using Zenject;

namespace Installers
{
    public class Installer : MonoInstaller
    {
        public Settings GameSettings;
        
        public override void InstallBindings()
        {
            InstallServices(GameSettings.AsyncProc, Container, GameSettings.GameSparksRtUnity, GameSettings.RtServiceSettings);
            InstallGui(Container, GameSettings.GuiSettings);
            InstallApp(Container, GameSettings.AppSettings);
        }

        private static void InstallServices(
            AsyncProcessor a,
            DiContainer c,
            GameSparksRTUnity gs,
            SparkRtService.Settings s)
        {
            c.Bind<AsyncProcessor>().FromInstance(a);
            c.Bind<RtQosService>().AsSingle();
            c.Bind<PrefabBuilder>().AsSingle();
            c.Bind<SparkRtService>().AsSingle();
            c.Bind<CsvWriterService>().AsSingle();
            c.Bind<SparkRtService.Settings>().FromInstance(s).AsSingle();
            c.Bind<GameSparksRTUnity>().FromInstance(gs).AsSingle();
        }

        private static void InstallGui(DiContainer c, Settings.Gui g)
        {
            c.Bind<AuthGui>().FromInstance(g.AuthGui).AsSingle();
            c.Bind<SessionGui>().FromInstance(g.SessionGui).AsSingle();
            c.Bind<ConnectionGui>().FromInstance(g.ConnectionGui).AsSingle();
            c.Bind<GuiController>().AsSingle();
        }

        private static void InstallApp(DiContainer c, App.Settings s)
        {
            c.Bind<App.Settings>().FromInstance(s).AsSingle();
            c.Bind<IInitializable>().To<App>().AsSingle();
            c.Bind<App>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public Gui GuiSettings;
            public AsyncProcessor AsyncProc;
            public App.Settings AppSettings;
            public GameSparksRTUnity GameSparksRtUnity;
            public SparkRtService.Settings RtServiceSettings;

            [Serializable]
            public class Gui
            {
                public AuthGui AuthGui;
                public SessionGui SessionGui;
                public ConnectionGui ConnectionGui;
            }
        }
    }
}