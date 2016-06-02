using Autofac;
using eFocus.VRIS.Core.Repositories;
using eFocus.VRIS.Datasources.MicrosoftGraph.Providers;

namespace eFocus.VRIS.Datasources.MicrosoftGraph
{
    public class MicrosoftGraphModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MicrosoftGraphTokenProvider>().SingleInstance();
            builder.RegisterType<MicrosoftGraphRepository>().As<ICalenderRepository>();
        }
    }
}
