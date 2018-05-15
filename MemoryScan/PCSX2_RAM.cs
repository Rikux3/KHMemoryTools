// Source: mostly Teamod
using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace KHMemory
{
    public class PCSX2_RAM
    {
        private const int PROCESS_ALL_ACCESS = 0x001F0FFF;
        private static Process PCSX2;
        private static IntPtr PCSX2_hProc;
        public static IntPtr BaseAddr = IntPtr.Zero;

        static bool IsEmulationRunning = false;
        static bool IsKH2Running = false;
        static bool IsTitlePassed = false;
        static bool IsMapLoaded = false;
        static bool IsPlayerInMap = false;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32", EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, [Out] int lpNumberOfBytesWritten);

        public static bool SeekPCSX2()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName.Contains("pcsx2"))
                {
                    PCSX2 = p;
                    PCSX2_hProc = OpenProcess(PROCESS_ALL_ACCESS, true, PCSX2.Id);
                    BaseAddr = PCSX2.MainModule.BaseAddress;
                    return true;
                }
            }
            return false;
        }

        #region Read methods
        public static byte[] ReadBytes(int address, int arrayLength)
        {
            var buffer = new byte[arrayLength];
            int bytesRead = 0;
            ReadProcessMemory(PCSX2_hProc, (IntPtr)address, buffer, buffer.Length, out bytesRead);
            return buffer;
        }
        public static short ReadShort(int address)
        {
            return BitConverter.ToInt16(ReadBytes(address, 2), 0);
        }
        public static int ReadInteger(int address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4), 0);
        }
        public static uint ReadUInteger(int address)
        {
            return BitConverter.ToUInt32(ReadBytes(address, 4), 0);
        }
        public static string ReadString(int address, int length = 4)
        {
            byte[] buffer = ReadBytes(address, length);
            int auto_length = 0;
            while (auto_length < buffer.Length && buffer[auto_length] != 0) { auto_length++; }
            return System.Text.Encoding.ASCII.GetString(SubArray(buffer, 0, auto_length)).Replace("\u0001", string.Empty);
        }
        #endregion

        #region Write methods
        public static void WriteBytes(int address, byte[] value)
        {
            WriteProcessMemory(PCSX2_hProc, (IntPtr)address, value, value.Length, 0);
        }
        public static void WriteShort(int address, short v)
        {
            WriteBytes(address, BitConverter.GetBytes(v));
        }
        public static void WriteInteger(int address, int v)
        {
            WriteBytes(address, BitConverter.GetBytes(v));
        }
        public static void WriteUInteger(int address, uint v)
        {
            WriteBytes(address, BitConverter.GetBytes(v));
        }
        #endregion

        #region Search methods
        public static int SearchBytes(byte[] value, int startAddr)
        {
            int nStartAddr = startAddr;
            int bytesRead = 0;
            bool success = false;

            while (!success)
            {
                byte[] buffer = new byte[value.Length];
                ReadProcessMemory(PCSX2_hProc, (IntPtr)nStartAddr, buffer, buffer.Length, out bytesRead);

                if (value.SequenceEqual(buffer))
                {
                    success = true;
                    return nStartAddr;
                }
                nStartAddr += 4;
            }
            return 0;
        }

        public static int SearchBytes(byte[] value)
        {
            return SearchBytes(value, 0x21A00000);
        }
        public static int SearchText(string text)
        {
            return SearchBytes(Encoding.ASCII.GetBytes(text));
        }
        public static int SearchText(string text, int startAtAddress)
        {
            return SearchBytes(Encoding.ASCII.GetBytes(text), startAtAddress);
        }
        #endregion

        #region Soraikos methods i shamelessly copied
        static public byte[] SubArray(byte[] data, long index, long length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        static int Pointer(int address, int offset)
        {
            return Som(ReadInteger(address), offset);
        }
        static int Som(int entree1, int entree2)
        {
            int somme = 0;
            try { somme = entree1 + entree2; } catch { Console.WriteLine("Pointer destination erroned, overflow occured: " + entree1.ToString() + " (" + entree1.ToString("X") + ") + " + entree2.ToString() + " (" + entree2.ToString("X")); }
            return somme;
        }
        #endregion

        public static bool IsRoomWithPlayerLoaded()
        {
            bool IsMapLoaded = IsRoomLoaded();
            IsPlayerInMap = IsMapLoaded && ReadInteger(IngameConstants.PLAYER_PTR) > 0 && ReadInteger(Pointer(IngameConstants.PLAYER_PTR, 0x20000148)) == ReadInteger(IngameConstants.PLAYER_PTR);

            return IsMapLoaded && IsPlayerInMap;
        }

        public static bool IsRoomLoaded()
        {
            IsEmulationRunning = ReadInteger(0x20000000) != 0;
            IsKH2Running = IsEmulationRunning && ReadInteger(0x21C94008) == 0x2E32484B;
            IsTitlePassed = IsKH2Running && ReadInteger(0x21D9EAAC) != 0;
            IsMapLoaded = IsTitlePassed && ReadBytes(0x21C6CAF7, 1)[0] == 1;
            return IsMapLoaded;
        }
    }
}
