using System;
using UnityEngine;
using UnityEngine.Splines;

public class TrainSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _train;
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private float _timeBetweenSpawnsInSeconds = 30f;
    
    private void Start()
    {
        SpawnTrains();
    }

    private async void SpawnTrains()
    {
        while (true)
        {
            await Awaitable.WaitForSecondsAsync(_timeBetweenSpawnsInSeconds);
            if (_splineContainer.Splines.Count == 0)
                continue;
            SpawnTrain(_splineContainer);
        }
    }
    
    public void SpawnTrain(SplineContainer splineContainer)
    {
        var trainParent = Instantiate(_train, transform.position, transform.rotation);
        var splineAnimates = trainParent.GetComponentsInChildren<SplineAnimate>();
        foreach (var splineAnimate in splineAnimates)
        {
            splineAnimate.Container = splineContainer;
            splineAnimate.Play();
        }
    }
}