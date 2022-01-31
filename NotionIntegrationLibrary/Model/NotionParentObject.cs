using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class NotionParentObject
    {

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("page_id")]
        public string PageId { get; set; }

    }

}