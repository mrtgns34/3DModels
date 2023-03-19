using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject MarkerObject;
    [Tooltip("X Longitude , Y Latitude")]
    public List<Vector2> Coordinates = new List<Vector2>();
    public float Speed;
    public int SpeedMultiplier;
    private OnlineMapsMarker3D marker;
    public Coroutine coroutine;

    private void Start()
    {
        DrawRoute();
        CreateMarker();
    }
    [ContextMenu("Calculate Distance")]
    public float CalculateTotalDistance()
    {
        float totalDistance = 0;
        for(int i = 0; i < Coordinates.Count-1; i++)
        {
            float distance = OnlineMapsUtils.DistanceBetweenPoints(Coordinates[i], Coordinates[i + 1]).magnitude;
            totalDistance += distance;
        }
        Debug.Log(totalDistance);
        return totalDistance;
    }
    [ContextMenu("Draw Route")]
    public void DrawRoute()
    {
        OnlineMapsDrawingLine onlineMapsDrawingLine = new OnlineMapsDrawingLine(Coordinates,Color.red,0.02f);
        OnlineMapsDrawingElementManager.AddItem(onlineMapsDrawingLine);
    }
    public void CreateMarker()
    {
        marker = OnlineMapsMarker3DManager.CreateItem(Coordinates[0], MarkerObject);
        marker.scale = 5f;
        coroutine=StartCoroutine(MoveBetweenTwoPoints(1));

    }
    int targetPositionIndex;
    public IEnumerator MoveBetweenTwoPoints(int targetPositionIndex)
    {
        while (targetPositionIndex < Coordinates.Count)
        {
            this.targetPositionIndex = targetPositionIndex;
            float timeToMove= (OnlineMapsUtils.DistanceBetweenPoints(Coordinates[targetPositionIndex-1], Coordinates[targetPositionIndex]).magnitude * 1000)/ ((Speed * 1000) / 3600);
            var currentPos = marker.position;
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / (timeToMove/SpeedMultiplier);
                marker.position = Vector3.Lerp(currentPos, Coordinates[targetPositionIndex], t);
                yield return null;
            }
            targetPositionIndex++;
        }
    }
    [ContextMenu("Stop Movement")]
    public void StopCoroutine()
    {
        StopCoroutine(coroutine);
    }
    [ContextMenu("Continue Movement")]
    public void ContinueCoroutine()
    {
        coroutine=StartCoroutine(MoveBetweenTwoPoints(targetPositionIndex));
    }
}
