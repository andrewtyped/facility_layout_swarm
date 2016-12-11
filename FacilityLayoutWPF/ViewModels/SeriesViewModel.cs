using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FacilityLayoutWPF.ViewModels
{
    public class SeriesViewModel
    {
        public IEnumerable<Point> Points { get; }

        public Point MinDisplayPoint { get; }

        public Point MaxDisplayPoint { get; }

        public Point Intervals { get; set; }

        public SeriesViewModel(IEnumerable<Point> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            Points = points;
            MinDisplayPoint = GetMinDisplay();
            MaxDisplayPoint = GetMaxDisplay();

        }

        private Point GetMinDisplay()
        {
            var minX = Math.Floor(Points.Min(p => p.X));
            var minY = Math.Floor(Points.Min(p => p.Y));

            return new Point(minX, minY);
        }

        public Point GetMaxDisplay()
        {
            var maxX = Math.Ceiling(Points.Max(p => p.X));
            var maxY = Math.Ceiling(Points.Max(p => p.Y));

            return new Point(maxX, maxY);
        }

        public Point GetXInterval()
        {
            var xRange = MaxDisplayPoint.X - MinDisplayPoint.X;
            var yRange = MaxDisplayPoint.Y - MinDisplayPoint.Y;


            return new Point(Math.Floor(Math.Sqrt(xRange)), Math.Floor(Math.Sqrt(yRange)));
        }

    }
}
