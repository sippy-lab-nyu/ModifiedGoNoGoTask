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
    const string TitleLabel = "Total Go Trials: {0} Total No-Go Trials {1} \nTotal Rewards: {2}";
    static readonly string[] ResponseLabels = Enum.GetNames(typeof(ResponseId));
    static readonly ResponseId[] ResponseValues = (ResponseId[])Enum.GetValues(typeof(ResponseId));
    GraphControl graph;
    IPointListEdit[] rates;

    public static class ColorMap
    {
        public static readonly Dictionary<ResponseId, Color> Default = new Dictionary<ResponseId, Color>
        {
            { ResponseId.Hit, Color.Green },
            { ResponseId.Miss, Color.Orange },
            { ResponseId.FalseAlarm, Color.IndianRed },
            { ResponseId.CorrectRejection, Color.LemonChiffon }
        };
    }

    public override void Load(IServiceProvider provider)
    {
        var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));

        graph = new GraphControl();
        graph.Dock = DockStyle.Fill;
        var indexAxis = graph.GraphPane.XAxis;
        indexAxis.Title.IsVisible = true;
        indexAxis.Title.Text = "Trial";
        indexAxis.Scale.Format = "F0";
        indexAxis.Type = AxisType.LinearAsOrdinal;
        indexAxis.MinorTic.IsAllTics = false;
        indexAxis.MajorTic.IsInside = false;
        indexAxis.ScaleFormatEvent += (scaleGraph, axis, value, index) =>
        {
            if (scaleGraph.CurveList.Count == 0) return null;
            var series = scaleGraph.CurveList[0];
            return index < series.NPts ? series[index].Tag as string : null;
        };

        rates = new IPointListEdit[ResponseValues.Length];
        for (int i = 0; i < rates.Length; i++)
        {
            rates[i] = new PointPairList();
            if (i % 2 != 0) continue;
            graph.GraphPane.AddCurve(ResponseLabels[i], rates[i], ColorMap.Default[ResponseValues[i]]);
        }

        graph.GraphPane.Title.IsVisible = true;
        graph.GraphPane.Title.Text = string.Format(TitleLabel, 0, 0, 0);
        var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
        if (visualizerService != null)
        {
            visualizerService.AddControl(graph);
        }
    }

    public override void Show(object value)
    {
        var descriptor = (ResponseDescriptor)value;
        rates[(int)ResponseId.Hit].Add(descriptor.Epoch, (float)descriptor.Hits / descriptor.Epoch);
        rates[(int)ResponseId.Miss].Add(descriptor.Epoch, (float)descriptor.Misses / descriptor.Epoch);
        rates[(int)ResponseId.FalseAlarm].Add(descriptor.Epoch, (float)descriptor.FalseAlarms / descriptor.Epoch);
        rates[(int)ResponseId.CorrectRejection].Add(descriptor.Epoch, (float)descriptor.CorrectRejections / descriptor.Epoch);
        graph.GraphPane.Title.Text = string.Format(TitleLabel, 
            descriptor.Hits + descriptor.Misses, 
            descriptor.FalseAlarms + descriptor.CorrectRejections,
            descriptor.Hits);
        graph.Invalidate();
    }

    public override void Unload()
    {
        graph.Dispose();
        graph = null;
    }
}
