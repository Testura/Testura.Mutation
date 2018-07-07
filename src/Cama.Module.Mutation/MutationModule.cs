using Cama.Infrastructure;
using Cama.Module.Mutation.Sections.MutationDetails;
using Cama.Module.Mutation.Sections.MutationOverview;
using Cama.Module.Mutation.Sections.Shell;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Cama.Module.Mutation
{
    public class MutationModule : IModule
    {
        public const string MutationOverviewRegionName = "MutationOverviewRegion";
        public const string MutationDetailsRegionName = "MutationDetailsRegionName";

        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public MutationModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(MutationShellView));
            _regionManager.RegisterViewWithRegion(MutationOverviewRegionName, typeof(MutationDetailsView));
            _regionManager.RegisterViewWithRegion(MutationDetailsRegionName, typeof(MutationOverviewView));
        }
    }
}
