using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Image image;
    public RocketProps rocketProps;

    private int lastHealth = 0;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (rocketProps.CurrentHealth != lastHealth)
        {
            lastHealth = rocketProps.CurrentHealth;
            image.fillAmount = ((float)lastHealth) / rocketProps.MaxHealth;
        }
	}
}
