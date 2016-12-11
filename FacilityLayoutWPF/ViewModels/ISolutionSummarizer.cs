using FaciltyLayout.Core.Models;
using System.Collections.Generic;

namespace FacilityLayoutWPF.ViewModels
{
    public interface ISolutionSummarizer
    {
        void Initialize(IEnumerable<FacilityLayoutSolution> solutions);
    }
}