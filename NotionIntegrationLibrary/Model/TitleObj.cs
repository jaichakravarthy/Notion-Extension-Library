using System.Collections.Generic;
using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class TitleObj
    {
        [JsonProperty("title")]
        public Dictionary<string, Text> Title { get; set; }


    }

}