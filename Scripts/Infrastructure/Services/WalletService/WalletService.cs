using System;
using _Client.Scripts.GameLoop.Data.PlayerProgress;
using _Client.Scripts.Infrastructure.Services.PurchaseService;
using _Client.Scripts.Infrastructure.Services.SaveService;

namespace _Client.Scripts.Infrastructure.Services.WalletService
{
    public class WalletService : IWalletService
    {
        private readonly IStorageService _storage;
        private readonly IPlayerProgressData _playerProgressData;

        public WalletService(
            IStorageService storage, IPlayerProgressData playerProgressData)
        {
            _storage = storage;
            _playerProgressData = playerProgressData;
        }

        public bool TryAddCurrency(CurrencyType currencyType, int count, out int diff)
        {
            switch (currencyType)
            {
                case CurrencyType.Soft:
                    _playerProgressData.ChangeSoft(count);
                    break;
                case CurrencyType.BoosterSelectChar:
                    _playerProgressData.ChangeBoosterSelectChar(count);
                    break;
                case CurrencyType.BoosterSelectWord:
                    _playerProgressData.ChangeBoosterSelectWord(count);
                    break;
            }
            
            diff = 0;
            return true;
        }

        public bool TryRemoveCurrency(CurrencyType currencyType, int count, out int diff)
        {
            switch (currencyType)
            {
                case CurrencyType.Soft:
                    if (_playerProgressData.Soft.CurrentValue < count)
                    {
                        diff = _playerProgressData.Soft.CurrentValue - count;
                        return false;
                    }
                    _playerProgressData.ChangeSoft(-count);
                    break;
                case CurrencyType.BoosterSelectChar:
                    if (_playerProgressData.BoosterSelectChar.CurrentValue < count)
                    {
                        diff = _playerProgressData.BoosterSelectChar.CurrentValue - count;
                        return false;
                    }
                    _playerProgressData.ChangeBoosterSelectChar(-count);
                    break;
                case CurrencyType.BoosterSelectWord:
                    if (_playerProgressData.BoosterSelectWord.CurrentValue < count)
                    {
                        diff = _playerProgressData.BoosterSelectWord.CurrentValue - count;
                        return false;
                    }
                    _playerProgressData.ChangeBoosterSelectWord(-count);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currencyType), currencyType, null);
            }
            
            diff = 0;
            return true;
        }

        public bool IsCurrencyEnough(CurrencyType currencyType, int count, out int diff)
        {
            switch (currencyType)
            {
                case CurrencyType.Soft:
                    diff = _playerProgressData.Soft.CurrentValue - count;
                    return diff >= 0;
                case CurrencyType.BoosterSelectChar:
                    diff = _playerProgressData.BoosterSelectChar.CurrentValue - count;
                    return diff >= 0;
                case CurrencyType.BoosterSelectWord:
                    diff = _playerProgressData.BoosterSelectWord.CurrentValue - count;
                    return diff >= 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currencyType), currencyType, null);
            }
        }
    }
}
