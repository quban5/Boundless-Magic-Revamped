using Unity.VisualScripting;
using UnityEngine;

public class TutorialTriggerScript : MonoBehaviour
{
    [Header("Action to perform")]
    [SerializeField] private string actionToPerform;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TutorialScript tutorial = GameManager.Instance.GetComponent<TutorialScript>();
            Debug.Log("collided");
            switch (actionToPerform)
            {
                case "ShowControlsSwipeTutorial":
                    tutorial.ShowSwipeControlsTutorial();
                    break;
                
                case "ShowDodgeTutorial":
                    tutorial.ShowDodgeTutorial();
                    break;

                case "ShowShootTutorial":
                    tutorial.ShowShootTutorial();
                    break;

                case "ShowSpellTutorial":
                    tutorial.ShowSpellTutorial();
                    break;

                case "ShowEndTutorial":
                    tutorial.ShowEndTutorial();
                    break;
            }
            Destroy(gameObject);
        }
    }
}
