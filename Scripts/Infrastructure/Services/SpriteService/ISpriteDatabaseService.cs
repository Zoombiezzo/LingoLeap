using _Client.Scripts.Infrastructure.Services.ConfigData;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using UnityEngine;

namespace _Client.Scripts.Infrastructure.Services.SpriteService
{
    public interface ISpriteDatabaseService : IConfigData
    { 
        Sprite GetSprite(string id);
        Sprite GetCurrencySprite(CurrencyType currencyType);
    }
}