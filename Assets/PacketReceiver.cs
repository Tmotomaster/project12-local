using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class PacketReceiver : MonoBehaviour
{
    public UdpClient udpClient;
    public string serverIP;
    public int serverPort = 13377;

    [SerializeField] private GameMaster gameMaster;

    private float now;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        udpClient = new UdpClient();
        udpClient.Connect(serverIP, serverPort);
        IPEndPoint remoteEP = new(IPAddress.Any, serverPort);
        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        now = Time.time;
    }

    void Update()
    {
        now = Time.time;
    }

  void ReceiveCallback(IAsyncResult ar)
    {
        try {
            IPEndPoint remoteEP = new(IPAddress.Any, serverPort);
            byte[] receivedData = udpClient.EndReceive(ar, ref remoteEP);
            // string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedData);
            // Debug.Log("Received: " + receivedMessage);

            // print("Received data with type: " + receivedData[0]);
            // print("Received message: " + BitConverter.ToString(receivedData));
            // print("Received message length: " + receivedData.Length);
            gameMaster.lastUpdate = now;
            if (receivedData[0] == 0x00)
            {
                gameMaster.OnReceivePlayerData(receivedData);
                // gameMaster.playerBytes = receivedData;
            }
            else if (receivedData[0] == 0x01)
            {
                gameMaster.OnReceiveProjectileData(receivedData);
                // gameMaster.projectileBytes = receivedData;
            }
            else if (receivedData[0] == 0x04)
            {
                print("Obtained player ID: " + receivedData[1]);
                gameMaster.playerId = receivedData[1];
            }
            else if (receivedData[0] == 0x06)
            {
                print("The version is incompatible with the server! Please update the game!");
                gameMaster.updateNeeded = true;
            }
            else
            {
                print("What even is dis?! Yuck! Ecghwlaaaarbb!!");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error receiving data: " + e.ToString());
        }

        // Continue receiving data
        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
