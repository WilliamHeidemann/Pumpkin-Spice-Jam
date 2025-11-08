using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class TrackBuilder
{
    private readonly SplineContainer _splineContainer;
    private readonly SplineInstantiate _splineInstantiate;

    public TrackBuilder(SplineContainer splineContainer)
    {
        _splineContainer = splineContainer;
        _splineInstantiate = splineContainer.GetComponent<SplineInstantiate>();
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
