using Bonsai;
using Bonsai.Design;
using Bonsai.Design.Visualizers;
using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

[assembly: TypeVisualizer(typeof(TwoAfcVisualizer), Target = typeof(TwoAfcDescriptor))]

public class TwoAfcVisualizer : DialogTypeVisualizer
{
    const string TitleLabel = "Total Hits: {0} Left Hits {1} \nRight Hits: {2}";
    static readonly string[] ResponseLabels = Enum.GetNames(typeof(ResponseId)).Skip(1).ToArray();
    static readonly ResponseId[] ResponseValues = ((ResponseId[])Enum.GetValues(typeof(ResponseId))).Skip(1).ToArray();
    GraphControl graph;
    IPointListEdit[] rates;

    public static class ColorMap
    {
        public static readonly Dictionary<ResponseId, Color> Default = new Dictionary<ResponseId, Color>
        {
            { ResponseId.None, Color.Transparent },
            { ResponseId.LeftHit, Color.Green },
            { ResponseId.RightHit, Color.Orange },
            { ResponseId.FalseAlarm, Color.IndianRed },
            { ResponseId.CorrectRejection, Color.LemonChiffon },
            { ResponseId.PullPenalty, Color.Transparent },
            { ResponseId.IncorrectAction, Color.Transparent },
            { ResponseId.EarlyResponse, Color.Transparent },
            { ResponseId.EarlyExit, Color.Transparent },
            { ResponseId.Hit, Color.Yellow },
            { ResponseId.Miss, Color.Green },
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
        var descriptor = (TwoAfcDescriptor)value;
        //rates[(int)ResponseId.Hit-1].Add(descriptor.Epoch, (float)descriptor.Hits / (descriptor.Hits + descriptor.Misses));
        // rates[(int)ResponseId.Hit-1].Add(descriptor.Epoch, (float)descriptor.Hits / (descriptor.TotalGoTrials));
        // rates[(int)ResponseId.Miss-1].Add(descriptor.Epoch, (float)descriptor.Misses / descriptor.Epoch);
        // rates[(int)ResponseId.FalseAlarm-1].Add(descriptor.Epoch, (float)descriptor.FalseAlarms / (descriptor.FalseAlarms + descriptor.CorrectRejections));
        // rates[(int)ResponseId.CorrectRejection-1].Add(descriptor.Epoch, (float)descriptor.CorrectRejections / descriptor.Epoch);
        graph.GraphPane.Title.Text = string.Format(TitleLabel, 
            descriptor.TotalHits,
            descriptor.LeftHits,
            descriptor.RightHits);
        graph.Invalidate();
    }

    public override void Unload()
    {
        graph.Dispose();
        graph = null;
    }
}
