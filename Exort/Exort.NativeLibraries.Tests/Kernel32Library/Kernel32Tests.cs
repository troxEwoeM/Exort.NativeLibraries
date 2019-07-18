using System;
using System.Diagnostics;
using Exort.NativeLibraries.Kernel32Library;
using Exort.NativeLibraries.Kernel32Library.Declaration;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;

namespace Exort.NativeLibraries.Tests.Kernel32Library
{
    [TestFixture]
    public class Kernel32Tests
    {
        [SetUp]
        public void SetUp()
        {
            var config = new LoggingConfiguration();
            var logConsole = new ColoredConsoleTarget("logConsole");
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logConsole);
            LogManager.Configuration = config;
        }

        [Test]
        public void Kernel32Test()
        {
            var kernel32 = new Kernel32();
            var loadResult = kernel32.Load();
            var unloadResult = kernel32.Unload();
            Assert.AreEqual(true, loadResult.Success);
            Assert.AreEqual(true, unloadResult.Success);
        }

        [Test]
        public void OpenAndCloseProcess()
        {
            var kernel32 = new Kernel32();
            var loadResult = kernel32.Load();
            var process = Process.Start("notepad");
            var openResult = kernel32.OpenProcess(ProcessAccessFlags.QueryInformation, true, process.Id);
            var closeResult = kernel32.CloseHandle(openResult);
            process.Close();
            var unloadResult = kernel32.Unload();

            Assert.AreEqual(true, loadResult.Success);
            Assert.AreEqual(true, unloadResult.Success);
            Assert.AreNotEqual(IntPtr.Zero, openResult);
            Assert.AreEqual(true, unloadResult.Success);
        }
    }
}