using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Transform Model;
    [Tooltip("Radians per second of the tube's curve the player covers.")]
    public float speedInRadians = 0.25f;
    [Tooltip("Radians per second of the tube's inner circle the player covers.")]
    public float turnAngle = 1.5f;

    public float Score { get; private set; } = 0;
    public int Lives { get; private set; } = 3;

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
        if(Lives > 0)
            HandleInput();

        currentTube = TubeSystem.Instance.tubes[1];
        radiansTravelledInCurrentTube += speedInRadians * Time.deltaTime;
        if(Lives > 0)
            Score += speedInRadians * Time.deltaTime * (currentTube.CurveLength / (2 * Mathf.PI));
        if(radiansTravelledInCurrentTube > currentTube.RadiansCovered)
        {
            radiansTravelledInCurrentTube -= currentTube.RadiansCovered;
            TubeSystem.Instance.RemoveSegment();
            currentTube = TubeSystem.Instance.tubes[1];
            //Offset the angle by the rotation of the new pipe, so the player re-enters in the right position
            float offsetInRadians = (currentTube.Rotation / 360) * 2 * Mathf.PI;
            angle = (angle + offsetInRadians) % (2 * Mathf.PI);
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

    IEnumerator GameOver()
    {
        PlayerPrefs.SetInt("EditLeaderboardEntryIndex", -1);

        var leaderBoard = Leaderboard.GetLeaderboard();
        string[] names = leaderBoard.Item1;
        int[] scores = leaderBoard.Item2;

        //Place the player's score on the leaderboard
        for(int i = 9; i >= 0; i--)
        {
            //If there is a higher score and that score is lower than the currently iterated score, bump this score down one
            if(i >= 1 && scores[i-1] < Score)
            {
                names[i] = names[i - 1];
                scores[i] = scores[i - 1];
            }
            else if(scores[i] < Score)
            {
                names[i] = " ";
                scores[i] = (int)Score;
                PlayerPrefs.SetInt("EditLeaderboardEntryIndex", i);
            }
        }

        Leaderboard.SetLeaderboard(names, scores);

        //Turn of the model to show the player has died
        Model.gameObject.SetActive(false);

        //Wait for a few seconds before switching scenes so the player can see they died
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("ScoreScreen");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Obstacle")
        {
            Lives--;
            if (Lives <= 0)
                StartCoroutine(GameOver());
        }
    }
}
