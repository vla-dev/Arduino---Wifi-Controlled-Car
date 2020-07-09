using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrottleController : MonoBehaviour
{
    public Slider throttleInput;
    public float speed = 10;

    void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            if (throttleInput.value > 5)
            {
                throttleInput.value = throttleInput.value - speed * Time.deltaTime;
            }
            else if (throttleInput.value < -5)
            {
                throttleInput.value = throttleInput.value + speed * Time.deltaTime;
            }
        }

        if(throttleInput.value > 5 || throttleInput.value < -5) HandleSendThrottleData(throttleInput.value);
    }

    public void HandleSendThrottleData(float value)
    {
        MainController.Instance.SendUDPData(Constants.THROTTLE_PREFIX + value.ToString());
    }
}
