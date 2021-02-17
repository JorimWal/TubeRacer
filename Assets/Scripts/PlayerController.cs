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

    //DEBUG
    float BigAngle = 0;
    float anglePerTubeSegment;

    // Start is called before the first frame update
    void Start()
    {
        //DEBUG
        anglePerTubeSegment = (2 * Mathf.PI) / (float)TubeSystem.Instance.TubeSegments;
        Torus t = new Torus() { MajorRadius = 30, MinorRadius = 5 };
        for(int i = 0; i < TubeSystem.Instance.TubeSegments; i++)
        {
            BigAngle = i * anglePerTubeSegment;
            Debug.Log($"Normal point: {t.PointOnTorus(0, BigAngle)}, Segmented point: {t.PointOnSegmentedTorus(0, BigAngle, 20, 16)}");
        }
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
        angle += (x * Mathf.PI * turnAngle * Time.deltaTime);
        angle = angle % (2 * Mathf.PI);

        //DEBUG
        if (Input.GetKeyDown(KeyCode.A))
            BigAngle += anglePerTubeSegment;
        if (Input.GetKeyDown(KeyCode.D))
            BigAngle -= anglePerTubeSegment;
        BigAngle = BigAngle % (2 * Mathf.PI);

        if(Model != null)
            Model.Rotate(transform.right, 1);
    }

    void UpdatePlayerPosition()
    {
        //Get the point on a segmented torus of the tube segment
        Vector3 newTorusPoint = currentTube.Torus.PointOnSegmentedTorus(radiansTravelledInCurrentTube, BigAngle, currentTube.Segments, TubeSystem.Instance.TubeSegments);
        Vector3 estimatedNextTorusPoint = currentTube.Torus.PointOnSegmentedTorus(radiansTravelledInCurrentTube + (speedInRadians * Time.deltaTime), BigAngle, currentTube.Segments, TubeSystem.Instance.TubeSegments);
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
