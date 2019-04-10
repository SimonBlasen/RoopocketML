using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFuelBar : MonoBehaviour {

    public Image image;
    public TextMeshProUGUI text;
    public RocketProps rocketProps;

    private float lastFuel = 0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (rocketProps.CurrentFuel != lastFuel)
        {
            lastFuel = rocketProps.CurrentFuel;
            float percent = lastFuel / rocketProps.MaxFuel;
            image.fillAmount = percent;
            int percentInt = (int)(percent * 100f + 0.999f);
            text.text = percentInt.ToString() + "%";
        }
    }
}
