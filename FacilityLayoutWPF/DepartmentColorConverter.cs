using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FacilityLayoutWPF
{
    class DepartmentColorConverter : IValueConverter
    {
        private static ReadOnlyDictionary<int, Brush> _departmentColors = new ReadOnlyDictionary<int, Brush>(
            new Dictionary<int, Brush>()
            {
                {0, Brushes.Black },
                {1, Brushes.AliceBlue },
                {2, Brushes.Aquamarine },
                {3, Brushes.Beige },
                {4, Brushes.CadetBlue },
                {5, Brushes.BurlyWood },
                {6, Brushes.CornflowerBlue },
                {7, Brushes.DarkOrange },
                {8, Brushes.DarkSalmon },
                {9, Brushes.DeepSkyBlue },
                {10, Brushes.DarkSeaGreen },
                {11, Brushes.FloralWhite },
                {12, Brushes.Khaki },
                {13, Brushes.MistyRose },
                {14, Brushes.PaleGreen },
                {15, Brushes.Plum },
                {16, Brushes.Silver },
                {17, Brushes.Violet },
                {18, Brushes.Wheat },
                {19, Brushes.SandyBrown },
                {20, Brushes.Purple },
            });

        public static IReadOnlyDictionary<int, Brush> DepartmentColors
        {
            get
            {
                return _departmentColors;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intVal = (int)value;
            return DepartmentColors[intVal];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
