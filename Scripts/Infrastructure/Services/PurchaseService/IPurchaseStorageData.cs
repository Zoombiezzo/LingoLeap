using System.Collections.Generic;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.PurchaseService
{
    public interface IPurchaseStorageData : IStorage
    {
        IReadOnlyList<PurchaseProduct> PendingProducts { get; }
    }
}