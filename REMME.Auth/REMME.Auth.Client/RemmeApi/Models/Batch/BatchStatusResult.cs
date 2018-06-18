using Newtonsoft.Json;
using System;

namespace REMME.Auth.Client.RemmeApi.Models.Batch
{
    public class BatchStatusResult
    {
        public BatchStatusEnum Status { get => GetStatusFromString(); }

        [JsonProperty("status")]
        public string StatusString { get; set; }

        [JsonProperty("batch_id")]
        public string BatchId { get; set; }

        private BatchStatusEnum GetStatusFromString()
        {
            var batchStatusEnum = BatchStatusEnum.NO_RESOURCE;
            Enum.TryParse<BatchStatusEnum>(StatusString, out batchStatusEnum);

            return batchStatusEnum;
        }
    }

    public enum BatchStatusEnum
    {
        NO_RESOURCE,
        COMMITTED,
        PENDING
    }
}
