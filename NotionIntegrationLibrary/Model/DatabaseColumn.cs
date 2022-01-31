using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class DatabaseColumn
    {
        //Columns
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }


    }

}