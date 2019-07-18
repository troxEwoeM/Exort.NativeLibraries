using Exort.NativeLibraries.Base.Results;
using Exort.NativeLibraries.Properties;
using NLog;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Exort.NativeLibraries.Base
{
    /// <summary>
    /// Managed wrapper for native libraries
    /// </summary>
    public abstract class NativeLibrary
    {
        protected readonly string LibraryPath;
        protected IntPtr LibraryHandle = IntPtr.Zero;
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Managed wrapper for native libraries
        /// </summary>
        /// <param name="libraryPath">Name or path to library</param>
        protected NativeLibrary(string libraryPath)
        {
            this.LibraryPath = libraryPath;
        }

        /// <summary>
        /// Library loading into memory
        /// </summary>
        /// <returns>Success or not and error message</returns>
        public LoadLibraryResult Load()
        {
            if (string.IsNullOrEmpty(this.LibraryPath))
            {
                Logger.Fatal(string.Format(Resources.LibraryNullOrEmptyPathMessage, this.LibraryPath), this.LibraryPath);
                return new LoadLibraryResult(false, string.Format(Resources.LibraryNullOrEmptyPathMessage, this.LibraryPath));
            }

            Logger.Trace(string.Format(Resources.LibraryLoadingStartedMessage, this.LibraryPath), this.LibraryPath);
            this.LibraryHandle = Kernel32.LoadLibrary(this.LibraryPath);
            if (this.LibraryHandle != IntPtr.Zero)
            {
                Logger.Info(string.Format(Resources.LibraryLoadedSuccessfullyMessage, this.LibraryPath), this.LibraryPath);
                Logger.Trace(string.Format(Resources.LibraryMethodsLoadingStartedMessage, this.LibraryPath), this.LibraryPath);
                var result = this.LoadMethods();

                return result.Success ? new LoadLibraryResult(true) : result;
            }

            var errorCode = Marshal.GetLastWin32Error();
            var exception = new Win32Exception(errorCode);

            Logger.Fatal(string.Format(Resources.LibraryLoadFailedMessage, this.LibraryPath, exception.Message), this.LibraryPath,
                exception.Message);

            return new LoadLibraryResult(false, string.Format(Resources.LibraryLoadFailedMessage, exception.Message, ""));
        }

        public LoadLibraryResult Unload()
        {
            if (this.LibraryHandle == IntPtr.Zero)
            {
                var msg = string.Format(Resources.LibraryUnloadNotFoundMessage, this.LibraryPath);
                Logger.Fatal(msg, this.LibraryPath);
                return new LoadLibraryResult(false, msg);
            }

            var result = Kernel32.FreeLibrary(this.LibraryHandle);

            if (!result)
            {
                var errorCode = Marshal.GetLastWin32Error();
                var exception = new Win32Exception(errorCode);
                var msg = string.Format(Resources.LibraryUnloadFailedMessage, this.LibraryPath, exception.Message);
                Logger.Fatal(msg, this.LibraryPath, exception.Message);
                return new LoadLibraryResult(false, msg);
            }
            Logger.Info(string.Format(Resources.LibraryUnloadSuccessfullyMessage, this.LibraryPath));
            return new LoadLibraryResult(true);
        }

        protected abstract LoadLibraryResult LoadMethods();

        protected LoadMethodResult<T> LoadMethod<T>(string functionName) where T : class
        {
            if (this.LibraryHandle == IntPtr.Zero)
            {
                var msg = string.Format(Resources.MethodLoadAnUnloadedLibraryMessage, this.LibraryPath, functionName);
                Logger.Fatal(msg);
                return new LoadMethodResult<T>(false, default(T), msg);
            }
            var functionHandle = Kernel32.GetProcAddress(this.LibraryHandle, functionName);
            if (functionHandle == IntPtr.Zero)
            {
                var msg = string.Format(Resources.MethodNotFoundMessage, this.LibraryPath, functionName);
                Logger.Fatal(msg);
                return new LoadMethodResult<T>(false, default(T), msg);
            }
            Logger.Trace(string.Format(Resources.MethodLoadingStartedMessage, this.LibraryPath, functionName));
            var method = Marshal.GetDelegateForFunctionPointer(functionHandle, typeof(T)) as T;
            if (method != null)
            {
                Logger.Trace(string.Format(Resources.MethodLoadedSuccessfulltyMessage, this.LibraryPath, functionName));
                return new LoadMethodResult<T>(true, method);
            }
            else
            {
                var msg = string.Format(Resources.MethodBadDelegateTypeMessage, this.LibraryPath, functionName);
                Logger.Fatal(msg);
                return new LoadMethodResult<T>(false, default(T), msg);
            }
        }
    }
}