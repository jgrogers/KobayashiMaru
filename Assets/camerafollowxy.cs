using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerafollowxy : MonoBehaviour
{
    [SerializeField] private GameObject toFollow;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (toFollow != null)
        {
            transform.position = new Vector3(toFollow.transform.position.x, transform.position.y, toFollow.transform.position.z);
        }
    }
}
