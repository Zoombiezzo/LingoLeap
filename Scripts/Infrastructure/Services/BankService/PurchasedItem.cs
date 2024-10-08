using System;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    [Serializable]
    public class PurchasedItem
    {
        public string Id;
        public int Count = 0;
    }
}