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
    public float objectLifetime = 5.0f;
    public float arrowEndPoint;

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

        float throwPower = mouseDelta.magnitude * sensitivity;
        throwPower = Mathf.Clamp(throwPower, 0.0f, maxThrowDistance);
        throwVector = -mouseDelta.normalized * throwPower;
    }

    void SetArrow()
    {
        _lr.positionCount = 2;
        _lr.SetPosition(0, transform.position);
        _lr.SetPosition(1, transform.position + throwVector / arrowEndPoint);
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

        transform.parent = null;
        _rb.AddForce(throwVector * 20);
        GetComponent<TrailRenderer>().emitting = true;
        StartCoroutine(StopAtEndPoint(throwDistance));
    }

    IEnumerator StopAtEndPoint(float throwDistance)
    {
        yield return new WaitForSeconds(throwDistance / 5f);

        _rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(objectLifetime);
        Destroy(gameObject);

        // Notify the Throwing script that the throw is completed.
        FindObjectOfType<Throwing>().EndThrow();
    }

    private void OnTriggerEnter2D(Collider2D theCollider)
    {
        string tag = theCollider.gameObject.tag;
        CollectableInventory inventory = GameObject.FindWithTag("Inventory").GetComponent<CollectableInventory>();

        if (tag == "Collectable")
        {
            if (inventory.numObjects < inventory.maxObjects)
            {
                inventory.AddItem();
                Destroy(theCollider.gameObject);
            }
        }

        if (tag == "Knight")
        {
            Destroy(theCollider.gameObject);
        }
    }
}