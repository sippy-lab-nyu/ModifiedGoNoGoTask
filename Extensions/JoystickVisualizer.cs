using Bonsai;
using Bonsai.Design;
using Bonsai.Design.Visualizers;
using System;
using System.Windows.Forms;

[assembly: TypeVisualizer(typeof(JoystickVisualizer), Target = typeof(LineGraphBuilder))]

public class JoystickVisualizer : LineGraphVisualizer
{
    public override void Load(IServiceProvider provider)
    {
        base.Load(provider);

        var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService)) as Control;
        if (visualizerService != null)
        {
            var graph = visualizerService.Controls[0] as GraphControl;
            if (graph != null)
            {
                graph.GraphPane.XAxis.Title.IsVisible = true;
                graph.GraphPane.YAxis.Title.IsVisible = true;
                graph.GraphPane.XAxis.Title.Text = "Time (sec)";
                graph.GraphPane.YAxis.Title.Text = "Joystick Deflection";
            }
        }
    }
}