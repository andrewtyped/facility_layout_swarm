using FacilityLayoutWPF.Services;
using FaciltyLayout.Core;
using FaciltyLayout.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FacilityLayoutWPF.ViewModels
{
    public class FacilityLayoutViewModel : BindableBase
    {
        public FacilityLayoutOptionsViewModel Options { get; }

        private FacilityStats _facilityStats;

        public FacilityStats FacilityStats
        {
            get
            {
                return _facilityStats;
            }
            private set
            {
                _facilityStats = value;
                OnPropertyChanged(nameof(FacilityStatsDisplay));
            }
        }

        private FacilityLayoutSolution _selectedSolution;

        public FacilityLayoutSolution SelectedSolution
        {
            get { return _selectedSolution; }
            set { SetProperty(ref _selectedSolution, value); }
        }


        private ObservableCollection<FacilityLayoutSolution> _solutions;

        public ObservableCollection<FacilityLayoutSolution> Solutions
        {
            get
            {
                return _solutions;
            }
            set
            {
                SetProperty(ref _solutions, value);
            }
        }

        public string FacilityStatsDisplay
        {
            get { return FacilityStats?.ToString() ?? ""; }
        }

        

        private readonly IIOService _ioService = new FacilityLayoutIOService();

        public FacilityLayoutViewModel()
        {
            Options = new FacilityLayoutOptionsViewModel();
            LoadFacilityData = new RelayCommand(OnLoadFacilityData);
            Solve = new RelayCommand(OnSolve);
            Solutions = new ObservableCollection<FacilityLayoutSolution>();
        }

        public RelayCommand LoadFacilityData { get; }

        private void OnLoadFacilityData()
        {
            var pathToData = _ioService.OpenFileDialog();

            if (pathToData == null)
                return;

            var facilityStatsRepository = new FacilityStatsRepository(pathToData);
            FacilityStats = facilityStatsRepository.Load();
        }

        public RelayCommand Solve { get; }

        private async void OnSolve()
        {
            var tileOrganizerOptions = new TileOrganizerOptions(
                Options.TermiteCount, 
                Options.Phase1Decay, 
                Options.Phase2Decay, 
                Options.SolutionCount, 
                Options.GravitationStartPoint, 
                Options.GreedyScholarRatio,
                20
           );


            var tileOrganizer = new TileOrganizer(tileOrganizerOptions);

            await Task.Run(() =>
            {
                foreach(var solution in tileOrganizer.ScholarMethod(FacilityStats))
                {
                    Action<FacilityLayoutSolution> Add = Solutions.Add;
                    Application.Current.Dispatcher.BeginInvoke(Add, solution);
                }
            });



        }
    }
}
