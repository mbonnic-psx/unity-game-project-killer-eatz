using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{

    [SerializeField] private Light light;
    public AudioSource click;

    // Start is called before the first frame update
    void Start()
    {
        if (light != null)
        {
            light.enabled = false;
        }
        click = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Turn FLashlight On/Off
        if (Input.GetMouseButtonDown(0))
        {
            ToggleFlashlight(); //Left Click
            if (click != null)
            {
                click.Play(0);
            }
            //play Audio Click ON/OFF
        }
    }

    //Method to toggle on and off the light in the flashlight
    private void ToggleFlashlight()
    {
        if (light != null)
        {
            light.enabled = !light.enabled;
        }
    }

}
