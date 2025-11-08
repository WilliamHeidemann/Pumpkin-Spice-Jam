using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UtilityToolkit.Runtime;

public class Pen : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] private LayerMask _layerMask;
    private Vector3 _pointBeforeLastPoint;
    private Vector3 _lastPoint;

    private Option<TrackBuilder.SplineConnection> _splineConnection;

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
        if (Input.GetMouseButtonDown(1))
        {
            _splineConnection = Option<TrackBuilder.SplineConnection>.None;
        }
        
        if (!TryHitPlane(out var mousePoint))
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (_splineConnection.IsSome(out var splineConnection))
            {
                if (_trackBuilder.GetNearestSplineConnection(mousePoint, _maxDistanceToExistingTrack)
                    .IsSome(out var nearestConnection))
                {
                    splineConnection.SetLastPoint(nearestConnection.Point);
                }
                // _splineConnection = Option<TrackBuilder.SplineConnection>.None;
            }
            // else
            {
                if (_trackBuilder.GetNearestSplineConnection(mousePoint, _maxDistanceToExistingTrack)
                    .IsSome(out var nearestConnection))
                {
                    _splineConnection = Option<TrackBuilder.SplineConnection>.Some(nearestConnection);
                }
                else
                {
                    _splineConnection =  Option<TrackBuilder.SplineConnection>.Some(_trackBuilder.New(mousePoint));
                }
            
                if (_splineConnection.IsSome(out var connection))
                {
                    connection.Add(mousePoint);
                }
            }
        }

        if (_splineConnection.IsSome(out var current))
        { 
            current.SetLastPoint(mousePoint);
        }
        
        _splineInstantiate.UpdateInstances();
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