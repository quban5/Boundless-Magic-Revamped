using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
    public static bool isTutorialActive;
    public GameObject TutorialChunk;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTutorial(bool toggleValue)
    {
        isTutorialActive = toggleValue;
    }

    public void PlayTutorial()
    {
        Debug.Log("Playing tutorial");
        GameObject chunkToDestroy = GameObject.Find("Easy1");
        Destroy(chunkToDestroy);
        Instantiate(TutorialChunk, new Vector3(8, 0, 0), Quaternion.identity);
    }

    public void ShowSwipeControlsTutorial()
    {
        Debug.Log("Showing Swipe Controls tutorial");
    }

    public void ShowDodgeTutorial()
    {
        
    }

    public void ShowShootTutorial()
    {
        
    }
}
