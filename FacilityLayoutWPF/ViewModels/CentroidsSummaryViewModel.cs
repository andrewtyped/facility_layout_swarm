using FaciltyLayout.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FacilityLayoutWPF.ViewModels
{
    public class CentroidsSummaryViewModel : ISolutionSummarizer, INotifyPropertyChanged
    {
        public Dictionary<int, SeriesViewModel> CentroidSummary { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void Initialize(IEnumerable<FacilityLayoutSolution> solutions)
        {
            var centroidPoints = new Dictionary<int, List<Point>>();

            var centroidPositions = solutions.SelectMany(s => s.DepartmentCentroids).ToList();

            var departments = centroidPositions.Select(cd => cd.Key).Distinct();

            foreach(var department in departments)
            {
                centroidPoints[department] = new List<Point>();
            }

            foreach(var centroidPosition in centroidPositions)
            {
                centroidPoints[centroidPosition.Key].Add(new Point(centroidPosition.Value.Column, centroidPosition.Value.Row));
            }

            CentroidSummary = centroidPoints.ToDictionary(cp => cp.Key, cp => new SeriesViewModel(cp.Value));
        }
    }
}
