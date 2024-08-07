//using System;
//using System.Threading.Tasks;
//using Nethereum.Web3;
//using Nethereum.ABI.FunctionEncoding.Attributes;
//using Nethereum.Contracts;
//using Nethereum.Hex.HexTypes;
//using Nethereum.Util;

//public class TokenBalanceChecker
//{
//    private readonly string _infuraUrl = "https://mainnet.infura.io/v3/YOUR_INFURA_PROJECT_ID"; // Example Infura URL

//    // Example ERC20 token contract address
//    private readonly string _tokenContractAddress = "0x1234567890abcdef1234567890abcdef12345678";

//    public async Task<decimal> GetTokenBalanceAsync(string userAddress)
//    {
//        var web3 = new Web3(_infuraUrl);

//        // Example ERC20 ABI (used to call standard ERC20 functions)
//        var contract = web3.Eth.GetContract(EthereumExamples.ERC20ABI, _tokenContractAddress);

//        // Get balance function
//        var balanceOfFunction = contract.GetFunction("balanceOf");

//        // Call balanceOf function to get balance
//        var balance = await balanceOfFunction.CallAsync<BigInteger>(userAddress);

//        // Convert balance from BigInteger to decimal
//        decimal balanceDecimal = UnitConversion.Convert.FromWei(balance);

//        return balanceDecimal;
//    }
//}
