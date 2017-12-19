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
            InstallGsRt(Container, GameSettings.GameSparksRtUnity);
            InstallServices(Container);
            InstallGui(Container, GameSettings.GuiSettings);
            InstallPacketController(Container, GameSettings.PacketControllerSettings);
            InstallApp(Container);
        }

        private static void InstallGsRt(DiContainer container, GameSparksRTUnity gs)
        {
            container.Bind<GameSparksRTUnity>().FromInstance(gs).AsSingle();
        }

        private static void InstallServices(DiContainer container)
        {
            container.Bind<SparkService>().AsSingle();
            container.Bind<SparkRtService>().AsSingle();
        }

        private static void InstallGui(DiContainer container, Settings.Gui gui)
        {
            container.Bind<AuthGui>().FromInstance(gui.AuthGui);
            container.Bind<PacketGui>().FromInstance(gui.PacketGui);
            container.Bind<UserInfoGui>().FromInstance(gui.UserInfoGui);
            container.Bind<RtSessionGui>().FromInstance(gui.RtSessionGui);
            container.Bind<MatchDetailsGui>().FromInstance(gui.MatchDetailGui);
            container.Bind<GuiController>().AsSingle();
        }

        private static void InstallPacketController(DiContainer container, PacketController.Settings settings)
        {
            container.Bind<PacketController.Settings>().FromInstance(settings).AsSingle();
            container.Bind<PacketController>().AsSingle();
        }

        private static void InstallApp(DiContainer container)
        {
            container.Bind<IInitializable>().To<App>().AsSingle();
            container.Bind<App>().AsSingle();
        }

        [Serializable]
        public class Settings
        {
            public Gui GuiSettings;
            public GameSparksRTUnity GameSparksRtUnity;
            public PacketController.Settings PacketControllerSettings;

            [Serializable]
            public class Gui
            {
                public AuthGui AuthGui;
                public PacketGui PacketGui;
                public UserInfoGui UserInfoGui;
                public RtSessionGui RtSessionGui;
                public MatchDetailsGui MatchDetailGui;
            }
        }
    }
}