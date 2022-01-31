using System;
using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class SelectObj
    {
        //[JsonProperty("id")]
        //public String id { get; set; }

        [JsonProperty("name")]
        public String name
        {
            get; set;
        }

        [JsonProperty("color")]
        public String color { get; set; }
    }

}