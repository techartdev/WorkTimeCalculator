/*
 MIT License

Copyright (c) 2022 Tech Art Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WorkTimeCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<DatesRange> DatesRange { get; set; } = new ObservableCollection<DatesRange>();

        public MainWindow()
        {
            InitializeComponent();
            DatesRange.CollectionChanged += DatesRange_CollectionChanged;
            DataContext = this;
        }

        private void DatesRange_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RecalcTotal();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (startDate.SelectedDate == null)
            {
                MessageBox.Show("Моля, изберете начална дата!");
                return;
            }

            if (endDate.SelectedDate == null)
            {
                MessageBox.Show("Моля, изберете крайна дата!");
                return;
            }

            if (endDate.SelectedDate < startDate.SelectedDate)
            {
                MessageBox.Show("Началната дата не може да бъде по-малка от крайната!");
                return;
            }

            var range = new DatesRange
            {
                StartDate = startDate.SelectedDate.Value,
                EndDate = endDate.SelectedDate.Value
            };

            range.PropertyChanged += Range_PropertyChanged;

            DatesRange.Add(range);
        }

        private void Range_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RecalcTotal();
        }

        private void RecalcTotal()
        {
            resultTb.Text = "Общо време: " + TimeSpanExtentions.PeriodBetween(DateTime.Now, DateTime.Now.AddDays(DatesRange.Sum(s => (s.EndDate - s.StartDate).TotalDays)), 3);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }
    }

    public static class TimeSpanExtentions
    {
        public static string PeriodBetween(DateTime then, DateTime now, byte numberOfPeriodUnits = 2)
        {
            // Translated from VB.Net to C# from: https://stackoverflow.com/a/1956265

            // numberOfPeriodUnits identifies how many time period units to show.
            // If numberOfPeriodUnits = 3, function would return:
            //      "3 years, 2 months and 13 days"
            // If numberOfPeriodUnits = 2, function would return:
            //      "3 years and 2 months"
            // If numberOfPeriodUnits = 6, (maximum value), function would return:
            //      "3 years, 2 months, 13 days, 13 hours, 29 minutes and 9 seconds"

            if (numberOfPeriodUnits > 6 || numberOfPeriodUnits < 1)
            {
                throw new ArgumentOutOfRangeException($"Parameter [{nameof(numberOfPeriodUnits)}] is out of bounds. Valid range is 1 to 6.");
            }

            short Years = 0;
            short Months = 0;
            short Days = 0;
            short Hours = 0;
            short Minutes = 0;
            short Seconds = 0;
            short DaysInBaseMonth = (short)(DateTime.DaysInMonth(then.Year, then.Month));

            Years = (short)(now.Year - then.Year);

            Months = (short)(now.Month - then.Month);
            if (Months < 0)
            {
                Months += 12;
                Years--; // add 1 year to months, and remove 1 year from years.
            }

            Days = (short)(now.Day - then.Day);
            if (Days < 0)
            {
                Days += DaysInBaseMonth;
                Months--;
            }

            Hours = (short)(now.Hour - then.Hour);
            if (Hours < 0)
            {
                Hours += 24;
                Days--;
            }

            Minutes = (short)(now.Minute - then.Minute);
            if (Minutes < 0)
            {
                Minutes += 60;
                Hours--;
            }

            Seconds = (short)(now.Second - then.Second);
            if (Seconds < 0)
            {
                Seconds += 60;
                Minutes--;
            }

            // This is the display functionality.
            StringBuilder TimePeriod = new StringBuilder();
            short NumberOfPeriodUnitsAdded = 0;

            if (Years > 0)
            {
                TimePeriod.Append(Years);
                TimePeriod.Append(" годин" + (Years != 1 ? "и" : "а") + ", ");
                NumberOfPeriodUnitsAdded++;
            }

            if (numberOfPeriodUnits == NumberOfPeriodUnitsAdded)
            {
                goto ParseAndReturn;
            }

            if (Months > 0)
            {
                TimePeriod.AppendFormat(Months.ToString());
                TimePeriod.Append(" месец" + (Months != 1 ? "а" : "") + ", ");
                NumberOfPeriodUnitsAdded++;
            }

            if (numberOfPeriodUnits == NumberOfPeriodUnitsAdded)
            {
                goto ParseAndReturn;
            }

            if (Days > 0)
            {
                TimePeriod.Append(Days);
                TimePeriod.Append(" " + (Days != 1 ? "дни" : "ден") + ", ");
                NumberOfPeriodUnitsAdded++;
            }

            if (numberOfPeriodUnits == NumberOfPeriodUnitsAdded)
            {
                goto ParseAndReturn;
            }

            if (Hours > 0)
            {
                TimePeriod.Append(Hours);
                TimePeriod.Append(" час" + (Hours != 1 ? "а" : "") + ", ");
                NumberOfPeriodUnitsAdded++;
            }

            if (numberOfPeriodUnits == NumberOfPeriodUnitsAdded)
            {
                goto ParseAndReturn;
            }

            if (Minutes > 0)
            {
                TimePeriod.Append(Minutes);
                TimePeriod.Append(" минут" + (Minutes != 1 ? "и" : "а") + ", ");
                NumberOfPeriodUnitsAdded++;
            }

            if (numberOfPeriodUnits == NumberOfPeriodUnitsAdded)
            {
                goto ParseAndReturn;
            }

            if (Seconds > 0)
            {
                TimePeriod.Append(Seconds);
                TimePeriod.Append(" секунд" + (Seconds != 1 ? "и" : "а") + "");
                NumberOfPeriodUnitsAdded++;
            }

        ParseAndReturn:
            // If the string is empty, that means the datetime is less than a second in the past.
            // An empty string being passed will cause an error, so we construct our own meaningful
            // string which will still fit into the "Posted * ago " syntax.

            if (TimePeriod.ToString() == "")
            {
                TimePeriod.Append("по малко от 1 секунда");
            }

            return TimePeriod.ToString().TrimEnd(' ', ',').ToString().ReplaceLast(",", " и");
        }

        public static string ReplaceLast(this string source, string search, string replace)
        {
            int pos = source.LastIndexOf(search);

            if (pos == -1)
            {
                return source;
            }

            return source.Remove(pos, search.Length).Insert(pos, replace);
        }
    }
}
