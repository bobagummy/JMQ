using System.Collections;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    Vector3 throwVector;
    Rigidbody2D _rb;
    LineRenderer _lr;
    public float sensitivity;
    public float maxThrowDistance;
    private Vector3 initialMousePos;
    public float objectLifetime = 5.0f; // Adjust this to set the lifetime of the object.

    void Start()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _lr = this.GetComponent<LineRenderer>();
    }

    void OnMouseDown()
    {
        initialMousePos = Input.mousePosition;
        CalculateThrowVector();
        SetArrow();
    }

    void OnMouseDrag()
    {
        CalculateThrowVector();
        SetArrow();
    }

    void CalculateThrowVector()
    {
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 mouseDelta = currentMousePos - initialMousePos;

        // Calculate the throw power based on the length of the line renderer.
        float throwPower = mouseDelta.magnitude * sensitivity;

        // Limit the throw power to be within the max throw distance.
        throwPower = Mathf.Clamp(throwPower, 0.0f, maxThrowDistance);

        // Calculate the throw direction.
        throwVector = -mouseDelta.normalized * throwPower;
    }

    void SetArrow()
    {
        _lr.positionCount = 2;
        _lr.SetPosition(0, transform.position);
        _lr.SetPosition(1, transform.position + throwVector / 5);
        _lr.enabled = true;
    }

    void OnMouseUp()
    {
        RemoveArrow();
        Throw();
    }

    void RemoveArrow()
    {
        _lr.enabled = false;
    }

    public void Throw()
    {
        float throwDistance = throwVector.magnitude;

        if (throwDistance > maxThrowDistance)
        {
            throwDistance = maxThrowDistance;
            throwVector = throwVector.normalized * maxThrowDistance;
        }

        _rb.AddForce(throwVector * 10);

        StartCoroutine(StopAtEndPoint(throwDistance));
    }

    IEnumerator StopAtEndPoint(float throwDistance)
    {
        yield return new WaitForSeconds(throwDistance / 10f);

        _rb.velocity = Vector2.zero;

        // Delay for objectLifetime seconds and then destroy the object.
        yield return new WaitForSeconds(objectLifetime);
        Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D theCollider)
    {
        string tag = theCollider.gameObject.tag;
        CollectableInventory inventory = GameObject.FindWithTag("Inventory").GetComponent<CollectableInventory>();

        if (tag == "Collectable")
        {
            if (inventory.numObjects < inventory.maxObjects)
            {
                // Add the collectable to the inventory.
                inventory.AddItem();

                // Destroy the collectable object.
                Destroy(theCollider.gameObject);
            }
        }
    }

}
