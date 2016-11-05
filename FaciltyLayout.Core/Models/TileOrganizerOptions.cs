using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaciltyLayout.Core.Models
{
    public class TileOrganizerOptions
    {
        public int? TermiteCount { get; }
        public int Phase1Decay { get; }
        public int Phase2Decay { get; }
        public int CycleCount { get; }
        public int GravStart { get; }
        public int RatioOfGreedyTermitesToScholarTermites {get;}
        public int UIUpdateFrequency { get; }

        public TileOrganizerOptions(int? termiteCount, int? phase1Decay, int? phase2Decay, int? cycleCount, int? gravStart, int? ratioOfGreedyTermitesToScholarTermites, int? uiUpdateFrequency)
        {
            TermiteCount = termiteCount;
            Phase1Decay = phase1Decay ?? 10;
            Phase2Decay = phase2Decay ?? 10;
            CycleCount = cycleCount ?? 10;
            GravStart = gravStart ?? 10;
            RatioOfGreedyTermitesToScholarTermites = ratioOfGreedyTermitesToScholarTermites ?? 3;
            UIUpdateFrequency = uiUpdateFrequency ?? 20000;
        }
    }
}
