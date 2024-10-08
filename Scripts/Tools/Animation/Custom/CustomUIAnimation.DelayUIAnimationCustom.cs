using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Client.Scripts.Tools.Animation.Custom
{
    public partial class CustomUIAnimation
    {
        [Serializable]
        [HideLabel]
        private class DelayUIAnimationCustom : CustomAnimation
        {
            [SerializeField] private bool _isRandom;
            [SerializeField] [FoldoutGroup("RANDOM")] [HorizontalGroup("RANDOM/VALUE")]
            [ShowIf("_isRandom")]
            private float _delayMin;
            [SerializeField] [FoldoutGroup("RANDOM")] [HorizontalGroup("RANDOM/VALUE")]
            [ShowIf("_isRandom")]
            private float _delayMax;
            [SerializeField] 
            [ShowIf("@_isRandom == false")]
            private float _delay;
            
            public override void Initialize()
            {
            }

            public override Sequence Create()
            {
                var delay = _delay;

                if (_isRandom)
                {
                    delay = Random.Range(_delayMin, _delayMax);
                }

                var sequence = DOTween.Sequence();
                sequence.AppendInterval(delay);
                return sequence;
            }

            public override void Reset()
            {
            }
        }
    }
}