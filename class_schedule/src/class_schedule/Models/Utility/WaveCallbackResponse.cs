namespace WaveUtility
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class WaveCallbackResponse
    {
        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("merchantId")]
        public string merchantId { get; set; }

        [JsonProperty("orderId")]
        public string? orderId { get; set; }

        [JsonProperty("merchantReferenceId")]
        public string merchantReferenceId { get; set; }

        [JsonProperty("frontendResultUrl")]
        public string frontendResultUrl { get; set; }

        [JsonProperty("backendResultUrl")]
        public string backendResultUrl { get; set; }

        [JsonProperty("initiatorMsisdn")]
        public string? initiatorMsisdn { get; set; }

        [JsonProperty("amount")]
        public int amount { get; set; }

        [JsonProperty("timeToLiveSeconds")]
        public int timeToLiveSeconds { get; set; }

        [JsonProperty("paymentDescription")]
        public string? paymentDescription { get; set; }

        [JsonProperty("currency")]
        public string? currency { get; set; }

        [JsonProperty("hashValue")]
        public string hashValue { get; set; }

        [JsonProperty("additionalField1")]
        public string? additionalField1 { get; set; }

        [JsonProperty("additionalField2")]
        public string? additionalField2 { get; set; }

        [JsonProperty("additionalField3")]
        public string? additionalField3 { get; set; }

        [JsonProperty("additionalField4")]
        public string? additionalField4 { get; set; }

        [JsonProperty("additionalField5")]
        public string? additionalField5 { get; set; }

        [JsonProperty("transactionId")]
        public string transactionId { get; set; }

        [JsonProperty("paymentRequestId")]
        public int paymentRequestId { get; set; }

        [JsonProperty("requestTime")]
        public string requestTime { get; set; }

        [JsonIgnore]
        public List<AdditionalField1Item>? additionalField1Parsed =>
            string.IsNullOrWhiteSpace(additionalField1)
                ? null
                : JsonConvert.DeserializeObject<List<AdditionalField1Item>>(additionalField1);
    }

    public class AdditionalField1Item
    {
        [JsonProperty("name")]
        public string? name { get; set; }

        [JsonProperty("amount")]
        public int amount { get; set; }
    }

    public partial class WaveCallbackResponse
    {
        public static WaveCallbackResponse FromJson(string json) =>
            JsonConvert.DeserializeObject<WaveCallbackResponse>(json, WaveUtility.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this WaveCallbackResponse self) =>
            JsonConvert.SerializeObject(self, WaveUtility.Converter.Settings);
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
