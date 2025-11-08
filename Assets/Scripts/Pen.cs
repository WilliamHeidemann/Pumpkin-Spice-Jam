using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Pen : MonoBehaviour
{
    private Camera _mainCamera;
    private List<GameObject> _checkPoints = new();
    [SerializeField] private GameObject _sphere;
    [SerializeField] private LayerMask _layerMask;
    private Vector3 _pointBeforeLastPoint;
    private Vector3 _lastPoint;
    private Spline _spline;
    private bool _prepend;

    private float _lastTimePlaced;

    [SerializeField] private float _maxCurveAngle;


    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private SplineInstantiate _splineInstantiate;
    private TrackBuilder _trackBuilder;

    void Start()
    {
        _mainCamera = Camera.main;
        _trackBuilder = new TrackBuilder(_splineContainer);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TryHitPlane(out var mousePoint))
            {
                if (GetNearestKnotEndPoint2(mousePoint, out Spline spline, out bool prepend, out Vector3 nearest))
                {
                    _spline = spline;
                    _prepend = prepend;
                    _lastPoint = nearest;
                    AddTrack(mousePoint);
                }
                else
                {
                    _spline = _splineContainer.AddSpline();
                    _spline.Closed = false;
                    _lastPoint = mousePoint;
                    AddTrack(mousePoint);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (TryHitPlane(out Vector3 mousePoint))
            {
                // Debug.DrawLine(_lastPoint, mousePoint, Color.red);
                if (Vector3.SqrMagnitude(_lastPoint - mousePoint) > 0.5f)
                {
                    // var lastSegment = _lastPoint - _pointBeforeLastPoint;
                    // var extendedSegment = lastSegment + _lastPoint;
                    // var nextSegment = mousePoint - _lastPoint;
                    // var rotatedSegment = Vector3.RotateTowards(extendedSegment, nextSegment, Mathf.Deg2Rad * _maxCurveAngle, 0f);
                    // var angle = Vector3.Angle(lastSegment, nextSegment);
                    // if (angle < 90 || _pointBeforeLastPoint == Vector3.zero)
                    // var proposedPoint = rotatedSegment.normalized + _lastPoint;
                    // Debug.DrawLine(_lastPoint, proposedPoint);

                    if (Time.timeSinceLevelLoad - _lastTimePlaced > 0.1)
                    {
                        AddTrack(mousePoint);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            var points = _checkPoints.Select(sphere => sphere.transform.position);
            _trackBuilder.BuildTrack(points);
            Clear();
        }
    }

    private bool GetNearestKnotEndPoint2(Vector3 mousePoint, out Spline nearestSpline, out bool prepend,
        out Vector3 closest)
    {
        nearestSpline = null;
        prepend = false;
        if (_splineContainer.Splines.Count == 0 || _splineContainer.Splines.All(spline => spline.Count == 0))
        {
            closest = Vector3.zero;
            return false;
        }

        nearestSpline = GetNearestSpline(mousePoint);
        var firstPointDistance = math.distancesq(nearestSpline[0].Position, mousePoint);
        var lastPointDistance = math.distancesq(nearestSpline[^1].Position, mousePoint);
        if (firstPointDistance < lastPointDistance)
        {
            closest = nearestSpline[0].Position;
            prepend = true;
        }
        else
        {
            closest = nearestSpline[^1].Position;
            prepend = false;
        }

        return true;
    }

    private Spline GetNearestSpline(float3 mousePoint)
    {
        return _splineContainer.Splines.Where(spline => spline.Count > 0).OrderBy(spline =>
        {
            var firstPointDistance = math.distancesq(spline[0].Position, mousePoint);
            var lastPointDistance = math.distancesq(spline[^1].Position, mousePoint);
            return math.min(firstPointDistance, lastPointDistance);
        }).First();
    }

    private void AddTrack(Vector3 point)
    {
        // var sphere = Instantiate(_sphere, point, Quaternion.identity);
        // _checkPoints.Add(sphere);
        if (_prepend)
        {
            _spline.Insert(0, point);
        }
        else
        {
            _spline.Add(point);
        }
        _splineInstantiate.UpdateInstances();
        _pointBeforeLastPoint = _lastPoint;
        _lastPoint = point;
        _lastTimePlaced = Time.timeSinceLevelLoad;
    }

    private void Clear()
    {
        foreach (var point in _checkPoints)
        {
            Destroy(point);
        }

        _checkPoints.Clear();
    }

    public bool TryHitPlane(out Vector3 position)
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            position = hit.point;
            return true;
        }

        position = Vector3.zero;
        return false;
    }
}