using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour {

    [SerializeField]
    private GameObject sphereOn;
    [SerializeField]
    private GameObject sphereOff;
    [SerializeField]
    private float minWaitTime = 0.1f;
    [SerializeField]
    private float maxWaitTime = 1f;
    [SerializeField]
    private float minWaitTimeWhenOn = 0.1f;
    [SerializeField]
    private float maxWaitTimeWhenOn = 1f;
    [SerializeField]
    private float minTimeTurnon = 0.4f;
    [SerializeField]
    private float maxTimeTurnon = 4f;

    private float totalTime = 0f;
    private float totalTimeWait = 0f;

    private float lightMaxIntensity = 0f;

    private float lightCooldown = 0f;
    private float lightCdWait = 0f;

    private bool systemActive = false;

    // Use this for initialization
    void Start ()
    {
        lightMaxIntensity = GetComponent<Light>().intensity;
        GetComponent<Light>().intensity = 0f;
        sphereOff.SetActive(true);
        sphereOn.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (systemActive)
        {
            totalTime += Time.deltaTime;

            if (totalTime >= totalTimeWait)
            {
                GetComponent<Light>().intensity = lightMaxIntensity;
                sphereOn.SetActive(true);
                sphereOff.SetActive(false);
                systemActive = false;
            }
            else
            {
                lightCooldown += Time.deltaTime;
                if (lightCooldown >= lightCdWait)
                {
                    lightCooldown = 0f;

                    if (GetComponent<Light>().intensity < lightMaxIntensity * 0.5f)
                    {
                        GetComponent<Light>().intensity = lightMaxIntensity;
                        sphereOn.SetActive(true);
                        sphereOff.SetActive(false);
                        lightCdWait = Random.Range(minWaitTimeWhenOn, maxWaitTimeWhenOn);
                    }
                    else
                    {
                        GetComponent<Light>().intensity = 0f;
                        sphereOn.SetActive(false);
                        sphereOff.SetActive(true);
                        lightCdWait = Random.Range(minWaitTime, maxWaitTime);
                    }
                }
            }
        }

    }

    private bool isOn = false;

    public bool On
    {
        get
        {
            return isOn;
        }
        set
        {
            isOn = value;

            if (isOn)
            {
                systemActive = true;
                totalTime = 0f;
                totalTimeWait = Random.Range(minTimeTurnon, maxTimeTurnon);
                lightCooldown = 0f;
                lightCdWait = Random.Range(minWaitTime, maxWaitTime);
            }
            else
            {
                systemActive = false;
                GetComponent<Light>().intensity = 0f;
                sphereOn.SetActive(false);
                sphereOff.SetActive(true);
            }
        }
    }
}
