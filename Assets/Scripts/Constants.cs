using UnityEngine;

public static class Constants
{
    #region SERIAL COMMUNICATION
    public const string SERIAL_PORT = "COM8";
    public const int BAUD_RATE = 9600;
    public const int UDP_PORT = 8888;
    #endregion

    #region ONLY_FOR_DEBUG_PURPOSES
    public static bool IN_DEBUG_MODE = false;
    public const string LOCAL_IP = "192.168.100.15"; //ESP8266 local ip after it's connected to my local network
    #endregion

    #region CONTROLLER_PREFIXES
    public const string STEERING_PREFIX = "STEERING_ANGLE:";
    public const string THROTTLE_PREFIX = "THROTTLE:";
    public const string RESET_PREFIX = "RESET:";
    #endregion

    #region DEFAULT_SETTINGS
    public const float DEFAULT_STEERING_ANGLE = 90f;
    #endregion

    #region UTILS
    public static string GetIPInput()
    {
        return PlayerPrefs.GetString("IP");
    }
    #endregion
}
