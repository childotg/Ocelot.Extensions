using Ocelot.Configuration.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.Extensions.Common
{
    public class FileConfigurationExtended: FileConfiguration
    {
        public FileConfigurationExtended()
        {

        }

        public FileConfigurationExtended(FileConfiguration model)
        {
            if (model != null)
            {
                this.Aggregates = model.Aggregates;
                this.DynamicReRoutes = model.DynamicReRoutes;
                this.GlobalConfiguration = model.GlobalConfiguration;
                this.ReRoutes = model.ReRoutes;
            }
        }

        public RouteExtensions RouteExtensions { get; set; }
    }

    public class RouteExtensions
    {
        public RouteExtensionsReplaceHandler ReplaceHandler { get; set; }
        
        
        
    }

    public class RouteExtensionsReplaceHandler
    {
        public int Settings { get; set; }
        public RouteExtensionsReplaceHandlerRule[] Rules { get; set; }
    }

    public class RouteExtensionsReplaceHandlerRule
    {
        public string[] AppliesTo { get; set; }
        public RouteExtensionsReplaceHandlerFindReplaceHeader[] ReplaceDownstreamHeaders { get; set; }
        public RouteExtensionsReplaceHandlerFindReplace[] ReplaceDownstreamContent { get; set; }
        public RouteExtensionsReplaceHandlerFindReplace[] ReplaceUpstreamContent { get; set; }


    }

    public class RouteExtensionsReplaceHandlerFindReplaceHeader : RouteExtensionsReplaceHandlerFindReplace
    {
        public string Header { get; set; }
    }

    public class RouteExtensionsReplaceHandlerFindReplace
    {
        public string Find { get; set; }
        public string Replace { get; set; }
    }
}
