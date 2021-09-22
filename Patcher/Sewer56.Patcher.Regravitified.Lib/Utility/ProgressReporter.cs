namespace Sewer56.Patcher.Regravitified.Lib.Utility
{
    public struct ProgressReporter
    {
        /// <summary>
        /// The current progress counter.
        /// </summary>
        public int Counter;

        /// <summary>
        /// Maximum value of the progress counter.
        /// </summary>
        public int MaxCounter;

        /// <summary>
        /// The method to call.
        /// </summary>
        public Events.ProgressCallback Callback;

        public ProgressReporter(int maxSteps, int counter = 0, Events.ProgressCallback callback = null)
        {
            Counter = counter;
            MaxCounter = maxSteps;
            Callback = callback;
        }

        /// <summary>
        /// Reports progress to the method and increments the counter by 1.
        /// </summary>
        /// <param name="text">Text to attach to the progress report.</param>
        public void Report(string text)
        {
            var progress = (double)Counter / MaxCounter;
            Callback?.Invoke(text, progress);
            Counter++;
        }
    }
}