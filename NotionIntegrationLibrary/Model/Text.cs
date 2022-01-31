using System.Collections.Generic;
using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class Text

    {
        [JsonProperty("text")]
        public Dictionary<string, string> TextContent { get; set; }
    }

}