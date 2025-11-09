using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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

        public void Add(float3 point)
        {
            if (PrependKnots) Spline.Insert(0, point);
            else Spline.Add(point);
        }
    }

    public SplineConnection New(Vector3 startingPoint)
    {
        var spline = _splineContainer.AddSpline();
        spline.Add(startingPoint);

        return new SplineConnection
        {
            Spline = spline,
            Point = startingPoint,
            PrependKnots = false
        };
    }

    public Option<SplineConnection> GetNearestSplineConnection(float3 queryPoint, float maxDistance)
    {
        var splines = _splineContainer.Splines.Where(spline => spline.Count != 0);
        foreach (Spline spline in splines)
        {// distance to query point may not be 0
            var distanceToFirst = math.distancesq(spline[0].Position, queryPoint);
            var distanceToLast = math.distancesq(spline[^1].Position, queryPoint);
            
            if (distanceToFirst < maxDistance && distanceToFirst != 0f)
            {
                return Option<SplineConnection>.Some(new SplineConnection
                {
                    Spline = spline,
                    Point = spline[0].Position,
                    PrependKnots = true
                });
            }

            if (distanceToLast < maxDistance && distanceToLast != 0f)
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

    public void Join(Spline spline1, Spline spline2)
    {
    }
}

public static class Extensions
{
    public static void SetLastPoint(this TrackBuilder.SplineConnection splineConnection, float3 position)
    {
        if (splineConnection.PrependKnots)
        {
            BezierKnot old = splineConnection.Spline[0];
            BezierKnot replacement = new(position, old.TangentIn, old.TangentOut, old.Rotation);
            splineConnection.Spline[0] = replacement;
        }
        else
        {
            BezierKnot old = splineConnection.Spline[^1];
            BezierKnot replacement = new(position, old.TangentIn, old.TangentOut, old.Rotation);
            splineConnection.Spline[^1] = replacement;
        }
    }
}