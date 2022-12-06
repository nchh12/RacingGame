using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutine : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 2;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(example());
    }
        

    IEnumerator example()
    {
        Debug.Log("~Before yield->Time->" + Time.time.ToString());
        yield return new WaitForSeconds(5f);
        Debug.Log("~After yield->Time->" + Time.time.ToString());
    }
}
