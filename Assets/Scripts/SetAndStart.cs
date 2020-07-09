using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetAndStart : MonoBehaviour
{
    public InputField ip;

    public void StartGame()
    {
        PlayerPrefs.SetString("IP", ip.text);
        SceneManager.LoadScene(1);
    }
}
