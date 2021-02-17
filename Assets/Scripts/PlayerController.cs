using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform Model;
    [Tooltip("Radians per second of the tube's curve the player covers.")]
    public float speedInRadians = 0.25f;
    [Tooltip("Radians per second of the tube's inner circle the player covers.")]
    public float turnAngle = 1.5f;

    TubeSegment currentTube;
    float radiansTravelledInCurrentTube = 0;
    float angle = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        currentTube = TubeSystem.Instance.tubes[1];
        radiansTravelledInCurrentTube += speedInRadians * Time.deltaTime;
        if(radiansTravelledInCurrentTube > currentTube.RadiansCovered)
        {
            radiansTravelledInCurrentTube -= currentTube.RadiansCovered;
            TubeSystem.Instance.RemoveSegment();
            currentTube = TubeSystem.Instance.tubes[1];
        }
        UpdatePlayerPosition();
    }

    void HandleInput()
    {
        float x = Input.GetAxis("Horizontal");
        angle -= (x * Mathf.PI * turnAngle * Time.deltaTime);
        angle = angle % (2 * Mathf.PI);

        if(Model != null)
            Model.Rotate(transform.right, 1);
    }

    void UpdatePlayerPosition()
    {
        //Get the point on a segmented torus of the tube segment
        //Vector3 newTorusPoint = currentTube.Torus.PointOnSegmentedTorus(radiansTravelledInCurrentTube, angle, currentTube.Segments, TubeSystem.Instance.TubeSegments);
        //Vector3 estimatedNextTorusPoint = currentTube.Torus.PointOnSegmentedTorus(radiansTravelledInCurrentTube + (speedInRadians * Time.deltaTime), angle, currentTube.Segments, TubeSystem.Instance.TubeSegments);
        Vector3 newTorusPoint = currentTube.Torus.PointOnTorus(radiansTravelledInCurrentTube, angle);
        Vector3 estimatedNextTorusPoint = currentTube.Torus.PointOnTorus(radiansTravelledInCurrentTube + (speedInRadians * Time.deltaTime), angle);
        //Transform that to world coordinates
        Vector3 newPosition = currentTube.transform.TransformPoint(newTorusPoint);
        Vector3 estimatedNextPosition = currentTube.transform.TransformPoint(estimatedNextTorusPoint);

        Vector3 curveCenter = currentTube.transform.TransformPoint(currentTube.Torus.PointOnCurve(radiansTravelledInCurrentTube));
        Vector3 up = (curveCenter - newPosition).normalized;
        Vector3 forward = (estimatedNextPosition - newPosition).normalized;
        transform.rotation = Quaternion.LookRotation(forward, up);
        transform.position = newPosition;
        transform.position += up * transform.localScale.y * 0.5f;
    }
}
