using System;
using System.IO;
using Qml.Net.Runtimes;

namespace Qml.Net.Tests
{
    public class BaseRuntimeManagerTests : BaseTests
    {
        // ReSharper disable MemberCanBePrivate.Global
        protected readonly string _runtimeUserDirectory;
        protected readonly string _runtimeExecutableDirectory;
        protected readonly string _runtimeCurrentDirectory;
        // ReSharper restore MemberCanBePrivate.Global
        private readonly string _tmpDirectory;
        private readonly Func<string> _oldRuntimeUserDirectory;
        private readonly Func<string> _oldRuntimeExecutableDirectory;
        private readonly Func<string> _oldRuntimeCurrentDirectory;

        public BaseRuntimeManagerTests()
        {
            _tmpDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", ""));
            Directory.CreateDirectory(_tmpDirectory);

            _runtimeUserDirectory = Path.Combine(_tmpDirectory, "user");
            _runtimeExecutableDirectory = Path.Combine(_tmpDirectory, "executable");
            _runtimeCurrentDirectory = Path.Combine(_tmpDirectory, "current");

            Directory.CreateDirectory(_runtimeUserDirectory);
            Directory.CreateDirectory(_runtimeExecutableDirectory);
            Directory.CreateDirectory(_runtimeCurrentDirectory);

            _oldRuntimeUserDirectory = RuntimeManager.GetRuntimeUserDirectory;
            _oldRuntimeExecutableDirectory = RuntimeManager.GetRuntimeExecutableDirectory;
            _oldRuntimeCurrentDirectory = RuntimeManager.GetRuntimeCurrentDirectory;

            RuntimeManager.GetRuntimeUserDirectory = () => _runtimeUserDirectory;
            RuntimeManager.GetRuntimeExecutableDirectory = () => _runtimeExecutableDirectory;
            RuntimeManager.GetRuntimeCurrentDirectory = () => _runtimeCurrentDirectory;
        }

        public override void Dispose()
        {
            Directory.Delete(_tmpDirectory, true);

            RuntimeManager.GetRuntimeUserDirectory = _oldRuntimeUserDirectory;
            RuntimeManager.GetRuntimeExecutableDirectory = _oldRuntimeExecutableDirectory;
            RuntimeManager.GetRuntimeCurrentDirectory = _oldRuntimeCurrentDirectory;

            base.Dispose();
        }
    }
}