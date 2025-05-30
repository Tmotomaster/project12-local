using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private GlobalSettings globalSettings;
    [SerializeField] private PacketReceiver receiver;
    [SerializeField] private PacketSender sender;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject projectileObject;
    [SerializeField] private Transform gameMap;
    public GameObject selfPlayer;

    [Header("Connection Stuff")]
    public bool firstConnection = true;
    public int playerId = -1;
    public int playerHp = 0;
    public byte[] playerBytes = null;
    public byte[] projectileBytes = null;

    [SerializeField] private float retryTime = 3f;
    public int maxRetries = 5;
    public float lastUpdate = 0f;
    [SerializeField] private float timeoutTime = 10f;
    public bool updateNeeded = false;

    private float time = 0f;
    public int retries = 0;

    [SerializeField] private int playerByteWidth = 9;
    [SerializeField] private int projectileByteWidth = 9;
    private List<GameObject> players = new();
    private List<Projectile> projectiles = new();

    private readonly List<uint> existingProjectiles = new();

    private byte[] connectionPacket;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        connectionPacket = new byte[] {
            0x03, // Join/Ping packet; wait for [00] response
            BitConverter.GetBytes(globalSettings.serverLevel)[0],
            BitConverter.GetBytes(globalSettings.serverLevel)[1]
        };
        print("Establishing connection...");
        sender.SendPacket(connectionPacket); // Join/Ping packet; wait for [00] response
        for (int i = 0; i < 16; i++)
        {
            GameObject player = Instantiate(playerObject, gameMap);
            player.GetComponent<PlayerLogic>().ID = i;
            player.GetComponent<PlayerLogic>().health = 0;
            // player.GetComponent<PlayerLogic>().score = 0;
            players.Add(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (updateNeeded) return;
        if (playerId == -1)
        {
            time += Time.deltaTime;
            firstConnection = true;
            if (time > retryTime)
            {
                time = 0f;
                ++retries;
                if (retries > maxRetries)
                {
                    // Some logique will be added here, trust me I won't postpone this indefinitely.
                    return;
                }
                print("Retrying connection...");
                sender.SendPacket(connectionPacket); // Join/Ping again
            }
        }
        else
        {
            selfPlayer = players[playerId];
            retries = maxRetries;

            // Send current player data, consequently receive player and projectile data.
            // Dead player data alternative (empty [04] packet)
            if (playerHp > 0)
            {
                firstConnection = false;
                // print(100 * (byte)playerId + " | " + (short)(100 * selfPlayer.transform.position.x) + ", " + (short)(100 * selfPlayer.transform.position.y) + ", " + (short)selfPlayer.transform.rotation.eulerAngles.z);
                sender.SendPacket(new byte[] {
                    0x02, // Player data packet
                    (byte)playerId,
                    BitConverter.GetBytes((short)(100 * selfPlayer.transform.position.x))[0],
                    BitConverter.GetBytes((short)(100 * selfPlayer.transform.position.x))[1],
                    BitConverter.GetBytes((short)(100 * selfPlayer.transform.position.y))[0],
                    BitConverter.GetBytes((short)(100 * selfPlayer.transform.position.y))[1],
                    BitConverter.GetBytes((short)selfPlayer.transform.rotation.eulerAngles.z)[0],
                    BitConverter.GetBytes((short)selfPlayer.transform.rotation.eulerAngles.z)[1],
                    (byte)(selfPlayer.GetComponent<PlayerLogic>().shooting ? 1 : 0)
                });
                // print($"Sending player data: {selfPlayer.transform.position.x} | {(short)selfPlayer.transform.position.x}, {selfPlayer.transform.position.y}, {selfPlayer.transform.rotation.eulerAngles.z}");
            }
            else
            {
                sender.SendPacket(new byte[] { 0x04, (byte)playerId }); // Dead player packet
            }

            for (int j = 0; j < players.Count; j++)
            {
                players[j].GetComponent<PlayerLogic>().health = 0; // Default health to 0 in case of disconnected players
            }

            for (int i = 0; i < (playerBytes.Length - 1) / playerByteWidth; i++)
            {
                int ID = playerBytes[playerByteWidth * i + 1];
                int posX = BitConverter.ToInt16(new byte[] { playerBytes[playerByteWidth * i + 2], playerBytes[playerByteWidth * i + 3] }, 0);
                int posY = BitConverter.ToInt16(new byte[] { playerBytes[playerByteWidth * i + 4], playerBytes[playerByteWidth * i + 5] }, 0);
                int rot = BitConverter.ToInt16(new byte[] { playerBytes[playerByteWidth * i + 6], playerBytes[playerByteWidth * i + 7] }, 0);
                int health = playerBytes[playerByteWidth * i + 8];
                int shooting = playerBytes[playerByteWidth * i + 9];

                players[ID].GetComponent<PlayerLogic>().health = health;



                if (ID != playerId || playerHp <= 0)
                {
                    players[ID].transform.SetPositionAndRotation(new Vector3((float)posX / 100, (float)posY / 100, 0), Quaternion.Euler(0, 0, rot));
                    // print("Updated player " + ID + " data");
                }
                if (ID == playerId)
                {
                    playerHp = health;
                }
                players[ID].GetComponent<PlayerLogic>().shooting = shooting != 0;
            }

            List<uint> outdatedProjectiles = new();
            for (int i = 0; i < existingProjectiles.Count; i++)
            {
                outdatedProjectiles.Add(existingProjectiles[i]);
            }

            for (int i = 0; i < (projectileBytes.Length - 1) / projectileByteWidth; i++)
            {
                int ownerId = projectileBytes[projectileByteWidth * i + 1];
                uint ID = projectileBytes[projectileByteWidth * i + 2];
                int posX = BitConverter.ToInt16(new byte[] { projectileBytes[projectileByteWidth * i + 3], projectileBytes[projectileByteWidth * i + 4] }, 0);
                int posY = BitConverter.ToInt16(new byte[] { projectileBytes[projectileByteWidth * i + 5], projectileBytes[projectileByteWidth * i + 6] }, 0);
                int velX = projectileBytes[projectileByteWidth * i + 7];
                int velY = projectileBytes[projectileByteWidth * i + 8];
                int ls = projectileBytes[projectileByteWidth * i + 9];

                if (!existingProjectiles.Contains(ID))
                {
                    existingProjectiles.Add(ID);
                }
                outdatedProjectiles.Remove(ID);

                bool notFound = true;
                for (int j = 0; j < projectiles.Count; j++)
                {
                    if (projectiles[j].ID == ID)
                    {
                        projectiles[j].UpdateProjectile(posX, posY, velX, velY, ls);
                        float lsColor = ls > 10 ? 1 : (float)ls / 10;
                        projectiles[j].projectile.GetComponent<SpriteRenderer>().color = new Color(lsColor * globalSettings.playerColors[ownerId].r + Mathf.Min(1 - lsColor, .8f), lsColor * globalSettings.playerColors[ownerId].g + Mathf.Min(1 - lsColor, .8f), lsColor * globalSettings.playerColors[ownerId].b + Mathf.Min(1 - lsColor, .8f));
                        // projectiles[j].posX = posX;
                        // projectiles[j].posY = posY;
                        // projectiles[j].velX = velX;
                        // projectiles[j].velY = velY;
                        // projectiles[j].ls = ls;
                        // projectiles[j].projectile.transform.position = new((float)posX / 100, (float)posY / 100);
                        notFound = false;
                        break;
                    }
                }
                if (notFound)
                {
                    GameObject projectile = Instantiate(projectileObject, gameMap);
                    projectile.transform.position = new((float)posX / 100, (float)posY / 100);
                    projectile.GetComponent<SpriteRenderer>().color = globalSettings.playerColors[ownerId];
                    projectiles.Add(new Projectile(ownerId, ID, posX, posY, velX, velY, ls, projectile));
                }
            }

            // print(outdatedProjectiles.Count + " outdated projectiles");
            for (int i = 0; i < outdatedProjectiles.Count; i++)
            {
                existingProjectiles.Remove(outdatedProjectiles[i]);
                // print("Removing outdated projectile " + outdatedProjectiles[i]);
                for (int j = 0; j < projectiles.Count; j++)
                {
                    if (projectiles[j].ID == outdatedProjectiles[i])
                    {
                        Destroy(projectiles[j].projectile);
                        projectiles.RemoveAt(j);
                        break;
                    }
                }
            }

            // Timeout logic
            if (Time.time - lastUpdate > timeoutTime)
            {
                print("Connection timed out, disconnecting...");
                playerId = -1;
                playerHp = 0;
            }
        }
    }

    public void OnReceivePlayerData(byte[] data)
    {
        playerBytes = data;
        // print("Received player data");
    }

    public void OnReceiveProjectileData(byte[] data)
    {
        projectileBytes = data;
        // print("Received projectile data");
        // for (int i = 0; i < (projectileBytes.Length - 1) / projectileByteWidth; i++)
        // {
        //     uint ID = projectileBytes[projectileByteWidth * i + 2];
        //     int ls = projectileBytes[projectileByteWidth * i + 9];
        //     if (ls <= 0)
        //     {
        //         for (int j = 0; j < projectiles.Count; j++)
        //         {
        //             if (projectiles[j].ID == ID)
        //             {
        //                 Destroy(projectiles[j].projectile);
        //                 projectiles.RemoveAt(j);
        //                 break;
        //             }
        //         }
        //     }
        // }
    }
}

public class Projectile
{
    public int ownerId;
    public uint ID;
    public int posX;
    public int posY;
    public int velX;
    public int velY;
    public int ls;

    public GameObject projectile;

    public Projectile(int ownerId, uint ID, int posX, int posY, int velX, int velY, int ls, GameObject projectile)
    {
        this.ownerId = ownerId;
        this.ID = ID;
        this.posX = posX;
        this.posY = posY;
        this.velX = velX;
        this.velY = velY;
        this.ls = ls;
        this.projectile = projectile;
    }

    public void UpdateProjectile(int posX, int posY, int velX, int velY, int ls)
    {
        this.posX = posX;
        this.posY = posY;
        this.velX = velX;
        this.velY = velY;
        this.ls = ls;
        projectile.transform.position = new((float)posX / 100, (float)posY / 100);
    }
}