using System;
using System.Runtime.InteropServices;

namespace Exort.NativeLibraries.Kernel32Library.Declaration
{
    public delegate bool ReadProcessMemoryDelegate(IntPtr handle, IntPtr baseAddress, [Out] byte[] buffer, int size, out IntPtr numberOfBytesRead);
}