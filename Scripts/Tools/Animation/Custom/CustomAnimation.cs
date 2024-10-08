using DG.Tweening;

namespace _Client.Scripts.Tools.Animation.Custom
{
    public abstract class CustomAnimation
    {
        public abstract void Initialize();
        public abstract Sequence Create();
        public abstract void Reset();
    }
}