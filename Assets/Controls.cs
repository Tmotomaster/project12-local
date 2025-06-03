using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private GlobalSettings globalSettings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMaster.playerId == -1 || gameMaster.playerHp <= 0) return;
        
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0))
        {
            gameMaster.selfPlayer.GetComponent<PlayerLogic>().shooting = true;
        }
        else
        {
            gameMaster.selfPlayer.GetComponent<PlayerLogic>().shooting = false;
        }

        // Movement
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            // gameMaster.selfPlayer.transform.position += globalSettings.playerSpeed * Time.deltaTime * Vector3.up;
            movement.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            // gameMaster.selfPlayer.transform.position -= globalSettings.playerSpeed * Time.deltaTime * Vector3.up;
            movement.y -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // gameMaster.selfPlayer.transform.position -= globalSettings.playerSpeed * Time.deltaTime * Vector3.right;
            movement.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            // gameMaster.selfPlayer.transform.position += globalSettings.playerSpeed * Time.deltaTime * Vector3.right;
            movement.x += 1;
        }
        if (movement != Vector3.zero)
        {
            movement.Normalize();
            gameMaster.selfPlayer.transform.position += globalSettings.playerSpeed * Time.deltaTime * movement;
        }
        gameMaster.selfPlayer.transform.position = new Vector3(
            Mathf.Clamp(gameMaster.selfPlayer.transform.position.x, -globalSettings.worldSize, globalSettings.worldSize),
            Mathf.Clamp(gameMaster.selfPlayer.transform.position.y, -globalSettings.worldSize, globalSettings.worldSize)
        );
        Camera.main.transform.position = gameMaster.selfPlayer.transform.position + new Vector3(0, 0, -10);

        // Mouse rotation stuff
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gameMaster.selfPlayer.transform.rotation = Quaternion.Euler(0, 0, GlobalSettings.RealArctan(mousePos - gameMaster.selfPlayer.transform.position));
    }
}
