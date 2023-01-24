using System.Windows.Xps.Serialization;

namespace Sewer56.Patcher.Riders.Effect.Utility;

public struct BlinkEffectHelper
{
    /// <summary>
    /// The text displayed by this helper.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Time the blink is set in Enabled/On state.
    /// </summary>
    public float TimeOn { get; private set; }

    /// <summary>
    /// Time the blink is set in Disabled/Off state.
    /// </summary>
    public float TimeOff { get; private set; }

    /// <summary>
    /// The Initial Assumed state. False means item is considered as starting non-visible.
    /// </summary>
    public bool InitialState { get; private set; }

    private float _timeElapsed;
    private bool _displayedOnLastBlink = false;

    /// <summary/>
    /// <param name="text">The text to construct.</param>
    /// <param name="timeOn">Time in ms used between each blink.</param>
    /// <param name="timeOff">Time in ms the blink is spent in disabled state.</param>
    /// <param name="initialState">The initial assumed state. False means item is assumed not visible.</param>
    public BlinkEffectHelper(string text, float timeOn, float timeOff, bool initialState = false)
    {
        Text = text;
        TimeOn = timeOn;
        TimeOff = timeOff;
        _displayedOnLastBlink = initialState;
    }

    /// <summary>
    /// Updates the current text helper.
    /// </summary>
    /// <returns>True if new text is available, else false.</returns>
    public bool Update(float deltaTime, out string newText)
    {
        _timeElapsed += deltaTime;
        var maxTime = GetCurrentMaxTime();
        if (_timeElapsed <= maxTime)
        {
            newText = default;
            return false;
        }

        // Update time tracker
        _timeElapsed -= maxTime;

        // Get next state.
        _displayedOnLastBlink = !_displayedOnLastBlink;
        newText = _displayedOnLastBlink ? Text : string.Empty;
        return true;
    }

    /// <summary>
    /// Resets the text helper.
    /// </summary>
    public void Reset()
    {
        _timeElapsed = 0;
        _displayedOnLastBlink = InitialState;
    }

    private float GetCurrentMaxTime() => _displayedOnLastBlink ? TimeOn : TimeOff;
}