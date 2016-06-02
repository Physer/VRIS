using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using eFocus.VRIS.Datasources.MicrosoftGraph;

namespace eFocus.VRIS.Infrastructure
{
    public class InfrastructureModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<MicrosoftGraphModule>();
        }
    }
}
