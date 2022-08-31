using System.Text;

namespace Sewer56.Patcher.Riders.Effect.Utility;

public struct TextDoubleTyperHelper
{
    /// <summary>
    /// The character used as placeholder.
    /// </summary>
    public char PlaceholderChar { get; set; }

    /// <summary>
    /// The final text to show.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Time before each character update is displayed in milliseconds.
    /// </summary>
    public float TimePerUpdateMs { get; private set; }

    private int _lastNumChars = -1;
    private float _timeElapsed;
    private StringBuilder _builder = new StringBuilder();

    public TextDoubleTyperHelper(char placeholderChar, string text, float timePerUpdate)
    {
        PlaceholderChar = placeholderChar;
        Text = text;
        TimePerUpdateMs = timePerUpdate;
    }

    /// <summary>
    /// Updates the current text helper.
    /// </summary>
    /// <returns>True if new text is available, else false.</returns>
    public bool Update(float deltaTime, out bool reachedEnd, out string newString)
    {
        _timeElapsed += deltaTime;
        var numChars = (int)(_timeElapsed / TimePerUpdateMs);
        newString = default;
        reachedEnd = numChars > Text.Length / 2;
        if (numChars == _lastNumChars || reachedEnd)
            return false;

        // Build the new string.
        _builder.Clear();

        // Append front
        for (int x = 0; x < numChars; x++)
            _builder.Append(Text[x]);

        var endStart = Text.Length - numChars;
        _builder.Append(PlaceholderChar, endStart - numChars);

        // Append back
        for (int x = endStart; x < Text.Length; x++)
            _builder.Append(Text[x]);

        newString = _builder.ToString();
        return true;
    }

    /// <summary>
    /// Resets the text helper.
    /// </summary>
    public void Reset()
    {
        _lastNumChars = -1;
        _timeElapsed = 0;
    }
}