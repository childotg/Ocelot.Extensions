using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.Extensions.Configuration.GoogleCloudStorage
{
   
    public class GoogleCloudStorageConfiguration
    {
        public string AccessKey { get; set; }
        public string Secret { get; set; }
        public string BucketName { get; set; }
        public string ObjectName { get; set; }
        
    }

}
