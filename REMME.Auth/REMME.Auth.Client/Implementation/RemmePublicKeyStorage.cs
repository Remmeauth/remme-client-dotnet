using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using REMME.Auth.Client.Contracts;
using REMME.Auth.Client.Contracts.Models;
using REMME.Auth.Client.Contracts.Models.PublicKeyStore;
using REMME.Auth.Client.Implementation.Utils;
using REMME.Auth.Client.RemmeApi;
using REMME.Auth.Client.RemmeApi.Models;
using REMME.Auth.Client.RemmeApi.Models.PublicKeys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace REMME.Auth.Client.Implementation
{
    public class RemmePublicKeyStorage : IRemmePublicKeyStorage
    {
        private const string FAMILY_NAME = "pub_key";
        private const string FAMILY_VERSION = "0.1";
        private const string HASH_ALGORITHM = "SHA512";

        private readonly RemmeApi.IRemmeApi _remmeRest;
        private readonly IRemmeTransactionService _remmeTransactionService;

        public RemmePublicKeyStorage(RemmeApi.IRemmeApi remmeRest, IRemmeTransactionService remmeTransactionService)
        {
            _remmeRest = remmeRest;
            _remmeTransactionService = remmeTransactionService;
        }

        public async Task<BaseTransactionResponse> Store(PublicKeyStoreDto publicKeyDto)
        {
            var storePayload = GenarateStorePayload(publicKeyDto);
            var remmeTransaction = _remmeTransactionService.GetTransactionPayload(storePayload, 0);

            var pubKeyAddress = REMChainUtils.GetAddressFromData(publicKeyDto.KeyPair.GetPublicKeyPem(), FAMILY_NAME);
            var inputsOutputs = _remmeTransactionService.GetDataInputOutput(pubKeyAddress);
            inputsOutputs.Add(REMChainUtils.GetSettingsAddressFromData("remme.economy_enabled"));
            inputsOutputs.Add(REMChainUtils.GetSettingsAddressFromData("remme.settings.storage_pub_key"));

            var transactionDto = _remmeTransactionService.GenerateTransactionDto(
                                                                remmeTransaction,
                                                                inputsOutputs,
                                                                FAMILY_NAME,
                                                                FAMILY_VERSION);

            var resultTrans = await _remmeTransactionService.CreateTransaction(transactionDto);

            return await _remmeTransactionService.SendTransaction(resultTrans);
        }

        public async Task<PublicKeyCheckResult> Check(string pemPublicKey)
        {
            var address = REMChainUtils.GetAddressFromData(pemPublicKey, FAMILY_NAME);
            return await CheckByAddress(address);
        }

        public async Task<PublicKeyCheckResult> CheckByAddress(string publicKeyAddress)
        {
            var payload = new PublicKeyCheckPayload { PublicKeyAddress = publicKeyAddress };

            return await _remmeRest
                .SendRequest<PublicKeyCheckPayload, PublicKeyCheckResult>(
                            RemmeMethodsEnum.GetPublicKeyInfo,
                            payload);
        }

        public async Task<PublicKeyCheckResult> Check(byte[] encodedPublicKey)
        {
            var pemPublicKey = string.Format(RSAExtensions.PUBLIC_KEY_PEM_FORMAT, Convert.ToBase64String(encodedPublicKey));
            return await Check(encodedPublicKey);
        }

        public async Task<BaseTransactionResponse> RevokeByAddress(string publicKeyAddress)
        {
            var revokeProto = new RevokePubKeyPayload
            {
                Address = publicKeyAddress
            };
            var remmeTransaction = _remmeTransactionService.GetTransactionPayload(revokeProto, 1);

            var inputsOutputs = _remmeTransactionService.GetDataInputOutput(revokeProto.Address);
            inputsOutputs.Add(REMChainUtils.GetSettingsAddressFromData("remme.economy_enabled"));

            var transactionDto = _remmeTransactionService.GenerateTransactionDto(
                                                                remmeTransaction,
                                                                inputsOutputs,
                                                                FAMILY_NAME,
                                                                FAMILY_VERSION);

            var resultTrans = await _remmeTransactionService.CreateTransaction(transactionDto);

            return await _remmeTransactionService.SendTransaction(resultTrans);
        }

        public async Task<BaseTransactionResponse> Revoke(string pemPublicKey)
        {
            var publicKeyAddress = REMChainUtils.GetAddressFromData(pemPublicKey, FAMILY_NAME);
            return await RevokeByAddress(publicKeyAddress);
        }

        public async Task<BaseTransactionResponse> Revoke(byte[] encodedPublicKey)
        {
            var pemPublicKey = string.Format(RSAExtensions.PUBLIC_KEY_PEM_FORMAT, Convert.ToBase64String(encodedPublicKey));
            return await Revoke(pemPublicKey);
        }

        public async Task<IEnumerable<string>> GetAccountStoredPublicKeys(string accountPublicKey)
        {
            return await _remmeRest
                .SendRequest<GetAccountPublicKeysRequest, IEnumerable<string>>
                (RemmeMethodsEnum.GetAccountPublicKeysList,
                 new GetAccountPublicKeysRequest { PublicKey = accountPublicKey });
        }

        public AsymmetricCipherKeyPair GenerateRsaKeyPair(int rsaKeySize = 2048)
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            return GenerateRsaKeyPair(random, rsaKeySize);
        }

        public AsymmetricCipherKeyPair GenerateRsaKeyPair(SecureRandom random, int rsaKeySize)
        {
            if (rsaKeySize % 8 != 0)
                throw new ArgumentException("Key size must be divisible by 8");

            var keyGenerationParameters = new KeyGenerationParameters(random, rsaKeySize);
            var rsaKeyPairGnr = new RsaKeyPairGenerator();
            rsaKeyPairGnr.Init(keyGenerationParameters);

            var validKeyPair = false;
            AsymmetricCipherKeyPair keyPair;
            do
            {
                keyPair = rsaKeyPairGnr.GenerateKeyPair();
                validKeyPair = keyPair.IsValidKeyParams(rsaKeySize);
            }
            while (!validKeyPair);

            return keyPair;
        }

        #region Private Helpers

        private NewPubKeyPayload GenarateStorePayload(PublicKeyStoreDto publicKeyDto)
        {
            var dataHashHex = publicKeyDto.EntityData.Sha512Digest().BytesToHexString();
            byte[] dataHashBytes = Encoding.UTF8.GetBytes(dataHashHex);
            var hashSignature = publicKeyDto.KeyPair.GetCryptoServiceProvider().SignData(dataHashBytes, HASH_ALGORITHM);

            return new NewPubKeyPayload
            {
                EntityType = NewPubKeyPayload.Types.EntityType.Personal,
                PublicKeyType = NewPubKeyPayload.Types.PubKeyType.Rsa,
                EntityHash = dataHashBytes.BytesToHexString(),
                EntityHashSignature = hashSignature.BytesToHexString(),
                PublicKey = publicKeyDto.KeyPair.GetPublicKeyPem(),
                ValidFrom = publicKeyDto.ValidityFrom,
                ValidTo = publicKeyDto.ValidityTo
            };
        }        

        #endregion
    }
}
