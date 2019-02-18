using Ocelot.Configuration.File;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.Extensions.Common
{
    public class FileConfigurationExtended: FileConfiguration
    {
        public OcelotExtensions Extensions { get; set; }
    }

    public class OcelotExtensions
    {
        public RouteExtensionsDelegatingHandlers DelegatingHandlerExtensions { get; set; }
    }

    public class RouteExtensionsDelegatingHandlers
    {
        public RouteExtensionsDelegatingHandlersReplaceHandler ReplaceHandler { get; set; }
    }

    public class RouteExtensionsDelegatingHandlersReplaceHandler
    {
        public string[] AppliesToRoutes { get; set; }
        public RouteExtensionsDelegatingHandlersReplaceHandlerFindReplaceHeader[] ReplaceDownstreamHeaders { get; set; }
        public RouteExtensionsDelegatingHandlersReplaceHandlerFindReplace[] ReplaceDownstreamContent { get; set; }
        public RouteExtensionsDelegatingHandlersReplaceHandlerFindReplace[] ReplaceUpstreamContent { get; set; }


    }

    public class RouteExtensionsDelegatingHandlersReplaceHandlerFindReplaceHeader : RouteExtensionsDelegatingHandlersReplaceHandlerFindReplace
    {
        public string Header { get; set; }
    }

    public class RouteExtensionsDelegatingHandlersReplaceHandlerFindReplace
    {
        public string Find { get; set; }
        public string Replace { get; set; }
    }
}
