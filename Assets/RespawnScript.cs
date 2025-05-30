using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private PacketSender sender;
    [SerializeField] private ButtonLogic buttonLogic;

    private bool mouseOver = false;

    void OnMouseEnter()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }
    
    void OnMouseUp()
    {
        if (mouseOver && !buttonLogic.disabled)
        {
            if (gameMaster.playerId != -1 && gameMaster.playerHp <= 0)
            {
                // gameMaster.respawnPlayer();
                sender.SendPacket(new byte[] { 0x05, (byte)gameMaster.playerId });
                buttonLogic.disabled = true;
            }
        }
    }
}
