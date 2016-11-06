using FaciltyLayout.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace FaciltyLayout.Core
{
    public class TileOrganizer
    {
        private readonly TileOrganizerOptions options;
        private readonly ContiguityTester contiguityTester = new ContiguityTester();
        private readonly FacilityEvaluator facilityEvaluator = new FacilityEvaluator();
        private object locker = new object();
        public TileOrganizer(TileOrganizerOptions options)
        {
            this.options = options;
        }

        public IEnumerable<FacilityLayoutSolution> GreedyMethod(FacilityStats facilityStats)
        {

            for (int cycle = 0; cycle < options.CycleCount; cycle++)
            {
                var startTime = DateTime.Now;

                var facilityLayoutModel = new FacilityLayoutModel(facilityStats);
                facilityLayoutModel.InitializeDepartmentTiles();
                facilityLayoutModel.TilePlaced += FacilityLayoutModel_OnTilePlaced;
                facilityLayoutModel.TileRemoved += FacilityLayoutModel_OnTileRemoved;

                OnFacilityInitialized(facilityLayoutModel, facilityStats);

                var rows = facilityLayoutModel.LayoutArea.Rows;
                var columns = facilityLayoutModel.LayoutArea.Columns;

                var termiteManager = new TermiteManager(facilityLayoutModel);
                var termiteCount = options.TermiteCount ?? (int)Round(rows * columns * 1.5); //HACK: magic number
                var termites = termiteManager.ReleaseTheTermites(termiteCount, options.RatioOfGreedyTermitesToScholarTermites);

                var totalContig = false;

                for(int loopCounter = 0;  !totalContig; loopCounter++)
                {
                    if (loopCounter % 25 == 0) //HACK: Magic number
                    {
                        facilityLayoutModel.UnlockTiles();

                        for (int y = 1; y < 50; y++) //HACK: magic number
                        {
                            MoveTiles(termites, facilityLayoutModel, facilityStats, 1);
                        }
                    }

                    MoveTiles(termites, facilityLayoutModel, facilityStats, 1); //HACK: magic number

                    Parallel.For(1, facilityStats.DepartmentCount + 1, (department) =>
                    {
                        var departmentIsContiguous = contiguityTester.DepartmentIsContiguous(department, facilityLayoutModel);

                        //try without lock first. Each iteration of this loop should affect non-overlapping segments of the facility model
                        if (departmentIsContiguous)
                            facilityLayoutModel.LockDeptTiles(department);
                    });

                    totalContig = contiguityTester.AllDepartmentsAreContiguous(facilityLayoutModel);

                    if (loopCounter % options.UIUpdateFrequency == 0)
                        OnOrganizerMileStoneReached(facilityLayoutModel.LayoutArea);                    
                }

                OnOrganizerMileStoneReached(facilityLayoutModel.LayoutArea);                    

                var volumeDistanceCostProduct = facilityEvaluator.VolumeDistanceCostProduct(facilityStats, facilityLayoutModel);
                var centroids = facilityEvaluator.CentroidCalculator(facilityStats, facilityLayoutModel);
                var runTime = DateTime.Now.Subtract(startTime);
                var solution = new FacilityLayoutSolution(volumeDistanceCostProduct, runTime, centroids, facilityLayoutModel.Facility);

                yield return solution;
            }
        }

        public IEnumerable<FacilityLayoutSolution> ScholarMethod(FacilityStats facilityStats)
        {

            var contigIndicator = false;
            var n = 0;

            var cycle = 0;
            var refreshCounter = 0;

            while(cycle <= options.CycleCount)
            {
                var facilityLayoutModel = new FacilityLayoutModel(facilityStats);
                facilityLayoutModel.InitializeDepartmentTiles();

                facilityLayoutModel.TilePlaced += FacilityLayoutModel_OnTilePlaced;
                facilityLayoutModel.TileRemoved += FacilityLayoutModel_OnTileRemoved;

                OnFacilityInitialized(facilityLayoutModel, facilityStats);

                var rows = facilityLayoutModel.LayoutArea.Rows;
                var columns = facilityLayoutModel.LayoutArea.Columns;

                var termiteManager = new TermiteManager(facilityLayoutModel);
                var loopPhase = 1;
                var loopCounter = 0;

                var termiteCount = options.TermiteCount ?? (int)Math.Round(rows * columns * 1.5); //HACK: magic number
                var termites = termiteManager.ReleaseTheTermites(termiteCount , options.RatioOfGreedyTermitesToScholarTermites);

                var startTime = DateTime.Now;

                var totalContig = false;
                //Achieve contiguity among all departments
                while (!totalContig)
                {
                    if (refreshCounter % 600 == 0) //HACK: Magic number
                    {
                        facilityLayoutModel.UnlockTiles();

                        for(int y = 1; y < 50; y++) //HACK: magic number
                        {
                            MoveTiles(termites, facilityLayoutModel, facilityStats, loopPhase);
                        }
                    }

                    MoveTiles(termites, facilityLayoutModel, facilityStats, loopPhase);
                    loopCounter++;
                    refreshCounter++;
                    totalContig = contiguityTester.AllDepartmentsAreContiguous(facilityLayoutModel);

                    if (n < termites.Count)
                    {
                        if (loopCounter % options.Phase1Decay == 0 && loopCounter >= options.GravStart - Round((double)options.GravStart / 4.0, 0)) //HACK: Magic number
                        {
                            termites[n] = termites[n].ChangeType<GreedyTermite>();
                            n++;
                        }
                    }

                    if (loopCounter >= options.GravStart + Round(options.GravStart / 2.0, 0))
                    {
                        for (int a = 1; a <= facilityStats.DepartmentCount; a++)
                        {
                            contigIndicator = contiguityTester.DepartmentIsContiguous(a, facilityLayoutModel);

                            if (!contigIndicator)
                                totalContig = false;
                            else
                                facilityLayoutModel.LockDeptTiles(a);
                        }
                    }

                    if (loopCounter % options.UIUpdateFrequency == 0)
                        OnOrganizerMileStoneReached(facilityLayoutModel.LayoutArea);

                }

                facilityLayoutModel.UnlockTiles();

                while(rows > facilityStats.FacilitySize.Rows && columns > facilityStats.FacilitySize.Columns)
                {
                    facilityLayoutModel.ReduceLayoutArea(termites, options.GravStart, ref loopCounter);
                    OnOrganizerMileStoneReached(new GridSize(rows, columns));
                    rows--;
                    columns--;
                }

                loopCounter = 0;
                n = 0;
                loopPhase = 2;
                facilityLayoutModel.UnlockTiles();

                var adjTileContainSameDepartment = false;
                //TODO: IMPLEMENT PHASE 2 NEXT
                do
                {
                    MoveTiles(termites, facilityLayoutModel, facilityStats, loopPhase);
                    loopCounter++;
                    totalContig = contiguityTester.AllDepartmentsAreContiguous(facilityLayoutModel);

                    if (n < termites.Count)
                    {
                        if (loopCounter % options.Phase1Decay == 0 && loopCounter >= options.GravStart - Round((double)options.GravStart / 4.0, 0)) //HACK: Magic number
                        {
                            termites[n] = termites[n].ChangeType<GreedyTermite>();
                            n++;
                        }
                    }

                    if (totalContig)
                    {
                        for (int i = 0; i < facilityLayoutModel.LayoutArea.Rows; i++)
                        {
                            for (int j = 0; j < facilityLayoutModel.LayoutArea.Columns; j++)
                            {
                                if (facilityLayoutModel.IsTileAssigned(i, j) == false)
                                {
                                    adjTileContainSameDepartment = contiguityTester.AdjacentTilesContainSameDepartment(termites[0].TileDept, i, j, facilityLayoutModel, facilityStats.DepartmentSizes);

                                    if (adjTileContainSameDepartment)
                                    {
                                        termites[0].DropTile(facilityLayoutModel, i, j);
                                        break;
                                    }
                                }
                            }

                            if (adjTileContainSameDepartment)
                                break;
                        }
                    }

                    if (loopCounter % options.UIUpdateFrequency == 0)
                        OnOrganizerMileStoneReached(facilityLayoutModel.LayoutArea);

                } while (!totalContig || !adjTileContainSameDepartment);

                OnOrganizerMileStoneReached(facilityLayoutModel.LayoutArea);

                var stopTime = DateTime.Now;
                var runTime = stopTime.Subtract(startTime);
                var volumeDistanceCostProduct = facilityEvaluator.VolumeDistanceCostProduct(facilityStats, facilityLayoutModel);
                var centroids = facilityEvaluator.CentroidCalculator(facilityStats, facilityLayoutModel);
                var solution = new FacilityLayoutSolution(volumeDistanceCostProduct, runTime, centroids, facilityLayoutModel.Facility);

                facilityLayoutModel.TilePlaced -= FacilityLayoutModel_OnTilePlaced;
                facilityLayoutModel.TileRemoved -= FacilityLayoutModel_OnTileRemoved;

                yield return solution;
            }
        }

        private void MoveTiles(List<Termites> termites, FacilityLayoutModel facilityLayoutModel, FacilityStats facilityStats, int loopPhase)
        {
            var hoardingTermite = loopPhase == 1 ? 0 : 1;

            for(int i = hoardingTermite; i < termites.Count; i++)
            {
                termites[i].MoveTile(facilityLayoutModel, facilityStats, contiguityTester);

            }
        }

        public event EventHandler<FacilityInitializedEventArgs> FacilityInitialized;
        public event EventHandler<GridEventArgs> OrganizerMilestoneReached;
        public event EventHandler<TileEventArgs> TilePlaced;
        public event EventHandler<TileEventArgs> TileRemoved;

        private void FacilityLayoutModel_OnTilePlaced(object sender, TileEventArgs e)
        {
            TilePlaced?.Invoke(sender, e);
        }

        private void FacilityLayoutModel_OnTileRemoved(object sender, TileEventArgs e)
        {
            TileRemoved?.Invoke(sender, e);
        }

        private void OnFacilityInitialized(FacilityLayoutModel facilityLayoutModel, FacilityStats facilityStats)
        {
            FacilityInitialized?.Invoke(this, new FacilityInitializedEventArgs(facilityLayoutModel, facilityStats));
        }

        private void OnOrganizerMileStoneReached(GridSize layoutArea)
        {
            OrganizerMilestoneReached?.Invoke(this, new GridEventArgs(layoutArea));
        }
    }
}
