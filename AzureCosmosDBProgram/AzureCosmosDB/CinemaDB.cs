using Newtonsoft.Json;

namespace CosmosGettingStartedTutorial
{
    public class CinemaDB
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }
        public string cinemaName { get; set; }
        public Movie[] Movies { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Movie
    {
        public string movieName { get; set; }
        public int price { get; set; }
    }
}
