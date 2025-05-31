using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PacketSender : MonoBehaviour
{
    [SerializeField] private PacketReceiver packetReceiver;

    private UdpClient udpClient;

    void Start()
    {
        // udpClient = new UdpClient();
        udpClient = packetReceiver.udpClient;
        // udpClient.Connect(serverIP, serverPort);
        // SendPacket("Hello, server!");
    }

    public void SendPacket(byte[] data)
    {
        try
        {
            udpClient.Send(data, data.Length);
            // print("Packet sent");
        }
        catch (SocketException e)
        {
            Debug.Log("Error connecting to server: " + e.ToString());
        }
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
