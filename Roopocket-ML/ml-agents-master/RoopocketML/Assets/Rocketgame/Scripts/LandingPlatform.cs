using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPlatform : MonoBehaviour
{
    [SerializeField]
    private float speedThreshhold = 0.1f;
    [SerializeField]
    private string platformName;

    [Space]

    [SerializeField]
    private Light[] spotlights;
    [SerializeField]
    private GameObject[] landedObjects;
    [SerializeField]
    private GameObject[] notLandedObjects;

    private List<Transform> landedTransforms = new List<Transform>();

    private List<bool> landedSended = new List<bool>();
    private List<float> standingStill = new List<float>();

    private bool isOneLandedBefore = false;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        bool isOneLanded = false;
		for (int i = 0; i < landedTransforms.Count; i++)
        {
            if (IsNotMoving(landedTransforms[i]))
            {
                standingStill[i] += Time.deltaTime;
            }
            else
            {
                standingStill[i] = 0f;
            }

            if (IsLanded(landedTransforms[i]) && landedSended[i] == false)
            {
                landedSended[i] = true;
                Manager.Instance.Landed(landedTransforms[i], platformName);
            }
            else if (IsLanded(landedTransforms[i]) == false && landedSended[i])
            {
                landedSended[i] = false;
                Manager.Instance.Takeoff(landedTransforms[i], platformName);
            }
            if (IsLanded(landedTransforms[i]))
            {
                isOneLanded = true;
            }
        }
        
        toggleLandingEffects(isOneLanded);
        
	}

    private void toggleLandingEffects(bool isOneLanded)
    {
        if (isOneLanded && isOneLandedBefore == false)
        {
            isOneLandedBefore = true;

            executeLandingEffects(true);
        }
        else if (isOneLanded == false && isOneLandedBefore)
        {
            isOneLandedBefore = false;

            executeLandingEffects(false);
        }
    }

    private void executeLandingEffects(bool on)
    {
        for (int i = 0; i < spotlights.Length; i++)
        {
            spotlights[i].GetComponent<FlickerLight>().On = on;
        }
        for (int i = 0; i < landedObjects.Length; i++)
        {
            landedObjects[i].SetActive(on);
        }
        for (int i = 0; i < notLandedObjects.Length; i++)
        {
            notLandedObjects[i].SetActive(!on);
        }
    }

    public bool IsNotMoving(Transform rocket)
    {
        for (int i = 0; i < landedTransforms.Count; i++)
        {
            if (rocket.GetInstanceID() == landedTransforms[i].GetInstanceID())
            {
                return rocket.GetComponent<Rigidbody>().velocity.magnitude < speedThreshhold;
            }
        }

        return false;
    }

    public bool IsLanded(Transform rocket)
    {
        for (int i = 0; i < landedTransforms.Count; i++)
        {
            if (rocket.GetInstanceID() == landedTransforms[i].GetInstanceID())
            {
                return standingStill[i] >= 1f;
            }
        }

        return false;
    }

    public bool StandsOn(Transform rocket)
    {
        for (int i = 0; i < landedTransforms.Count; i++)
        {
            if (rocket.GetInstanceID() == landedTransforms[i].GetInstanceID())
            {
                return true;
            }
        }

        return false;
    }

    public void KIResetStandOn()
    {
        landedTransforms.Clear();
        landedTransforms = new List<Transform>();
    }

    public void TriggerExit(Transform other)
    {
        if (other.tag == "Rocket")
        {
            if (landedTransforms.Contains(other))
            {
                for (int i = 0; i < landedTransforms.Count; i++)
                {
                    if (landedTransforms[i].GetInstanceID() == other.GetInstanceID())
                    {
                        if (landedSended[i])
                        {
                            landedSended[i] = false;
                            Manager.Instance.Takeoff(landedTransforms[i], platformName);
                        }

                        standingStill.RemoveAt(i);
                        landedTransforms.RemoveAt(i);
                        landedSended.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        else if (other.tag == "RocketKI")
        {
            Transform transRock = null;
            transRock = other.GetComponentInChildren<RocketController>().transform;

            if (landedTransforms.Contains(transRock))
            {
                for (int i = 0; i < landedTransforms.Count; i++)
                {
                    if (landedTransforms[i].GetInstanceID() == transRock.GetInstanceID())
                    {
                        if (landedSended[i])
                        {
                            landedSended[i] = false;
                            Manager.Instance.Takeoff(landedTransforms[i], platformName);
                        }

                        standingStill.RemoveAt(i);
                        landedTransforms.RemoveAt(i);
                        landedSended.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

    public void TriggerEnter(Transform other)
    {
        if (other.tag == "Rocket")
        {
            if (landedTransforms.Contains(other) == false)
            {
                standingStill.Add(0f);
                landedTransforms.Add(other);
                landedSended.Add(false);
            }
        }
        else if (other.tag == "RocketKI")
        {
            Debug.Log("Enter");
            Transform transRock = null;
            transRock = other.GetComponentInChildren<RocketController>().transform;
            if (landedTransforms.Contains(transRock) == false)
            {
                standingStill.Add(0f);
                landedTransforms.Add(transRock);
                landedSended.Add(false);
            }
        }
    }
}
