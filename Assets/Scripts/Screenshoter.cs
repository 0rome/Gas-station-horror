using UnityEngine;

public class Screenshoter : MonoBehaviour
{
    [SerializeField] private string fileName = "screen_1";
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ScreenCapture.CaptureScreenshot(fileName + ".png");
        }
    }
}
