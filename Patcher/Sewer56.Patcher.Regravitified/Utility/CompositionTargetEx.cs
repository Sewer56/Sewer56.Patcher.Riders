using System;
using System.Windows.Media;

namespace Sewer56.Patcher.Riders.Utility;

public static class CompositionTargetEx
{
    private static TimeSpan _last = TimeSpan.Zero; 
    private static event EventHandler<RenderingEventArgs> _frameUpdating; 
    
    public static event EventHandler<RenderingEventArgs> FrameUpdating
    {
        add
        {
            if (_frameUpdating == null) 
                CompositionTarget.Rendering += CompositionTarget_Rendering; 

            _frameUpdating += value;
        }
        remove
        {
            _frameUpdating -= value; 
            if (_frameUpdating == null) 
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }
    }

    static void CompositionTarget_Rendering(object sender, EventArgs e)
    {
        RenderingEventArgs args = (RenderingEventArgs)e; 
        if (args.RenderingTime == _last) 
            return; 

        _last = args.RenderingTime; 
        _frameUpdating?.Invoke(sender, args);
    }
}