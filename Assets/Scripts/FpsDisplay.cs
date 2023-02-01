using UnityEngine;
using UnityEngine.UI;

public class FpsDisplay : MonoBehaviour
{
    public Text fps;        //Show framerate on the Text

    private float pollingTime = 1f;     //how frequently fps display will update
    private float time;
    private int frameCount;

    private void Update()
    {
        //Time gap between first and last frame
        time += Time.deltaTime;

        frameCount++;

        if (time >= pollingTime)
        {
            //Calculate the frame per second
            int frameRate = Mathf.RoundToInt(frameCount / time);
            fps.text = frameRate.ToString() + "FPS";

            time -= pollingTime;
            frameCount = 0;

        }
    }
}
