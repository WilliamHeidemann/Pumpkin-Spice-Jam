using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrackBuilder
{
    private readonly SplineContainer _splineContainer;

    public TrackBuilder(SplineContainer splineContainer)
    {
        _splineContainer = splineContainer;
    }
    
    public void BuildTrack(IEnumerable<Vector3> points)
    {
        var splintContainer = Object.Instantiate(_splineContainer);
        Spline spline = splintContainer.Spline;
        foreach (var point in points)
        {
            spline.Add(point);
        }
    }

}
