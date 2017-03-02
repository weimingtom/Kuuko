using System;
using System.Collections.Generic;
using System.Text;

namespace KopiLua
{
    public class DateTimeProxy
    {
        private DateTime stm;

        public DateTimeProxy()
        {
            
        }

        public DateTimeProxy(int year, int month, int day, int hour, int min, int sec)
        {
            stm = new DateTime(year, month, day, hour, min, sec);
        }

        public void setUTCNow()
        {
            stm = DateTime.UtcNow;
        }

        public void setNow()
        {
            stm = DateTime.Now;
        }

        public int getSecond()
        {
            return stm.Second;
        }

        public int getMinute()
        {
            return stm.Minute;
        }

        public int getHour()
        {
            return stm.Hour;
        }

        public int getDay()
        {
            return stm.Day;
        }

        public int getMonth()
        {
            return stm.Month;
        }

        public int getYear()
        {
            return stm.Year;
        }

        public int getDayOfWeek()
        {
            return (int)stm.DayOfWeek;
        }

        public int getDayOfYear()
        {
            return stm.DayOfYear;
        }

        public bool IsDaylightSavingTime()
        {
            return stm.IsDaylightSavingTime();
        }

        public double getTicks()
        {
            return stm.Ticks;
        }

        public static double getClock()
        {
            long ticks = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return ((Double/*lua_Number*/)ticks) / (Double/*lua_Number*/)1000;
        }
    }
}
