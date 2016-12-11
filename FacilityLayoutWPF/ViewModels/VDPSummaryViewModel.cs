using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaciltyLayout.Core.Models;
using System.Windows;

namespace FacilityLayoutWPF.ViewModels
{
    public class VDPSummaryViewModel : ISolutionSummarizer, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public SeriesViewModel VDPSummary { get; private set; }

        public void Initialize(IEnumerable<FacilityLayoutSolution> solutions)
        {
            VDPSummary = new SeriesViewModel(solutions.Select((solution,i) => 
                                                               new Point(i, solution.VolumeDistanceCostProduct)));
        }
    }
}
