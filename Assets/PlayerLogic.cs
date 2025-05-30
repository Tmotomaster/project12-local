using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public int ID;
    public int health;
    public bool shooting;
    // public int score;

    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private SpriteRenderer shooterSprite;

    private GlobalSettings globalSettings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        globalSettings = GameObject.FindGameObjectWithTag("GlobalSettings").GetComponent<GlobalSettings>();

        playerSprite.color = globalSettings.playerColors[ID];
        playerSprite.enabled = false;
        shooterSprite.enabled = false;
    }

    void Update()
    {
        if (health > 0)
        {
            playerSprite.enabled = true;
            shooterSprite.enabled = true;
            playerSprite.color = globalSettings.playerColors[ID];
        }
        else
        {
            playerSprite.enabled = false;
            shooterSprite.enabled = false;
        }
    }
}
