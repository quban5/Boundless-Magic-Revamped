using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private GameObject catcher;
    [SerializeField] private Animator magicUIAnimator;
    [SerializeField] private GameObject magicUI;
    [SerializeField] private CatcherPositions catcherPosition;
    [SerializeField] private Transform[] linePositions;
    [SerializeField] private float catcherSpeed;
    [SerializeField] private GameObject pickup;
    [SerializeField] private float pickupSpawnRange;

    public enum CatcherPositions
    {
        Top = 0,
        Middle = 1,
        Bottom = 2
    }

    private void Start()
    {
        GameManager.magicToggleEvent += OnMagicToggled;
    }

    void Update()
    {
        //Set the score text
        scoreText.text = "Score: " + GameManager.Instance.GetScore().ToString();

        //Set the score text
        ammoText.text = "Ammo: " + GameManager.Instance.GetAmmo().ToString();

        //Make the catcher go up
        if ((PlayerInputManager.Instance.upClicked) && ((int)catcherPosition > 0))
        {
            catcherPosition--;
        }

        //Make the catcher go down
        if ((PlayerInputManager.Instance.downClicked) && ((int)catcherPosition < 2))
        {
            catcherPosition++;
        }

        //Set catcher position
        Vector3 catcherPos = catcher.transform.localPosition;
        Vector3 newCatcherPos = new Vector3(catcherPos.x, linePositions[(int)catcherPosition].localPosition.y);
        catcher.transform.localPosition = Vector3.Lerp(catcherPos, newCatcherPos, catcherSpeed * Time.unscaledDeltaTime);
    }

    public void OnMagicToggled()
    {
        magicUIAnimator.SetTrigger("Tab");

        if (GameManager.Instance.GetMagicActivated())
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject newPickup = Instantiate(pickup, magicUI.transform);
                float pickupXSpawnPos = UnityEngine.Random.Range(-pickupSpawnRange / 2, pickupSpawnRange);
                float pickupYSpawnPos = linePositions[i].localPosition.y;
                newPickup.transform.localPosition = new Vector3(pickupXSpawnPos, pickupYSpawnPos, 0);
                newPickup.GetComponent<PickupScript>().SetPosition((CatcherPositions)i);
            }
        }
    }

    public CatcherPositions GetCatcherPos()
    {
        return catcherPosition;
    }
}
