using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class NotionPageObject
    {
        [JsonProperty("object")]
        public string Objecttype { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, ColumnValue> properties { get; set; }
        [JsonProperty("parent")]
        public NotionParentObject Parent { get; set; }

        [JsonProperty("created_time")]
        public string Created_Time { get; set; }

        [JsonProperty("last_edited_time")]
        public DateTime Last_Edited_Time { get; set; }

    }

}