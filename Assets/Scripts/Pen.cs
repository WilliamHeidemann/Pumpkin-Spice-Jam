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
                _splineConnection = splineConnection;
            }
            else
            {
                _splineConnection = _trackBuilder.New(mousePoint);
            }
            
            _splineConnection.Add(mousePoint);
        }

        if (Input.GetMouseButton(0))
        {
            _splineConnection.SetLastPoint(mousePoint);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_trackBuilder.GetSplineConnection(mousePoint, _maxDistanceToExistingTrack).IsSome(out var endConnection))
            {
                _splineConnection.SetLastPoint(endConnection.Point);
            }
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