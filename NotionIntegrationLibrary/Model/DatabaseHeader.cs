using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NotionIntegrationLibrary
{

    /* public class TextContent
     {
         [JsonProperty("content")]
         public String Content { get; set; }
     }*/


    public class DatabaseHeader
        {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

    }

}