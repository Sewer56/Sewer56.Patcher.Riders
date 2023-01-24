namespace Sewer56.Patcher.Riders.Effect.SRDX.TitleEffectStates;

/// <summary>
/// Common interface for individual effects in titles.
/// </summary>
public interface ITitleEffectState
{
    /// <summary>
    /// Updates the visuals of the current state.
    /// </summary>
    /// <param name="effect">The parent effect value.</param>
    /// <param name="deltaTime">Time elapsed since last frame.</param>
    /// <returns>True if the state should advance to the next state.</returns>
    public bool Update(TitleDXv2Effect effect, float deltaTime);

    /// <summary>
    /// Resets the state of the current effect..
    /// </summary>
    /// <returns>Nothing; ignore this value.</returns>
    public bool Init(TitleDXv2Effect effect);

    /// <summary>
    /// Performed when exiting this specific state.
    /// </summary>
    /// <returns>Nothing; ignore this value.</returns>
    public bool Exit(TitleDXv2Effect effect);
}