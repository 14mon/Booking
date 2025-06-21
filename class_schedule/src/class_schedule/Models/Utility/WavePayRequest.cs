namespace WavePayRequestModel
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    public partial class WavePayRequest
    {

        [JsonProperty("userId")]
        public Guid userId { get; set; }

        [JsonProperty("planId")]
        public string planId { get; set; }

    }

    public partial class WavePayRequest
    {
        public static WavePayRequest FromJson(string json) => JsonConvert.DeserializeObject<WavePayRequest>(json, WavePayRequestModel.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this WavePayRequest self) => JsonConvert.SerializeObject(self, WavePayRequestModel.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}