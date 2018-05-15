//This functions are simply copied from Teamod. Shame on me
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace KHMemHax_ReCoded.Utils
{
    public static class DataFunctions
    {
        static public byte[] SubArray(byte[] data, long index, long length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        //Combine two bytes array
        static public byte[] Combine(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
            System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }

        //Search a byte array in another byte array
        static public int SearchBytes(byte[] input, byte[] needle)
        {
            var len = needle.Length;
            var limit = input.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                    if (needle[k] != input[i + k]) break;
                if (k == len) return i;
            }
            return -1;
        }

        //Make a som without exception
        static public int Som(int entree1, int entree2)
        {
            int somme = 0;
            try { somme = entree1 + entree2; } catch { Console.WriteLine("Pointer destination erroned, overflow occured: " + entree1.ToString() + " (" + entree1.ToString("X") + ") + " + entree2.ToString() + " (" + entree2.ToString("X")); }
            return somme;
        }
    }

    public static class PCSX2_DMA
    {
        static Process PCSX2;
        static IntPtr PCSX2_Id;

        //Handle PCSX2
        [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        //Read from PCSX2
        [DllImport("kernel32.dll", SetLastError = false)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int BytesRead);

        //Write to PCSX2
        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, [Out] int lpNumberOfBytesWritten);

        //Search PCSX2 Process
        public static void SeekPCSX2()
        {
            try
            {
                Process.GetProcessById(PCSX2.Id);
                //if (found_info != null) found_info.Text = LanguageStrings[1] + " [" + PCSX2.ProcessName + "]";
            }
            catch
            {
                PCSX2_Id = (IntPtr)0;
                //if (found_info != null) found_info.Text = LanguageStrings[0];
                foreach (Process IsPCSX2 in Process.GetProcesses())
                    if (IsPCSX2.ProcessName.Contains("pcsx2"))
                    {
                        PCSX2 = IsPCSX2;
                        PCSX2_Id = OpenProcess(0x001F0FFF, true, PCSX2.Id);
                        SeekPCSX2();
                    }
            }
        }

        //Read Bytes from PCSX2 RAM
        public static byte[] ReadBytes(int address, int array_length)
        {
            var buffer = new byte[array_length];
            int BytesRead = 0;
            ReadProcessMemory(PCSX2_Id, new IntPtr(address), buffer, array_length, out BytesRead);
            return buffer;
        }

        //Write Bytes to PCSX2 RAM
        public static void WriteBytes(int address, byte[] ValueToWrite)
        {
            bool abort = false;
            while (/*IsTitlePassed && */ReadInt32(0x21C94008) != 0x2E32484B)
                abort = true;
            if (abort)
            {
                //firstMoveLoaded = 0;
                return;
            }
            WriteProcessMemory(PCSX2_Id, (IntPtr)address, ValueToWrite, ValueToWrite.Length, 0);
        }

        //Read Integer32 from PCSX2 RAM
        public static int ReadInt32(int address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4), 0);
        }

        //Read Integer32 from PCSX2 RAM
        public static ushort ReadUShort(int address)
        {
            return BitConverter.ToUInt16(ReadBytes(address, 2), 0);
        }

        //Read Integer16 from PCSX2 RAM
        public static short ReadInt16(int address)
        {
            return BitConverter.ToInt16(ReadBytes(address, 2), 0);
        }

        //Read Float from PCSX2 RAM
        public static float ReadFloat(int address)
        {
            return BitConverter.ToSingle(ReadBytes(address, 4), 0);
        }

        //Write Float to PCSX2 RAM
        public static void WriteFloat(int address, float valeur)
        {
            WriteBytes(address, BitConverter.GetBytes(valeur));
        }

        //Read String from PCSX2 RAM
        public static string ReadString(int address, int length)
        {
            byte[] buffer = ReadBytes(address, length);
            int auto_length = 0;
            while (/*IsTitlePassed && */auto_length < buffer.Length && buffer[auto_length] != 0) { auto_length++; }
            return System.Text.Encoding.ASCII.GetString(Utils.DataFunctions.SubArray(buffer, 0, auto_length));
        }

        //Write Integer32 to PCSX2 RAM
        public static void WriteInteger(int address, int ValueToWrite)
        {
            WriteBytes(address, BitConverter.GetBytes(ValueToWrite));
        }

        //Write Integer16 to PCSX2 RAM
        public static void WriteInteger(int address, short ValueToWrite)
        {
            WriteBytes(address, BitConverter.GetBytes(ValueToWrite));
        }

        //Write Integer16 to PCSX2 RAM
        public static void WriteInteger(int address, byte ValueToWrite)
        {
            WriteBytes(address, new byte[] { ValueToWrite });
        }

        //Write string to PCSX2 RAM
        public static void WriteString(int address, string ValueToWrite)
        {
            WriteBytes(address, Encoding.ASCII.GetBytes(ValueToWrite + "\x0"));
        }

        //Get a pointer som
        public static int Pointer(int address, int offset)
        {
            return Utils.DataFunctions.Som(ReadInt32(address), offset);
        }

        public static string GetModelName(int pointer)
        {
            return ReadString(Pointer(Pointer(pointer, 0x20000008), 0x20000008), 32);
        }
    }
}
