using Newtonsoft.Json;
using System;

namespace REMME.Auth.Client.RemmeApi.Models
{
    public class BatchStateUpdateDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("batch_statuses")]
        public BatchStatus BatchStatuses { get; set; }
    }

    public class BatchStatus
    {
        public BatchStatusEnum Status { get => GetStatusFromString(); }

        [JsonProperty("status")]
        public string StatusString { get; set; }

        [JsonProperty("batch_id")]
        public string BatchId { get; set; }

        private BatchStatusEnum GetStatusFromString()
        {
            var batchStatusEnum = BatchStatusEnum.NOT_CONFIRMED;
            Enum.TryParse<BatchStatusEnum>(StatusString, out batchStatusEnum);

            return batchStatusEnum;
        }
    }

    public enum BatchStatusEnum
    {
        NOT_CONFIRMED,
        OK,
        PENDING
    }
}
