using System.Collections.Generic;
using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class NotionDatabaseObject
    {
        [JsonProperty("object")]
        public string Objecttype { get; set; }


        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public List<Text> Title { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, DatabaseColumn> properties { get; set; }
        [JsonProperty("parent")]
        public NotionParentObject Parent { get; set; }

    }

}