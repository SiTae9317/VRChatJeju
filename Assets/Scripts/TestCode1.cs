using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.eulerAngles += Vector3.up * Time.deltaTime * 10.0f;
    }
}
