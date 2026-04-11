using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class MainMenuCanvasScript : MonoBehaviour
{
    [SerializeField] private GameObject[] spell1Info;
    [SerializeField] private GameObject[] spell2Info;
    [SerializeField] private GameObject[] spell3Info;
    private GameObject[][] spellInfo;
    [SerializeField] private Sprite redRuneSprite;
    [SerializeField] private Sprite yellowRuneSprite;
    [SerializeField] private Sprite blueRuneSprite;
    private List<List<RuneTypes>> activeSpells;
    private Dictionary<RuneTypes, Sprite> runeSprites = new Dictionary<RuneTypes, Sprite>();
    [SerializeField] private Button startButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the spellInfo array with the serialized GameObjects
        spellInfo = new GameObject[3][];
        spellInfo[0] = spell1Info; 
        spellInfo[1] = spell2Info; 
        spellInfo[2] = spell3Info;
        // Initialize the runeSprites dictionary with the corresponding sprites for each RuneType
        runeSprites[RuneTypes.Red] = redRuneSprite;
        runeSprites[RuneTypes.Yellow] = yellowRuneSprite;
        runeSprites[RuneTypes.Blue] = blueRuneSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RandomizeSpells()
    {
        GameManager.Instance.GenerateSpellPool();
        activeSpells = GameManager.Instance.GetActiveSpells();

        int spellIndex = 0;
        int runeIndex = 0;
        foreach (var spell in activeSpells)
        {
            foreach (var rune in spell)
            {
                Image runeImage = spellInfo[spellIndex][runeIndex].GetComponent<Image>();
                runeImage.sprite = runeSprites[rune];
                runeIndex++;
            }
            runeIndex = 0;
            spellIndex++;
        }

        startButton.interactable = true;
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }
}
