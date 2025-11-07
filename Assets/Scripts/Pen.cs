using System.Collections.Generic;
using System.Linq;
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
    
    [SerializeField] private SplineContainer _splineContainer;
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
            if (TryHitPlane(out var position))
            {
                AddSphere(position);
            }
        }
        
        if (Input.GetMouseButton(0))
        {
            if (TryHitPlane(out var position))
            {
                if (Vector3.SqrMagnitude(_lastPoint - position) is > 0.5f and < 3)
                {
                    var lastSegment = _lastPoint - _pointBeforeLastPoint;
                    var nextSegment = position - _lastPoint;
                    var angle = Vector3.Angle(lastSegment, nextSegment);
                    if (angle < 90 || _pointBeforeLastPoint == Vector3.zero)
                    {
                        AddSphere(position);
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

    private void AddSphere(Vector3 position)
    {
        var sphere = Instantiate(_sphere, position, Quaternion.identity);
        _checkPoints.Add(sphere);
        _pointBeforeLastPoint = _lastPoint;
        _lastPoint = position;
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
