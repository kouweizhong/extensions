﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;

namespace Signum.Entities.Extensions.Basics
{
    [Serializable]
    public class DateSpanDN : EmbeddedEntity
    {
        int years;
        public int Years
        {
            get { return years; }
            set { Set(ref years, value, "Years"); }
        }

        int months;
        public int Months
        {
            get { return months; }
            set { Set(ref months, value, "Months"); }
        }

        int days;
        public int Days
        {
            get { return days; }
            set { Set(ref days, value, "Days"); }
        }

        public bool EsNulo()
        {
            return years == 0 && months == 0 && days == 0;        
        }

        public DateTime Add(DateTime date)
        {
            return date.AddYears(years).AddMonths(months).AddDays(days);
        }

        public DateSpan ToDateSpan()
        {
            return new DateSpan(years, months, days); 
        }
    }
}
