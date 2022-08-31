using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using Reloaded.WPF.Theme.Default;
using Sewer56.Patcher.Riders.Effect.TitleEffectStates;
using Sewer56.Patcher.Riders.Utility;

namespace Sewer56.Patcher.Riders.Effect;

public class TitleDXv2Effect
{
    internal readonly ReloadedWindow Window;
    internal readonly WindowViewModel WindowVm;

    internal readonly Viewbox TitleContainer1;
    internal readonly Viewbox TitleContainer2;
    internal readonly TextBlock TitleBlock1;
    internal readonly TextBlock TitleBlock2;

    private TitleStates _state = TitleStates.ExGlPresents;
    private TitleStates _numStates = Enum.GetValues(typeof(TitleStates)).Cast<TitleStates>().Max() + 1;
    private readonly Stopwatch _watch = Stopwatch.StartNew();

    private ExglPresentsState _exglPresentsState = new();
    private SonicRiders20State _sonicRiders20State = new();
    private NeverOutgunnedState _neverOutgunnedState = new();
    private NextLevelSince2017State _nextLevelSince2017State = new();
    private DisplayReleaseNotesState _displayReleaseNotesState;

    public TitleDXv2Effect(ReloadedWindow window)
    {
        Window = window;
        WindowVm = Window.ViewModel;
        
        // Note: This might break one day with theme changes.
        TitleContainer1 = (Viewbox) Window.Template.FindName("ViewBoxTitle1", Window);
        TitleBlock1 = (TextBlock)TitleContainer1.Child;

        TitleContainer2 = (Viewbox)Window.Template.FindName("ViewBoxTitle2", Window);
        TitleBlock2 = (TextBlock)TitleContainer2.Child;

        CompositionTargetEx.FrameUpdating += OnMainWindowRendering;
        _displayReleaseNotesState = new DisplayReleaseNotesState(TitleContainer1, TitleContainer2);
    }

    private void OnMainWindowRendering(object sender, EventArgs e)
    {
        var deltaTime = (float)_watch.Elapsed.TotalMilliseconds;
        Update(deltaTime);
        _watch.Restart();
    }

    private void Update(float deltaTime)
    {
        bool nextState = _state switch
        {
            TitleStates.ExGlPresents => _exglPresentsState.Update(this, deltaTime),
            TitleStates.SonicRidersDx20 => _sonicRiders20State.Update(this, deltaTime),
            TitleStates.NeverOutgunned => _neverOutgunnedState.Update(this, deltaTime),
            TitleStates.DisplayReleaseNotes => _displayReleaseNotesState.Update(this, deltaTime),
            TitleStates.NextLevelSince2017 => _nextLevelSince2017State.Update(this, deltaTime),
            _ => throw new ArgumentOutOfRangeException()
        };

        // Advance state if necessary.
        if (nextState)
        {
            ExitState(_state);

            _state += 1;
            if (_state >= _numStates)
                _state -= _numStates;

            InitState(_state);
        }
    }

    /// <summary>
    /// Return value is a dummy, for programming convenience.
    /// </summary>
    private bool ExitState(TitleStates state)
    {
        return state switch
        {
            TitleStates.ExGlPresents => _exglPresentsState.Exit(this),
            TitleStates.SonicRidersDx20 => _sonicRiders20State.Exit(this),
            TitleStates.NeverOutgunned => _neverOutgunnedState.Exit(this),
            TitleStates.DisplayReleaseNotes => _displayReleaseNotesState.Exit(this),
            TitleStates.NextLevelSince2017 => _nextLevelSince2017State.Exit(this),
            _ => true
        };
    }

    /// <summary>
    /// Return value is a dummy, for programming convenience.
    /// </summary>
    private bool InitState(TitleStates state)
    {
        return state switch
        {
            TitleStates.ExGlPresents => _exglPresentsState.Init(this),
            TitleStates.SonicRidersDx20 => _sonicRiders20State.Init(this),
            TitleStates.NeverOutgunned => _neverOutgunnedState.Init(this),
            TitleStates.DisplayReleaseNotes => _displayReleaseNotesState.Init(this),
            TitleStates.NextLevelSince2017 => _nextLevelSince2017State.Init(this),
            _ => true
        };
    }

    private enum TitleStates
    {
        ExGlPresents,
        SonicRidersDx20,
        NextLevelSince2017,
        NeverOutgunned,
        DisplayReleaseNotes
    }
}