using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class ColumnValue
    {
        //rows or field data types

        [JsonProperty("date")]
        public DateObj DateObj { get; set; }


        [JsonProperty("select")]
        public SelectObj SelectObj { get; set; }

        [JsonProperty("title")]
        public List<Text> Title { get; set; }

        [JsonProperty("rich_text")]
        public List<Text> RichText { get; set; }

        [JsonProperty("phone_number")]
        public String Phone { get; set; }

        [JsonProperty("email")]
        public String Email { get; set; }

        [JsonProperty("checkbox")]
        public Nullable<Boolean> Checkbox { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
            
        [JsonProperty("number")]
        public Nullable<decimal> Number { get; set; }


    }

}