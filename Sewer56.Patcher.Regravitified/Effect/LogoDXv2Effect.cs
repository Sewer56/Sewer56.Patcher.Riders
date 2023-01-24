using Reloaded.WPF.Theme.Default;
using Sewer56.Patcher.Riders.Effect.TitleEffectStates;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Sewer56.Patcher.Riders.Effect.Utility;
using Sewer56.Patcher.Riders.Utility;

namespace Sewer56.Patcher.Riders.Effect;

public class LogoDXv2Effect
{
    internal readonly ReloadedWindow Window;
    internal readonly WindowViewModel WindowVm;
    internal readonly Image Image;
    
    private readonly Stopwatch _watch = Stopwatch.StartNew();

    private float _timeElapsed;
    private float _animationDuration = 3000;
    private UpdateThrottler _updateThrottler = new UpdateThrottler(120);

    public LogoDXv2Effect(MainWindow window, Image image)
    {
        Window = window;
        WindowVm = Window.ViewModel;
        Image = image;

        // Note: This might break one day with theme changes.
        CompositionTargetEx.FrameUpdating += OnMainWindowRendering;
    }

    private void OnMainWindowRendering(object sender, EventArgs e)
    {
        var deltaTime = (float)_watch.Elapsed.TotalMilliseconds;
        Update(deltaTime);
        _watch.Restart();
    }

    private void Update(float deltaTime)
    {
        if (!_updateThrottler.Update(deltaTime)) 
            return;

        deltaTime = _updateThrottler.TargetFrameTime;
        _timeElapsed += deltaTime;
        if (_timeElapsed > _animationDuration)
            _timeElapsed -= _animationDuration;

        var ratio = (_timeElapsed / _animationDuration) * (Math.PI * 2);
        const float maxOffsetY = 10;
        const float constantOffsetY = 5;
        var rotation = new TranslateTransform(0, (maxOffsetY * Math.Sin(ratio)) - constantOffsetY);
        Image.RenderTransform = rotation;
    }
}