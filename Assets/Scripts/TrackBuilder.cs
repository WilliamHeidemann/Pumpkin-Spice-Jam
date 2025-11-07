using System.Collections.Generic;
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
        spline.Closed = true;
        foreach (var point in points)
        {
            spline.Add(point);
        }
        _splineInstantiate.UpdateInstances();
    }

}
