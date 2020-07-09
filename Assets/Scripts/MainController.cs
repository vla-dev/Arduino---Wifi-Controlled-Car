using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    private static MainController _instance;
    public static MainController Instance { get { return _instance; } }
   
    public Text connectedIp;

    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        connectedIp.text = Constants.GetIPInput();
    }

    public void SendUDPData(string data)
    {
        SendToWifi.Instance.SendData(data);
    }

    public void ResetToDefault()
    {
        SendToWifi.Instance.SendData(Constants.RESET_PREFIX);
    }
}
