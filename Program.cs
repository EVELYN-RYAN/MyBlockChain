using System;
using Newtonsoft.Json;
using EllipticCurve;

namespace RootCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            PrivateKey key1 = new PrivateKey();
            PublicKey wallet1 = key1.publicKey();
            PrivateKey key2 = new PrivateKey();
            PublicKey wallet2 = key2.publicKey();


            Blockchain rootcoin = new Blockchain(2, 100);

            Console.WriteLine("Start The Miner.");
            rootcoin.MinePendingTransactions(wallet1);
            //decimal bal1 = rootcoin.GetBalanceOfWallet(wallet1);
            Console.WriteLine("\nBalance of wallet1 is $" + rootcoin.GetBalanceOfWallet(wallet1).ToString());

            Transaction tx1 = new Transaction(wallet1, wallet2, 10);
            tx1.SignTransaction(key1);
            rootcoin.addPendingTransaction(tx1);

            //Start up again
            Console.WriteLine("Start The Miner.");
            rootcoin.MinePendingTransactions(wallet2);
            Console.WriteLine("\nBalance of wallet1 is $" + rootcoin.GetBalanceOfWallet(wallet1).ToString());
            Console.WriteLine("\nBalance of wallet2 is $" + rootcoin.GetBalanceOfWallet(wallet2).ToString());
            //rootcoin.AddBlock(new Block(1, DateTime.Now.ToString("yyyyMMddhhmmssffff"), "amount: 50"));
            //rootcoin.AddBlock(new Block(2, DateTime.Now.ToString("yyyyMMddhhmmssffff"), "amount: 200"));


            string blockJSON = JsonConvert.SerializeObject(rootcoin,Formatting.Indented);
            Console.WriteLine(blockJSON);

            rootcoin.GetLatestBlock().PreviousHash = "12345";
            if(rootcoin.IsChainValid())
            {
                Console.WriteLine("Blockchain is Valid!");
            }
            else
            {
                Console.WriteLine("Blockchain is NOT Valid!");
            }
        }


    }

}
