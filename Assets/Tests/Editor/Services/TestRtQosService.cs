using NUnit.Framework;
using Services;
using Zenject;

namespace Tests.Editor.Services
{
	[TestFixture]
	public class TestRtQosService : ZenjectUnitTestFixture
	{
		[SetUp]
		public void CommonInstall()
		{
			//Container.Bind<AsyncProcessor>().AsSingle();
			//Container.Bind<RtQosService>().AsSingle();
		}
		
		[Test]
		public void CanStartPingTestTest()
		{
			//var s = Container.Resolve<RtQosService>();
		}


	}
}
