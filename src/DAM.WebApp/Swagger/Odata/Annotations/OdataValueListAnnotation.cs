using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace DAM.WebApp.Swagger.Odata.Annotations
{
    public class OdataValueListAnnotation<T>
    {
        [JsonProperty("@odata.context"), JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; }
        public IEnumerable<T> value { get; set; }

    }
}
