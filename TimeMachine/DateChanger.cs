using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TimeMachine
{
    public static class DateChanger
    {
        [DllImport("coredll.dll")]
        private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("coredll.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

        public static void GetTime()
        {
            // Call the native GetSystemTime method
            // with the defined structure.
            var stime = new SYSTEMTIME();
            GetSystemTime(ref stime);

            // Show the current time.           
            Debug.WriteLine("Current Time: " +
                stime.wHour.ToString() + ":"
                + stime.wMinute.ToString());
        }

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

        public static bool ChangeDateTime(short year, short month, short day, short hour, short minute, short second)
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
