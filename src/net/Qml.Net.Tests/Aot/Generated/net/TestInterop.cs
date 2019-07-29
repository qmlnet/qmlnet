using Qml.Net.Aot;
// ReSharper disable once CheckNamespace
namespace TestInterop
{
	public class TestInterop
	{
		private static bool _didRegister;
		private static readonly object Lock = new object();
		public static void Register()
		{
			lock (Lock)
			{
				if (_didRegister) return;
				AotTypes.AddAotType(1, typeof(global::Qml.Net.Tests.Aot.AotMethodInvocationTests.AotMethodInvocation));
				AotTypes.AddAotType(2, typeof(global::System.Object));
				_didRegister = true;
			}
		}
	}
}
