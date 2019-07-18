using Exort.NativeLibraries.Base;
using Exort.NativeLibraries.Base.Results;
using Exort.NativeLibraries.Kernel32Library.Declaration;
using System;
using System.Runtime.InteropServices;

namespace Exort.NativeLibraries.Kernel32Library
{
    public class Kernel32 : NativeLibrary
    {
        private CloseHandleDelegate _closeHandle;
        private OpenProcessDelegate _openProcess;
        private ReadProcessMemoryDelegate _readProcessMemory;
        private WriteProcessMemoryDelegate _writeProcessMemory;

        public Kernel32() : base("kernel32.dll")
        {
        }

        public bool CloseHandle(IntPtr handle)
        {
            if (this._closeHandle == null)
                throw new Exception("Library not loaded");
            return this._closeHandle.Invoke(handle);
        }

        public IntPtr OpenProcess(ProcessAccessFlags processAccess, bool inheritHandle, int processId)
        {
            if (this._openProcess == null)
                throw new Exception("Library not loaded");
            return this._openProcess.Invoke(processAccess, inheritHandle, processId);
        }

        public bool ReadProcessMemory(IntPtr handle, IntPtr baseAddress, [Out] byte[] buffer, int size, out IntPtr numberOfBytesRead)
        {
            if (this._openProcess == null)
                throw new Exception("Library not loaded");
            return this._readProcessMemory(handle, baseAddress, buffer, size, out numberOfBytesRead);
        }

        public bool WriteProcessMemory(IntPtr handle, IntPtr baseAddress, byte[] buffer, int size, out IntPtr numberOfBytesWritten)
        {
            if (this._openProcess == null)
                throw new Exception("Library not loaded");
            return this._writeProcessMemory(handle, baseAddress, buffer, size, out numberOfBytesWritten);
        }

        protected override LoadLibraryResult LoadMethods()
        {
            var closeHandleResult = this.LoadMethod<CloseHandleDelegate>("CloseHandle");
            if (closeHandleResult.Success)
                this._closeHandle = closeHandleResult.Delegate;
            else
                return new LoadLibraryResult(false, closeHandleResult.Error);

            var openProcessResult = this.LoadMethod<OpenProcessDelegate>("OpenProcess");
            if (openProcessResult.Success)
                this._openProcess = openProcessResult.Delegate;
            else
                return new LoadLibraryResult(false, openProcessResult.Error);

            var readProcessMemoryResult = this.LoadMethod<ReadProcessMemoryDelegate>("ReadProcessMemory");
            if (readProcessMemoryResult.Success)
                this._readProcessMemory = readProcessMemoryResult.Delegate;
            else
                return new LoadLibraryResult(false, readProcessMemoryResult.Error);

            var writeProcessMemoryResult = this.LoadMethod<WriteProcessMemoryDelegate>("WriteProcessMemory");
            if (writeProcessMemoryResult.Success)
                this._writeProcessMemory = writeProcessMemoryResult.Delegate;
            else
                return new LoadLibraryResult(false, writeProcessMemoryResult.Error);

            return new LoadLibraryResult(true);
        }
    }
}