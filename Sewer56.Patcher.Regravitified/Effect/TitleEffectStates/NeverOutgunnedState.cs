using Sewer56.Patcher.Riders.Effect.Utility;

namespace Sewer56.Patcher.Riders.Effect.TitleEffectStates;

public struct NeverOutgunnedState : ITitleEffectState
{
    private TextDoubleTyperHelper _doubleTyper = new TextDoubleTyperHelper('.', "New Features. Same Style.", 200);
    private DelayHelper _nextStateDelayer = new DelayHelper(6000);

    public NeverOutgunnedState() { }

    public bool Update(TitleDXv2Effect effect, float deltaTime)
    {
        if (_doubleTyper.Update(deltaTime, out bool end, out var newText))
            effect.TitleBlock1.Text = newText;

        return _nextStateDelayer.Update(deltaTime);
    }

    public bool Init(TitleDXv2Effect effect)
    {
        _doubleTyper.Reset();
        _nextStateDelayer.Reset();
        return true;
    }

    public bool Exit(TitleDXv2Effect effect) => true;
}