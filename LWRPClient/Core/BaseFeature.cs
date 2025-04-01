using LWRPClient.Features;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LWRPClient.Core
{
    abstract class BaseFeature : ILWRPFeature
    {
        public BaseFeature(LWRPConnection connection)
        {
            this.connection = connection;
            readyTask = new TaskCompletionSource<bool>();
        }

        protected readonly LWRPConnection connection;
        private readonly TaskCompletionSource<bool> readyTask;

        /// <summary>
        /// Creates a collection of messages to be sent to apply any changes made.
        /// </summary>
        /// <returns></returns>
        internal abstract void Apply(IList<LWRPMessage> updates);

        /// <summary>
        /// Fired when a connection was dropped.
        /// </summary>
        internal virtual void ResetState()
        {
            // Intentionally left blank
        }

        /// <summary>
        /// Sets this feature to be ready and fires the ready task.
        /// </summary>
        protected void MarkAsReady()
        {
            //Fire ready task completion, if we haven't yet
            if (!readyTask.Task.IsCompleted)
                readyTask.SetResult(true);
        }

        /// <summary>
        /// Returns a task that can be awaited until the feature is ready.
        /// </summary>
        /// <returns></returns>
        public Task WaitForReadyAsync()
        {
            return readyTask.Task;
        }

        /// <summary>
        /// Returns a task that can be awaited until the feature is ready with a timeout.
        /// </summary>
        /// <returns></returns>
        public async Task WaitForReadyAsync(TimeSpan timeout)
        {
            Task timeoutTask = Task.Delay(timeout);
            Task readyTask = this.readyTask.Task;
            Task completedTask = await Task.WhenAny(timeoutTask, readyTask);
            if (completedTask != readyTask)
                throw new TimeoutException();
        }
    }
}
