using System;
using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{
    public class DateObj
    {
        [JsonProperty("start")]
        public DateTime startDate { get; set; }

        //  [JsonProperty("end")]
        //   public DateTime endDate { get; set; }

        //   [JsonProperty("time_zone")]
        //  public String timeZone { get; set; }
    }

}