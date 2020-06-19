using Bonsai;
using Bonsai.Design;
using Bonsai.Design.Visualizers;
using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

[assembly: TypeVisualizer(typeof(ResponseVisualizer), Target = typeof(ResponseDescriptor))]

public class ResponseVisualizer : DialogTypeVisualizer
{
    static readonly string[] ResponseLabels = Enum.GetNames(typeof(ResponseId));
    static readonly ResponseId[] ResponseValues = (ResponseId[])Enum.GetValues(typeof(ResponseId));
    GraphControl graph;

    public override void Load(IServiceProvider provider)
    {
        var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));

        graph = new GraphControl();
        graph.Dock = DockStyle.Fill;
        var indexAxis = graph.GraphPane.XAxis;
        indexAxis.Title.IsVisible = true;
        indexAxis.Title.Text = "Trial";
        indexAxis.Type = AxisType.Text;
        indexAxis.Scale.IsReverse = true;
        indexAxis.MinorTic.IsAllTics = false;
        indexAxis.MajorTic.IsInside = false;
        indexAxis.ScaleFormatEvent += (scaleGraph, axis, value, index) =>
        {
            if (scaleGraph.CurveList.Count == 0) return null;
            var series = scaleGraph.CurveList[0];
            return index < series.NPts ? series[index].Tag as string : null;
        };

        var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
        if (visualizerService != null)
        {
            visualizerService.AddControl(graph);
        }
    }

    public override void Show(object value)
    {
        var descriptor = (ResponseDescriptor)value;
        graph.Invalidate();
    }

    public override void Unload()
    {
        graph.Dispose();
        graph = null;
    }
}
