using System;
using System.Net;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.IO;

namespace ZplWeb2Usb
{
    public static class RawPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint="OpenPrinterA", SetLastError=true, CharSet=CharSet.Ansi, ExactSpelling=true, CallingConvention=CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint="ClosePrinter", SetLastError=true, ExactSpelling=true, CallingConvention=CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint="StartDocPrinterA", SetLastError=true, CharSet=CharSet.Ansi, ExactSpelling=true, CallingConvention=CallingConvention.StdCall)]
        public static extern bool StartDocPrinter( IntPtr hPrinter, Int32 level,  [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint="EndDocPrinter", SetLastError=true, ExactSpelling=true, CallingConvention=CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint="StartPagePrinter", SetLastError=true, ExactSpelling=true, CallingConvention=CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint="EndPagePrinter", SetLastError=true, ExactSpelling=true, CallingConvention=CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint="WritePrinter", SetLastError=true, ExactSpelling=true, CallingConvention=CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, byte[] pBytes, Int32 dwCount, out Int32 dwWritten );

        public static bool SendStringToPrinter(string szPrinterName, byte[] bytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;

            di.pDocName = "Zebra Label";
            di.pDataType = "RAW";
            Console.WriteLine(di.pDocName);
            Console.WriteLine(szPrinterName.Normalize());

            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        bSuccess = WritePrinter(hPrinter, bytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
                //throw new Exception(dwError);
                Console.WriteLine(dwError);
            }
            return bSuccess;
        }
    }
}