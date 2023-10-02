using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Throwing : MonoBehaviour
{
    public Transform firePoint;
    public GameObject throwablePrefab;
    public Transform playerPos;

    private void Start()
    {
        firePoint.position = playerPos.position;
        SpawnThrowable();
    }

    private void Update()
    {
        Debug.Log(firePoint.position);

    }

    public void SpawnThrowable()
    {

        GameObject throwableObj = Instantiate(throwablePrefab, firePoint.position, firePoint.rotation);
    }
}
