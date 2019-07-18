using System;

namespace Exort.NativeLibraries.Kernel32Library.Declaration
{
    public delegate bool WriteProcessMemoryDelegate(IntPtr handle, IntPtr baseAddress, byte[] buffer, int size, out IntPtr numberOfBytesWritten);
}