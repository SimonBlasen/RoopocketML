using UnityEngine;
using System.Collections;

public class SplineLookat : MonoBehaviour {

    public bool enableLookAt;
    public bool enableMoveTo;
    public bool enableMoveToLerp;
    public GameObject lookAt;
    public float moveSpeed = 1f;
    public float moveSpeedLerp = 0.12f;
    public GameObject moveTo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //transform.rotation = Quaternion.LookRotation(lookAt.transform.position - transform.position, new Vector3(0, 1, 0));
        if (enableLookAt && lookAt != null)
        {
            transform.LookAt(lookAt.transform);
        }

        if (enableMoveTo && moveTo != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTo.transform.position, Time.deltaTime * moveSpeed);
        }

        if (enableMoveToLerp && moveTo != null)
        {
            transform.position = Vector3.Lerp(transform.position, moveTo.transform.position, moveSpeedLerp);
        }
    }
}
