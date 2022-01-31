using System;
using System.Text;
using System.Threading;

/*
 * Adapted from https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54
 * With further improvements.
 */
namespace Sewer56.Patcher.Riders.Cli.Cmd
{
    public class ProgressBar : IDisposable
    {
        private const string Animation = @"|/-\";
        
        private readonly TimeSpan _animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private readonly Timer _timer;

        private int _blockCount;
        private double _currentProgress = 0;
        private string _currentText     = string.Empty;
        private bool _disposed          = false;
        private int _animationIndex     = 0;
        private string _extraText;
        private bool _clearTextOnExit;

        public ProgressBar(int blockCount = 32, bool clearTextOnExit = false)
        {
            _blockCount = blockCount;
            _timer = new Timer(TimerHandler);
            _clearTextOnExit = clearTextOnExit;

            // A progress bar is only for temporary display in a console window.
            // If the console output is redirected to a file, draw nothing.
            // Otherwise, we'll end up with a lot of garbage in the target file.
            if (!Console.IsOutputRedirected)
                ResetTimer();
        }

        public void Dispose()
        {
            lock (_timer)
            {
                _disposed = true;
                if (_clearTextOnExit)
                    UpdateText(string.Empty);
                else
                    Console.WriteLine();
            }
        }

        public void Report(double value, string text)
        {
            // Make sure value is in [0..1] range
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref _currentProgress, value);
            _extraText = text;
        }

        private void TimerHandler(object state)
        {
            lock (_timer)
            {
                if (_disposed) return;

                int progressBlockCount = (int) (_currentProgress * _blockCount);
                int percent = (int) (_currentProgress * 100);
                string text = $"[{new string('#', progressBlockCount)}{new string('-', _blockCount - progressBlockCount)}] {percent,3}% {Animation[_animationIndex++ % Animation.Length]} {_extraText}";
                UpdateText(text);

                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(_currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
                commonPrefixLength++;

            // Backtrack to the first differing character
            var outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = _currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            _currentText = text;
        }

        private void ResetTimer() => _timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));
    }
}
