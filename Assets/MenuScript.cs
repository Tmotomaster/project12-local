using UnityEngine;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject lobby;
    [SerializeField] private GameObject defeated;
    [SerializeField] private GameObject connecting;
    [SerializeField] private GameObject noConnection;
    [SerializeField] private GameObject updateNeeded;

    [SerializeField] private ButtonLogic lobbyButton;
    [SerializeField] private ButtonLogic defeatedButton;

    private bool alive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMaster.updateNeeded)
        {
            menu.SetActive(true);
            lobby.SetActive(false);
            defeated.SetActive(false);
            connecting.SetActive(false);
            noConnection.SetActive(false);
            updateNeeded.SetActive(true);
            return;
        }
        if (gameMaster.playerId == -1)
        {
            menu.SetActive(true);
            alive = true;
            if (gameMaster.retries >= gameMaster.maxRetries)
            {
                lobby.SetActive(false);
                defeated.SetActive(false);
                connecting.SetActive(false);
                noConnection.SetActive(true);
                updateNeeded.SetActive(false);
            }
            else
            {
                lobby.SetActive(false);
                defeated.SetActive(false);
                connecting.SetActive(true);
                noConnection.SetActive(false);
                updateNeeded.SetActive(false);
            }
        }
        else if (gameMaster.playerHp <= 0)
        {
            menu.SetActive(true);
            if (alive)
            {
                lobbyButton.disabled = false;
                defeatedButton.disabled = false;
                alive = false;
            }
            if (gameMaster.firstConnection)
            {
                lobby.SetActive(true);
                defeated.SetActive(false);
                connecting.SetActive(false);
                noConnection.SetActive(false);
                updateNeeded.SetActive(false);
            }
            else
            {
                lobby.SetActive(false);
                defeated.SetActive(true);
                connecting.SetActive(false);
                noConnection.SetActive(false);
                updateNeeded.SetActive(false);
            }
        }
        else
        {
            menu.SetActive(false);
            lobby.SetActive(false);
            defeated.SetActive(false);
            connecting.SetActive(false);
            noConnection.SetActive(false);
            updateNeeded.SetActive(false);
            alive = true;
        }
    }
}
