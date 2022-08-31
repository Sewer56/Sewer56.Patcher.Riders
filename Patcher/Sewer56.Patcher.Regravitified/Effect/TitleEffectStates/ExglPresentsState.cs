using Sewer56.Patcher.Riders.Effect.Utility;

namespace Sewer56.Patcher.Riders.Effect.TitleEffectStates;

public struct ExglPresentsState : ITitleEffectState
{
    private TextTyperEffectHelper _titleTyper = new TextTyperEffectHelper("Extreme Gear Labs Presents", 150);
    private DelayHelper _nextStateDelayer = new DelayHelper(2500);

    public ExglPresentsState() { }

    public bool Update(TitleDXv2Effect effect, float deltaTime)
    {
        if (_titleTyper.Update(deltaTime, out var reachedEnd, out var newTitle))
            effect.TitleBlock1.Text = newTitle;

        // Wait until transitioning to next.
        if (reachedEnd)
        {
            if (_nextStateDelayer.Update(deltaTime))
                return true;
        }
        
        return false;
    }

    public bool Init(TitleDXv2Effect effect)
    {
        _titleTyper.Reset();
        _nextStateDelayer.Reset();
        return true;
    }

    public bool Exit(TitleDXv2Effect effect) => true;
}