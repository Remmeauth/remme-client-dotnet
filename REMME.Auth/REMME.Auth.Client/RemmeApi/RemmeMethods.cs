using System.Collections.Generic;

namespace REMME.Auth.Client.RemmeApi
{
    public static class RemmeMetodsConventer
    {
        public static Dictionary<RemmeMethodsEnum, string> MethodsMapping =
            new Dictionary<RemmeMethodsEnum, string>
            {
                {RemmeMethodsEnum.GetPublicKeyInfo, "get_public_key_info" },
                {RemmeMethodsEnum.GetBalance, "get_balance" },
                {RemmeMethodsEnum.GetBatchStatus, "get_batch_status" },
                {RemmeMethodsEnum.GetAtomicSwapInfo, "get_atomic_swap_info" },
                {RemmeMethodsEnum.GetAtomicSwapPublicKey, "get_atomic_swap_public_key" },
                {RemmeMethodsEnum.GetAccountPublicKeysList, "get_public_keys_list" },
                {RemmeMethodsEnum.GetNodePublicKey, "get_node_public_key" },
                {RemmeMethodsEnum.ExportNodePrivateKey, "export_node_key" },
                {RemmeMethodsEnum.SendRawTransaction, "send_raw_transaction" },

                {RemmeMethodsEnum.GetNodeInfo, "get_node_info" },
                {RemmeMethodsEnum.GetBlockInfo, "get_blocks" },
                {RemmeMethodsEnum.GetBlocksList, "list_blocks" },
                {RemmeMethodsEnum.FetchBlock, "fetch_block" },
                {RemmeMethodsEnum.GetBatchList, "list_batches" },
                {RemmeMethodsEnum.FetchBatch, "fetch_batch" },
                {RemmeMethodsEnum.GetTransactionsList, "list_transactions" },
                {RemmeMethodsEnum.FetchTransaction, "fetch_transaction" },
                {RemmeMethodsEnum.GetStateList, "list_state" },
                {RemmeMethodsEnum.FetchState, "fetch_state" },
                {RemmeMethodsEnum.FetchPeers, "fetch_peers" },
                {RemmeMethodsEnum.GetReceiptsList, "list_receipts" }
            };

        public static string GetMethodNameString(RemmeMethodsEnum methodType)
        {
            return MethodsMapping[methodType];
        }
    }

    public enum RemmeMethodsEnum
    {
        GetPublicKeyInfo,
        GetBalance,
        GetBatchStatus,
        GetAtomicSwapInfo,
        GetAtomicSwapPublicKey,
        GetAccountPublicKeysList,
        GetNodePublicKey,
        ExportNodePrivateKey,
        SendRawTransaction,

        GetNodeInfo,
        GetBlockInfo,
        GetBlocksList,
        FetchBlock,
        GetBatchList,
        FetchBatch,
        GetTransactionsList,
        FetchTransaction,
        GetStateList,
        FetchState,
        FetchPeers,
        GetReceiptsList
    }
}
