using System.Collections.Generic;

namespace _Client.Scripts.Infrastructure.Services.BankService
{
    public interface IBankCategory
    {
        string Name { get; }
        string Id { get; }
        List<IBankCategory> Categories { get; }
        List<IBankItem> Items { get; }
        ICategoryView View { get; }
    }
}