using System.Collections.Generic;
using Sewer56.Patcher.Riders.Utility;

namespace Sewer56.Patcher.Riders.Effect.Utility;

public struct RandomFillTextEffectHelper
{
    /// <summary>
    /// The text displayed by this helper.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Time before each character is displayed in milliseconds.
    /// </summary>
    public float TimePerCharacterMs { get; private set; }

    /// <summary>
    /// Placeholder character.
    /// </summary>
    public char Placeholder { get; private set; }

    private int _lastNumChars;
    private float _timeElapsed;
    private char[] _characters = null;
    private List<int> _indicesToFill = new List<int>(); // Could use array for more efficiency, but this is good enough for now.

    /// <summary/>
    /// <param name="text">The text to construct.</param>
    /// <param name="timePerCharacter">Time in ms used before each character is displayed.</param>
    /// <param name="placeholder">Placeholder character.</param>
    public RandomFillTextEffectHelper(string text, float timePerCharacter, char placeholder)
    {
        Text = text;
        TimePerCharacterMs = timePerCharacter;
        Placeholder = placeholder;
        _characters = new char[Text.Length];
    }

    /// <summary>
    /// Updates the current text helper.
    /// </summary>
    /// <returns>True if new text is available, else false.</returns>
    public bool Update(float deltaTime, out bool reachedEnd, out string newString)
    {
        _timeElapsed += deltaTime;
        var numChars = (int)(_timeElapsed / TimePerCharacterMs);
        newString = default;
        reachedEnd = numChars > Text.Length;
        if (numChars == _lastNumChars || reachedEnd || _indicesToFill.Count <= 0)
            return false;

        // Pop last character.
        // We remove last to avoid internal array copy.
        var index = _indicesToFill[^1];
        _indicesToFill.RemoveAt(_indicesToFill.Count - 1);
        _characters[index] = Text[index];
        newString = new string(_characters);
        _lastNumChars = numChars;
        return true;
    }

    /// <summary>
    /// Resets the text helper.
    /// </summary>
    public void Reset()
    {
        _lastNumChars = -1;
        _timeElapsed = 0;

        _indicesToFill.Clear();
        for (int x = 0; x < Text.Length; x++)
        {
            _indicesToFill.Add(x);
            _characters[x] = Placeholder;
        }

        _indicesToFill.Shuffle();
    }
}