using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialScript : MonoBehaviour
{
    public static bool isTutorialActive;

    public GameObject TutorialChunk;

    [Header("Tutorial Panels")]
    [SerializeField] private GameObject swipeTutorial;
    [SerializeField] private GameObject dodgeTutorial;
    [SerializeField] private GameObject shootTutorial;
    [SerializeField] private GameObject spellTutorial;
    [SerializeField] private GameObject endTutorial;
    private RectTransform canvasTransform;

    private GameObject spawnedTutorialChunk;

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
        spawnedTutorialChunk = Instantiate(TutorialChunk, new Vector3(8, 0, 0), Quaternion.identity);
    }

    public void ShowSwipeControlsTutorial()
    {
        Debug.Log("Showing Swipe Controls tutorial");
        OpenTutorial(swipeTutorial);
    }

    public void ShowDodgeTutorial()
    {
        Debug.Log("Showing Dodge tutorial");
        OpenTutorial(dodgeTutorial);
    }

    public void ShowShootTutorial()
    {
        Debug.Log("Showing Shoot tutorial");
        OpenTutorial(shootTutorial);
    }

    public void ShowSpellTutorial()
    {
        Debug.Log("Showing Spell tutorial");
        OpenTutorial(spellTutorial);
    }

    public void ShowEndTutorial()
    {
        Debug.Log("Showing End tutorial");
        OpenTutorial(endTutorial);
        Destroy(spawnedTutorialChunk);
        isTutorialActive = false;
    }

    private void OpenTutorial(GameObject prefab)
    {
        if (!isTutorialActive || prefab == null) return;

        if (canvasTransform == null)
        {
            Canvas activeCanvas = GameObject.FindAnyObjectByType<Canvas>();
            if (activeCanvas != null) canvasTransform = activeCanvas.GetComponent<RectTransform>();
            else return;
        }

        Instantiate(prefab, canvasTransform);

        Time.timeScale = 0f;
    }
}