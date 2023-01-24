namespace Sewer56.Patcher.Riders.Effect.Utility;

public struct TextTyperEffectHelper
{
    /// <summary>
    /// The text displayed by this helper.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Time before each character is displayed in milliseconds.
    /// </summary>
    public float TimePerCharacterMs { get; private set; }

    private int _lastNumChars;
    private float _timeElapsed;

    /// <summary/>
    /// <param name="text">The text to construct.</param>
    /// <param name="timePerCharacter">Time in ms used before each character is displayed.</param>
    public TextTyperEffectHelper(string text, float timePerCharacter)
    {
        Text = text;
        TimePerCharacterMs = timePerCharacter;
    }

    /// <summary>
    /// Updates the current text helper.
    /// </summary>
    /// <returns>True if new text is available, else false.</returns>
    public bool Update(float deltaTime, out bool reachedEnd, out string subString)
    {
        _timeElapsed += deltaTime;
        var numChars = (int)(_timeElapsed / TimePerCharacterMs);
        subString = default;
        reachedEnd = numChars > Text.Length;
        if (numChars == _lastNumChars || reachedEnd) 
            return false;

        subString = Text.Substring(0, numChars);
        return true;
    }

    /// <summary>
    /// Resets the text helper.
    /// </summary>
    public void Reset()
    {
        _lastNumChars = 0;
        _timeElapsed  = 0;
    }
}