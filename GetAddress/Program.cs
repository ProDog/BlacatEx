using System;
using Nethereum.Hex.HexConvertors.Extensions;

namespace GetAddress
{
    class Program
    {
        static void Main(string[] args)
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            var account = new Nethereum.Web3.Accounts.Account(privateKey);

            
        }
    }
}
