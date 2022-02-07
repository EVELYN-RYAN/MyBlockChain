using System;
using System.Collections.Generic;
using System.Linq;
using EllipticCurve;
namespace RootCoin
{
    class Blockchain
    {
        public List<Block> Chain { get; set; }

        public int Difficulty { get; set; }
        List<Transaction> pendingTransactions {get; set;}
        public decimal MiningReward { get; set; }

        public Blockchain(int difficulty,decimal miningReward)
        {
            this.Chain = new List<Block>();
            this.Chain.Add((CreateGenesisBlock()));
            this.Difficulty = difficulty;
            this.MiningReward = miningReward;
            this.pendingTransactions = new List<Transaction>();

        }
        public Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyyMMddhhmmssffff"), new List<Transaction>());
        }
        public Block GetLatestBlock()
        {
            return this.Chain.Last();
        }
        public void AddBlock(Block newBlock)
        {
            newBlock.PreviousHash = this.GetLatestBlock().Hash;
            newBlock.Hash = newBlock.CalculateHash();
            this.Chain.Add(newBlock);
        }
        public void addPendingTransaction(Transaction transaction)
        {
            if(transaction.FromAddress is null || transaction.ToAddress is null)
            {
                throw new Exception("Transaction must include a to and from address");
            }
            if(transaction.Amount > this.GetBalanceOfWallet(transaction.FromAddress))
            {
                throw new Exception("There is insufficent funds");
            }
            if(transaction.IsValid() == false)
            {
                throw new Exception("Cannot add an invalid transaction to a block");
            }
        }
        public decimal GetBalanceOfWallet(PublicKey address)
        {
            decimal balance = 0;

            string addressDER = BitConverter.ToString(address.toDer()).Replace("=", "");

            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if (transaction.FromAddress != null)
                    {

                        string fromDER = BitConverter.ToString(transaction.FromAddress.toDer()).Replace("=", "");

                        if (fromDER == addressDER)
                        {
                            balance -= transaction.Amount;
                        }
                    }
                    string toDER = BitConverter.ToString(transaction.ToAddress.toDer()).Replace("=", "");
                    if (toDER == addressDER)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }
        public void MinePendingTransactions(PublicKey miningRewardWallet)
        {
            Transaction rewardtx = new Transaction(null, miningRewardWallet, MiningReward);
            this.pendingTransactions.Add(rewardtx);

            Block newBlock = new Block(GetLatestBlock().Index + 1, DateTime.Now.ToString("yyyyMMddhhmmssffff"), this.pendingTransactions, GetLatestBlock().Hash);
            newBlock.Mine(this.Difficulty);

            Console.WriteLine("Block succesfully mined!");

            this.Chain.Add(newBlock);
            this.pendingTransactions = new List<Transaction>();
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < this.Chain.Count; i++)
            {
                Block CurrentBlock = this.Chain[i];
                Block PreviousBlock = this.Chain[i - 1];

                //Check if the current bock hash is same as calculated hash
                if (CurrentBlock.Hash != CurrentBlock.CalculateHash())
                {
                    return false;
                }
                if (CurrentBlock.PreviousHash != PreviousBlock.Hash)
                {
                    return false;
                }

            }
            return true;


        }
    }
}

