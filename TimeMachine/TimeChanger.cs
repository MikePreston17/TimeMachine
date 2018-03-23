using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TimeMachine
{
    public class TimeChanger
    {
        public void GetTime()
        {
            // Call the native GetSystemTime method
            // with the defined structure.
            var systemTime = new SYSTEMTIME();
            GetSystemTime(ref systemTime);

            // Show the current time.           
            Debug.WriteLine("Current Time: " +
                systemTime.wHour.ToString() + ":"
                + systemTime.wMinute.ToString());
        }

        public bool ChangeTime(DateTime time) => ChangeTime((short)time.Year, (short)time.Month, (short)time.Day, (short)time.Hour, (short)time.Minute, (short)time.Second);

        public bool ChangeTime(short year, short month, short day, short hour, short minute, short second)
        {
            try
            {
                var systemTime = new SYSTEMTIME
                {
                    wYear = year,
                    wMonth = month,
                    wDay = day,
                    wHour = hour,
                    wMinute = minute,
                    wSecond = second
                };

                SYSTEMTIME.SetLocalTime(ref systemTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        [DllImport("coredll.dll")]
        private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("coredll.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        private static void SetTime()
        {
            // Call the native GetSystemTime method
            // with the defined structure.
            var systime = new SYSTEMTIME();
            GetSystemTime(ref systime);

            // Set the system clock ahead one hour.
            systime.wHour = (short)(systime.wHour + 1 % 24);
            SetSystemTime(ref systime);
            Debug.WriteLine("New time: " + systime.wHour.ToString() + ":"
                + systime.wMinute.ToString());
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetLocalTime([In] ref SYSTEMTIME st);
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }
    }
}
