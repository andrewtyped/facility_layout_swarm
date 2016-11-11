using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityLayoutWPF.ViewModels
{
    public class FacilityLayoutOptionsViewModel : BindableBase
    {
        private int? _termiteCount;

        public int? TermiteCount
        {
            get { return _termiteCount; }
            set { SetProperty(ref _termiteCount, value); }
        }

        private int? _phase1Decay;

        public int? Phase1Decay
        {
            get { return _phase1Decay; }
            set { SetProperty(ref _phase1Decay, value); }
        }

        private int? _phase2Decay;

        public int? Phase2Decay
        {
            get { return _phase2Decay; }
            set { SetProperty(ref _phase2Decay, value); }
        }

        private int? _gravitationStartPoint;

        public int? GravitationStartPoint
        {
            get { return _gravitationStartPoint; }
            set { SetProperty(ref _gravitationStartPoint, value); }
        }

        private int? _greedyScholarRatio;

        public int? GreedyScholarRatio
        {
            get { return _greedyScholarRatio; }
            set { SetProperty(ref _greedyScholarRatio, value); }
        }

        private int? _solutionCount;

        public int? SolutionCount
        {
            get { return _solutionCount; }
            set { SetProperty(ref _solutionCount, value); }
        }
    }
}
