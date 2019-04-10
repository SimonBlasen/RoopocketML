using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProps : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float tickTime = 1f;
    [SerializeField]
    private float collisionDamage = 40;
    [SerializeField]
    private float impactToleranceThrusters = 5f;
    [SerializeField]
    private int maxHealth = 200;
    [SerializeField]
    private float maxFuel = 200f;
    [SerializeField]
    private float thrustFuelPerSecond = 4f;

    [Header("References")]
    [SerializeField]
    private Transform[] okToHitTransforms;
    [SerializeField]
    private RocketController rocketController;
    [SerializeField]
    private CameraMultiController cameraMulti;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject dieExplosion;
    [SerializeField]
    private GameObject deadRocket;
    [SerializeField]
    private GameObject dieExplosionForce;

    private int currentHealth;

    private float currentFuel = 0f;

    private float tickCounter = 0f;

    // Use this for initialization
    void Start()
    {
        MLAccDamage = 0;
        currentHealth = maxHealth;
        currentFuel = maxFuel;
        cameraMulti.IsRocketDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFuel > 0f)
        {
            for (int i = 0; i < rocketController.Thrusts.Length; i++)
            {
                if (rocketController.Thrusts[i])
                {
                    currentFuel -= Time.deltaTime * thrustFuelPerSecond;
                }
            }
        }
        else
        {
            currentFuel = 0f;
        }
    }

    public bool OutOfFuel
    {
        get
        {
            return currentFuel <= 0f;
        }
    }

    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
    }

    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }

    public float CurrentFuel
    {
        get
        {
            return currentFuel;
        }
    }

    public float MaxFuel
    {
        get
        {
            return maxFuel;
        }
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
        if (currentFuel > maxFuel)
        {
            currentFuel = maxFuel;
        }
    }

    public void Die()
    {
        //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //Debug.Log("Died");
        GameObject explosion = Instantiate(dieExplosion);
        explosion.transform.position = transform.position;

        GameObject deadRocketInst = Instantiate(deadRocket);
        deadRocketInst.transform.position = transform.position;
        deadRocketInst.transform.rotation = transform.rotation;

        cameraMulti.IsRocketDead = true;

        Destroy(explosion, 3.8f);

        GameObject instForce = Instantiate(dieExplosionForce);
        instForce.transform.position = transform.position;
        
        Destroy(instForce, 2f);

        gameObject.SetActive(false);
    }

    [Header("Shake Settings")]
    [SerializeField]
    private float magnitude = 2f;
    [SerializeField]
    private float roughness = 10f;
    [SerializeField]
    private float fadeIn = 0.1f;
    [SerializeField]
    private float fadeOut = 0.3f;

    public void Damage(int damage)
    {
        if (rocketController.IsKI == false)
        {
            CameraShakeInstance c = CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeIn, fadeOut);
        }
        else
        {
            //TODO just for learning

            MLAccDamage += 1;
        }


        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public int MLAccDamage
    {
        get;set;
    }




    private void OnCollisionStay(Collision collision)
    {/*
        bool notOkay = false;
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            bool thisCollisionIsOkay = false;
            for (int j = 0; j < okToHitTransforms.Length; j++)
            {
                if (collision.contacts[i].thisCollider.transform.GetInstanceID() == okToHitTransforms[j].GetInstanceID())
                {
                    thisCollisionIsOkay = true;
                    break;
                }
            }
            if (thisCollisionIsOkay == false)
            {
                notOkay = true;
                break;
            }
        }

        if (notOkay)
        {
            tickCounter += Time.deltaTime;
            if (tickCounter >= tickTime)
            {
                //Debug.Log("Collision stay");
                tickCounter = 0f;
                Damage(collisionDamage);
            }
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Impact: " + collision.impulse.magnitude.ToString());
        bool notOkay = false;
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            bool thisCollisionIsOkay = false;
            for (int j = 0; j < okToHitTransforms.Length; j++)
            {
                if (collision.contacts[i].thisCollider.transform.GetInstanceID() == okToHitTransforms[j].GetInstanceID())
                {
                    thisCollisionIsOkay = true;
                    break;
                }
            }
            if (thisCollisionIsOkay == false)
            {
                notOkay = true;
                break;
            }
        }

        if (notOkay)
        {
            //Debug.Log("Collision");
            //tickCounter = 0f;
            Damage((int)(collisionDamage * collision.impulse.magnitude));
        }
        else
        {
            if (collision.impulse.magnitude > impactToleranceThrusters)
            {
                Damage((int)(collisionDamage * (collision.impulse.magnitude - impactToleranceThrusters)));
            }
        }
    }
}
