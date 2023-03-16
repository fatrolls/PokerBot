using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot
{
    public class ReadMemory
    {

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public static int Process_PID = -1;
        public static int ProcessBaseAddress = 0;
        public static void OpenProcess(string processName, string windowName)
        {
            while (true)
            {
                var processList = Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero && p.ProcessName == processName);
                foreach (var process in processList)
                {
                    var handle = process.MainWindowHandle;
                    const int nChars = 256;
                    var buff = new StringBuilder(nChars);

                    if (GetWindowText(handle, buff, nChars) <= 0)
                        break;

                    var name = buff.ToString();

                    if (name.Contains(windowName))
                    {
                        Process_PID = process.Id;
                        ProcessBaseAddress = (int)process.MainModule.BaseAddress;
                        break;
                    }
                }
                if (Process_PID != -1)
                {
                    Console.WriteLine("Testing memory reading start");
                    Console.WriteLine("--------------------------------");
                    ReadString(0x074A65E4, 20);
                    Console.WriteLine(ReadPointer(ProcessBaseAddress, 0x00B51980, 0x1C8, 0x3C, 0x344, 0x670, 0xF8, 0x1A8, 0x13C).ToString());
                    ReadUInt(0x07162408);
                    ReadUShort(0x07162408);
                    ReadShort(0x07162408);
                    ReadByte(0x07162408);
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("Testing memory reading end");
                    break;
                }
                Thread.Sleep(500);
            }
        }

        public static byte[] ReadMem(int address, int pSize)
        {
            if (Process_PID == -1)
            {
                Console.WriteLine("Cannot read memory, error");
                return null;
            }

            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, Process_PID);

            int bytesRead = 0;
            byte[] buffer = new byte[pSize];
            ReadProcessMemory((int)processHandle, address, buffer, pSize, ref bytesRead);
            return buffer;
        }

        public static string ReadString(int Address, int Size)
        {

            byte[] buffer = ReadMem(Address, Size);
            Console.WriteLine(Encoding.ASCII.GetString(buffer) + " (" + Size.ToString() + "bytes)");
            return Encoding.ASCII.GetString(buffer);
        }

        public static string ReadStringUnicode(int Address, int Size)
        {

            byte[] buffer = ReadMem(Address, Size);
            Console.WriteLine(Encoding.Unicode.GetString(buffer) + " (" + Size.ToString() + "bytes)");
            return Encoding.Unicode.GetString(buffer);
        }

        public static byte ReadByte(int address)
        {
            Console.WriteLine("Byte value = " + ReadMem(address, 1)[0]);
            return ReadMem(address, 1)[0];
        }

        public static short ReadShort(int address)
        {
            Console.WriteLine("Short value = " + BitConverter.ToInt16(ReadMem(address, 2), 0));
            return BitConverter.ToInt16(ReadMem(address, 2), 0);
        }
        public static ushort ReadUShort(int address)
        {
            Console.WriteLine("UShort value = " + BitConverter.ToUInt16(ReadMem(address, 2), 0));
            return BitConverter.ToUInt16(ReadMem(address, 2), 0);
        }

        public static uint ReadUInt(int address)
        {
            Console.WriteLine("UInteger value = " + BitConverter.ToUInt32(ReadMem(address, 4), 0));
            return BitConverter.ToUInt32(ReadMem(address, 4), 0);
        }

        public static uint ReadPointer(int Address, int pOffset)
        {
            return ReadUInt(Address + pOffset);
        }

        public static uint ReadPointer(int Address, int pOffset1, int pOffset2)
        {
            return ReadUInt((int)(ReadUInt(Address + pOffset1)) + pOffset2);
        }

        public static uint ReadPointer(int Address, int pOffset, int pOffset2, int pOffset3)
        {
            return ReadUInt((int)ReadUInt((int)ReadUInt(Address + pOffset) + pOffset2) + pOffset3);
        }

        public static uint ReadPointer(int Address, int pOffset, int pOffset2, int pOffset3, int pOffset4)
        {
            return ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt(Address + pOffset) + pOffset2) + pOffset3) + pOffset4);
        }

        public static uint ReadPointer(int Address, int pOffset, int pOffset2, int pOffset3, int pOffset4, int pOffset5)
        {
            return ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt(Address + pOffset) + pOffset2) + pOffset3) + pOffset4) + pOffset5);
        }

        public static uint ReadPointer(int Address, int pOffset, int pOffset2, int pOffset3, int pOffset4, int pOffset5, int pOffset6)
        {
            return ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt(Address + pOffset) + pOffset2) + pOffset3) + pOffset4) + pOffset5) + pOffset6);
        }
        public static uint ReadPointer(int Address, int pOffset, int pOffset2, int pOffset3, int pOffset4, int pOffset5, int pOffset6, int pOffset7)
        {
            return ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt(Address + pOffset) + pOffset2) + pOffset3) + pOffset4) + pOffset5) + pOffset6) + pOffset7);
        }

        public static uint ReadPointer(int Address, int pOffset, int pOffset2, int pOffset3, int pOffset4, int pOffset5, int pOffset6, int pOffset7, int pOffset8)
        {
            return ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt((int)ReadUInt(Address + pOffset) + pOffset2) + pOffset3) + pOffset4) + pOffset5) + pOffset6) + pOffset7) + pOffset8);
        }
    }
}
