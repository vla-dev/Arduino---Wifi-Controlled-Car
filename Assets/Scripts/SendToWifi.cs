using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class SendToWifi : MonoBehaviour
{
    private static SendToWifi _instance;
    public static SendToWifi Instance { get { return _instance; } }

    public static int _port = Constants.UDP_PORT;

    private Socket _sock;
    private IPEndPoint _endPoint;

    void Awake()
    {
        SetAsSingleton();
        InitUDP();
    }

    #region PUBLIC_METHODS
    public void SendData(string val)
    {
        string _text = val;
        byte[] _send_buffer = Encoding.ASCII.GetBytes(_text);

        _sock.SendTo(_send_buffer, _endPoint);
    }
    #endregion

    #region PRIVATE_METHODS
    private void SetAsSingleton()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void InitUDP()
    {
        _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        string _savedIPAddress = Constants.IN_DEBUG_MODE ? Constants.LOCAL_IP : Constants.GetIPInput().Trim();
        IPAddress _serverAddr = IPAddress.Parse(_savedIPAddress);
        _endPoint = new IPEndPoint(_serverAddr, _port);
    }
    #endregion
}