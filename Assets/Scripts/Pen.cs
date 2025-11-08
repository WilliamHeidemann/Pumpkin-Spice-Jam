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
    [SerializeField] private LayerMask _layerMask;
    private Vector3 _pointBeforeLastPoint;
    private Vector3 _lastPoint;

    private TrackBuilder.SplineConnection _splineConnection;

    private float _lastTimePlaced;

    [SerializeField] private float _maxCurveAngle;
    [SerializeField] private float _maxDistanceToExistingTrack;


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
        if (!TryHitPlane(out var mousePoint))
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (_trackBuilder.GetSplineConnection(mousePoint, _maxDistanceToExistingTrack)
                .IsSome(out var splineConnection))
            {
                // assert that we want to start building from connection point
                _splineConnection = splineConnection;
            }
            else
            {
                _splineConnection = _trackBuilder.Build(mousePoint);
                // start a new spline and assert that we want to start building from the new spline
            }
            
            // create the next point to be modified each frame
            _splineConnection.Spline.SetLastPoint(mousePoint);
        }

        if (Input.GetMouseButton(0))
        {
            // put the last point where mousepoint is
            _splineConnection.Spline.SetLastPoint(mousePoint);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_trackBuilder.GetSplineConnection(mousePoint, _maxDistanceToExistingTrack).IsSome(out var connection))
            {
                _splineConnection.Spline.SetLastPoint(connection.Point);
            }
            // check if the point should snap to a nearby connection point
            // or if it should end in the open
        }
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