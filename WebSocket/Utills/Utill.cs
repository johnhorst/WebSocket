using System;
using System.Runtime.InteropServices;

namespace WebSocket.Utility
{
    public static class Convertor
    {
        public static int SizeOf<T>() where T : struct
        {
            return Marshal.SizeOf(typeof(T));
        }

        public static byte[] ToBinary(object obj)
        {
            if (obj == null)
                return null;
            byte[] buffer = new byte[Marshal.SizeOf(obj)];
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, buffer, 0, Marshal.SizeOf(obj));
            return buffer;
        }

        public static T FromBinary<T>(byte[] obj) where T : struct
        {
            if (obj == null)
                return default(T);
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(obj, 0, ptr, size);
            T temp = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return temp;
        }
      
    }
}
