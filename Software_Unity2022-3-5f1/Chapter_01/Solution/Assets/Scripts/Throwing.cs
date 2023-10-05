using System.Collections;
using UnityEngine;

public class Throwing : MonoBehaviour
{
    public GameObject throwablePrefab;
    private GameObject currentThrowable;
    public GameObject player;
    Transform playerPos;
    bool canThrow = true; // Initialize to true to allow the first throw.
    bool isThrowing = false; // Flag to check if a throw is in progress.
    bool isThrowned=true;
    public float throwCooldown = 2.0f; // Adjust this to set the cooldown time between throws.


    private void Update()
    {
        playerPos = player.transform;

        if (throwablePrefab != null && canThrow)
        {
            if (Input.GetMouseButtonDown(0) && !isThrowing && isThrowned)
            {
                StartThrow();
            }
        }

        if (currentThrowable != null && isThrowing && !isThrowned) // Update position only when not throwing.
        {
            currentThrowable.transform.position = playerPos.position;
        }
    }

    public void SpawnThrowable()
    {
        Debug.Log("throwable spawned");
        currentThrowable = Instantiate(throwablePrefab, player.transform);

        // Start the cooldown coroutine.
        StartCoroutine(ThrowCooldown());
    }

    void StartThrow()
    {
        canThrow = false;
        isThrowing = true;
        isThrowned = true;
        SpawnThrowable();
    }

    public void EndThrow()
    {
        canThrow = true;
        isThrowing = false;
        isThrowned = true;
    }

    IEnumerator ThrowCooldown()
    {
        if (isThrowing)
        {
            yield return new WaitForSeconds(throwCooldown);
            canThrow = true; // Allow another throw after the cooldown.
        }
    }
}
