using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class DatabaseResponse
    {
        [JsonProperty("object")]
        public string objecttype { get; set; }
        [JsonProperty("results")]
        public List<NotionDatabaseObject> Results { get; set; }

        [JsonProperty("title")]
        public List<Text> Title { get; set; }

        [JsonProperty("has_more")]
        public Boolean HasMore { get; set; }
        [JsonProperty("start_cursor")]
        public string Start_cursor { get; set; }
        [JsonProperty("next_cursor")]
        public string Next_cursor { get; set; }
        [JsonProperty("page_size")]
        public int Page_size { get; set; }
        
    }

}