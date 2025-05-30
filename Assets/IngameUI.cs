using UnityEngine;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private GlobalSettings globalSettings;
    [SerializeField] private TextMesh hpText;
    [SerializeField] private Transform hpBar;
    [SerializeField] private SpriteRenderer hpBarSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hpBarSprite.color = gameMaster.playerHp > 0 ? globalSettings.playerColors[gameMaster.playerId] : Color.clear;
        hpText.text = gameMaster.playerHp.ToString();
        hpBar.localPosition = new(.5f * gameMaster.playerHp / globalSettings.maxHp - .5f, 0f);
        hpBar.localScale = new Vector3((float)gameMaster.playerHp / globalSettings.maxHp, hpBar.localScale.y, 1f);
    }
}
