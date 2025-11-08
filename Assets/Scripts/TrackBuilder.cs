using System.Collections.Generic;
using System.Linq;using System.Xml.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UtilityToolkit.Runtime;

public class TrackBuilder
{
    private readonly SplineContainer _splineContainer;
    private readonly SplineInstantiate _splineInstantiate;

    public TrackBuilder(SplineContainer splineContainer)
    {
        _splineContainer = splineContainer;
        _splineInstantiate = splineContainer.GetComponent<SplineInstantiate>();
    }

    public class SplineConnection
    {
        public Spline Spline;
        public Vector3 Point;
        public bool PrependKnots;
    }

    public SplineConnection Build(Vector3 startingPoint)
    {
        return new SplineConnection
        {
            Spline = _splineContainer.AddSpline(),
            Point = startingPoint,
            PrependKnots = false
        };
    }

    public Option<SplineConnection> GetSplineConnection(float3 queryPoint, float maxDistance)
    {
        var splines = _splineContainer.Splines.Where(spline => spline.Count != 0);
        foreach (Spline spline in splines)
        {
            if (math.distancesq(spline[0].Position, queryPoint) < maxDistance)
            {
                return Option<SplineConnection>.Some(new SplineConnection
                {
                    Spline = spline,
                    Point = spline[0].Position,
                    PrependKnots = true
                });
            }

            if (math.distancesq(spline[^1].Position, queryPoint) < maxDistance)
            {
                return Option<SplineConnection>.Some(new SplineConnection
                {
                    Spline = spline,
                    Point = spline[^1].Position,
                    PrependKnots = false
                });
            }
        }
        
        return Option<SplineConnection>.None;
    }

    public void BuildTrack(IEnumerable<Vector3> points)
    {
        Spline spline = _splineContainer.AddSpline();
        spline.Closed = false;
        foreach (var point in points)
        {
            spline.Add(point);
        }

        _splineInstantiate.UpdateInstances();
    }

    public void InsertRange(Spline spline, IEnumerable<Vector3> points, bool prepend)
    {
        var index = prepend ? 0 : spline.Count;
        spline.InsertRange(index, points.Select(point => (float3)point));
        _splineInstantiate.UpdateInstances();
    }
}

public static class Extensions
{
    public static void SetLastPoint(this Spline spline, float3 position)
    {
        BezierKnot old = spline[^1];
        BezierKnot replacement = new(position, old.TangentIn, old.TangentOut, old.Rotation);
        spline[^1] = replacement;
    }
}