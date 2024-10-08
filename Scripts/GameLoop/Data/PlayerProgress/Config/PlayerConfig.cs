using UnityEngine;

namespace _Client.Scripts.GameLoop.Data.PlayerProgress.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig", order = 0)]
    public class PlayerConfig : ScriptableObject, IPlayerConfig
    {
        [SerializeField] private int _soft;
        [SerializeField] private int _boosterSelectChar;
        [SerializeField] private int _boosterSelectWord;
        public int Soft => _soft;
        public int BoosterSelectChar => _boosterSelectChar;
        public int BoosterSelectWord => _boosterSelectWord;
    }
}