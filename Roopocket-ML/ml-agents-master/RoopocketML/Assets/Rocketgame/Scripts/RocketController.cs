using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RocketController : MonoBehaviour {

    [SerializeField]
    private KeyCode[] keyCodes;
    [SerializeField]
    private KeyCode keyLander;
    [SerializeField]
    private Transform[] thrustPositions;
    [SerializeField]
    private ParticleSystem[] thrustParticles;
    [SerializeField]
    private Light[] thrustLights;
    [SerializeField]
    private float thrustStrength = 10.0f;
    [SerializeField]
    private Transform midPoint;
    [SerializeField]
    private RocketProps rocketProps;
    [SerializeField]
    private LanderMover[] landerMovers;
    [SerializeField]
    private bool kiRocket = false;
    [SerializeField]
    private ParticleSystem[] thrustParticles0;
    [SerializeField]
    private ParticleSystem[] thrustParticles1;
    [SerializeField]
    private ParticleSystem[] thrustParticles2;
    [SerializeField]
    private ParticleSystem[] thrustParticles3;

    private bool[] thrusts = null;

    private Rigidbody ownRig;

	// Use this for initialization
	void Start ()
    {
        Init();

        //Only for ML Learning
        for (int i = 0; i < landerMovers.Length; i++)
        {
            landerMovers[i].TurnOut = false;
        }
    }

    private void Update()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(ownRig.angularVelocity);
        localVelocity.x = 0f;
        localVelocity.y = 0f;

        ownRig.angularVelocity = transform.TransformDirection(localVelocity);

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (rocketProps.OutOfFuel == false)
        {
            for (int i = 0; i < thrustPositions.Length; i++)
            {
                if (useFloatThrusts)
                {
                    ownRig.AddForceAtPosition(transform.up * thrustStrength * (thrustsFloat[i] + 0.7f < 0f ? 0f : (thrustsFloat[i] + 0.7f) / 1.7f), thrustPositions[i].position);
                }
                else
                {
                    if (thrusts[i])
                    {
                        ownRig.AddForceAtPosition(transform.up * thrustStrength, thrustPositions[i].position);
                    }
                }
            }

            if (IsKI == false)
            {
                for (int i = 0; i < keyCodes.Length; i++)
                {
                    if (Input.GetKey(keyCodes[i]))
                    {
                        SetThrust(i, true);
                    }
                    else
                    {
                        SetThrust(i, false);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < keyCodes.Length; i++)
            {
                SetThrust(i, false);
            }
        }

        if (Input.GetKeyDown(keyLander) && IsKI == false)
        {
            for (int i = 0; i < landerMovers.Length; i++)
            {
                landerMovers[i].TurnOut = !landerMovers[i].TurnOut;
            }
        }


        float angleAbs = Vector3.Angle((new Vector3(0f, 0f, 1f)), transform.forward);
        if (Vector3.Angle((new Vector3(1f, 0f, 0f)), transform.forward) > 90f)
        {
            angleAbs = 360f - angleAbs;
        }

        angleAbs = angleAbs / 90f;
        while (angleAbs < 0f)
        {
            angleAbs += 4f;
        }
        angleAbs += 0.5f;
        angleAbs = ((int)angleAbs);
        angleAbs *= 90f;

        AbsoluteYAngle = angleAbs;


        if (!Turning)
        {
            

            bool zNull = false;
            if ((angleAbs >= 45f && angleAbs < 135f) || (angleAbs >= 225f && angleAbs < 315f))
            {
                zNull = false;
            }

            transform.rotation = Quaternion.Euler(zNull ? transform.rotation.eulerAngles.x : 0f, angleAbs, zNull ? 0f : transform.rotation.eulerAngles.z);
        }


        if (kiRocket)
        {
            if (useFloatThrusts == false)
            {
                for (int i = 0; i < thrusterAverageMoving.Length; i++)
                {
                    float avg = thrusterAverage(i);
                    EmissionModule em0 = thrustParticles0[i].emission;
                    em0.rateOverTime = 100f * avg;
                    EmissionModule em1 = thrustParticles1[i].emission;
                    em1.rateOverTime = 100f * avg;
                    EmissionModule em2 = thrustParticles2[i].emission;
                    em2.rateOverTime = 100f * avg;
                    EmissionModule em3 = thrustParticles3[i].emission;
                    em3.rateOverTime = 500f * avg;
                }
            }
            else
            {
                for (int i = 0; i < thrusterAverageMoving.Length; i++)
                {
                    EmissionModule em0 = thrustParticles0[i].emission;
                    em0.rateOverTime = 100f * (thrustsFloat[i] + 0.7f < 0f ? 0f : (thrustsFloat[i] + 0.7f) / 1.7f);
                    EmissionModule em1 = thrustParticles1[i].emission;
                    em1.rateOverTime = 100f * (thrustsFloat[i] + 0.7f < 0f ? 0f : (thrustsFloat[i] + 0.7f) / 1.7f);
                    EmissionModule em2 = thrustParticles2[i].emission;
                    em2.rateOverTime = 100f * (thrustsFloat[i] + 0.7f < 0f ? 0f : (thrustsFloat[i] + 0.7f) / 1.7f);
                    EmissionModule em3 = thrustParticles3[i].emission;
                    em3.rateOverTime = 100f * (thrustsFloat[i] + 0.7f < 0f ? 0f : (thrustsFloat[i] + 0.7f) / 1.7f);
                }
            }
        }
    }

    public bool LandingMoversOut
    {
        get
        {
            return landerMovers[0].TurnOut;
        }
        set
        {
            for (int i = 0; i < landerMovers.Length; i++)
            {
                landerMovers[i].TurnOut = value;
            }
        }
    }

    private void Init()
    {
        ownRig = GetComponent<Rigidbody>();
        if (midPoint != null)
        {
            ownRig.centerOfMass = midPoint.localPosition;
        }
        thrusts = new bool[thrustPositions.Length];
        for (int i = 0; i < thrusts.Length; i++)
        {
            thrusts[i] = false;
        }

        Normal = new Vector3(0f, 0f, -1f);
        Turning = false;

        Manager.Instance.ActivateManager();

        if (kiRocket)
        {
            for (int i = 0; i < thrusterAverageMoving.Length; i++)
            {
                thrusterAverageMoving[i] = new bool[32];
            }

            for (int i = 0; i < thrustParticles.Length; i++)
            {
                thrustParticles[i].Play();
            }

            for (int i = 0; i < thrCountIndexes.Length; i++)
            {
                thrCountIndexes[i] = 0;
            }
        }
    }

    public float AbsoluteYAngle
    {
        protected set;
        get;
    }

    public bool IsKI
    {
        get
        {
            return kiRocket;
        }
    }

    public bool Turning
    {
        get;set;
    }

    public Vector3 Normal
    {
        get; set;
    }

    public bool[] Thrusts
    {
        get
        {
            return thrusts;
        }
    }

    private float[] thrustsFloat = new float[4];

    public void SetThrustFloat(int index, float val)
    {
        if (thrusts == null)
        {
            Init();
        }
        if (index >= 0 && index < thrustPositions.Length)
        {
            thrustsFloat[index] = val;
        }
    }


    private bool useFloatThrusts = false;
    public bool UseFloatThrusts
    {
        get
        {
            return useFloatThrusts;
        }
        set
        {
            useFloatThrusts = value;
        }
    }

    public void SetThrust(int index, bool on)
    {
        if (thrusts == null)
        {
            Init();
        }
        if (index >= 0 && index < thrustPositions.Length)
        {
            if (!kiRocket)
            {
                if (on)
                {
                    //thrustLights[index].intensity = 5f;
                    thrustParticles[index].Play();
                }
                else
                {
                    //thrustLights[index].intensity = 0f;
                    thrustParticles[index].Stop();
                }
            }

            if (kiRocket)
            {
                sampleThrusterAverage(index);
            }

            thrusts[index] = on;
        }
    }


    private int[] thrCountIndexes = new int[4];
    private bool[][] thrusterAverageMoving = new bool[4][];
    private float thrusterAverage(int thrusterIndex)
    {
        float movAvg = 0f;
        for (int i = 0; i < thrusterAverageMoving[thrusterIndex].Length; i++)
        {
            movAvg += thrusterAverageMoving[thrusterIndex][i] ? 1f : 0f;
        }
        
        return movAvg / thrusterAverageMoving[thrusterIndex].Length;
    }

    private void sampleThrusterAverage(int index)
    {
        thrusterAverageMoving[index][thrCountIndexes[index]] = thrusts[index];

        thrCountIndexes[index]++;
        thrCountIndexes[index] = thrCountIndexes[index] % thrusterAverageMoving[0].Length;
    }

    private void sampleThrusterAverage()
    {
        for (int i = 0; i < thrusterAverageMoving.Length; i++)
        {
            thrusterAverageMoving[i][thrCountIndexes[i]] = thrusts[i];
            


            thrCountIndexes[i]++;
            thrCountIndexes[i] = thrCountIndexes[i] % thrusterAverageMoving[0].Length;
        }
    }
}
