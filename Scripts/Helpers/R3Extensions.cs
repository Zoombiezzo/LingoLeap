using System;
using System.Threading;
using R3;

namespace _Client.Scripts.Helpers
{
    public static class R3Extensions
    {
        public static Observable<T> AsObservable<T>(this Action<T> target, CancellationToken cancellationToken = default) => 
            Observable.FromEvent<T>(h => target += h, h => target -= h, cancellationToken);
    }
}