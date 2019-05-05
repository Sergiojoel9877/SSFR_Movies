using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SSFR_Movies.Helpers
{
    /// <summary>
    /// To avoid the use of ConfigureAwait(false); when a call must not to be awaited in the UI Thread.
    /// </summary>
    public struct SynchronizationContextRemover : INotifyCompletion
    {
        public bool IsCompleted
        {
            get => SynchronizationContext.Current == null;
        }

        public void OnCompleted(Action continuation)
        {
            var prevContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                continuation();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevContext);
            }
        }

        public SynchronizationContextRemover GetAwaiter() => this;

        public void GetResult()
        {
        }
    }
}
