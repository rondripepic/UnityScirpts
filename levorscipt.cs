using UnityEngine;

public class levorscipt : MonoBehaviour
{
    // Variables
    private bool islevoron = false;
    private SpriteRenderer spriteRenderer;

    public Sprite offlever;
    public Sprite onlever;
    public bool IsOnLevor = false;
    [Header("object being affected and how")]
    public GameObject AffectedObject;
    public string howareweaffectingtheobject;
    private bool shouldRotate = false;
    private bool shouldRotateBack = false;
    [Header("only for roatation. how it works angle is how far you are turning the object.")]
    [Header("set max rotation detal the the same as angle so it works.")]
    public float angle; 
    public float maxRotationDelta = 90f; 

    private float startZRotation;
    private float rotatedAmount;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void leverflip()
    {
        islevoron = !islevoron;
        if (!islevoron)
        {
            spriteRenderer.sprite = offlever;
            if (howareweaffectingtheobject == "transparent") AffectedObject.SetActive(true);
            if (howareweaffectingtheobject == "roatations")
            {
                shouldRotate = false;
                shouldRotateBack = true;
            }
        }
        else
        {
            spriteRenderer.sprite = onlever;
            if (howareweaffectingtheobject == "transparent") AffectedObject.SetActive(false);
            if (howareweaffectingtheobject == "roatations")
            {
                shouldRotate = true;
                shouldRotateBack = false;
                // Store the starting rotation and reset rotated amount
                startZRotation = AffectedObject.transform.eulerAngles.z;
                rotatedAmount = 0f;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && IsOnLevor)
        {
            if (!islevoron && Mathf.Approximately(rotatedAmount, 0f))
            {
                leverflip();
            }
            else if (islevoron && Mathf.Approximately(rotatedAmount, maxRotationDelta))
            {
                leverflip();
            }
        }

        if (shouldRotate && howareweaffectingtheobject == "roatations")
        {
            float rotationStep = angle * Time.deltaTime;
            if (rotatedAmount + Mathf.Abs(rotationStep) < Mathf.Abs(maxRotationDelta))
            {
                AffectedObject.transform.Rotate(0f, 0f, rotationStep);
                rotatedAmount += Mathf.Abs(rotationStep);
            }
            else
            {
                // Clamp to max rotation
                float remaining = Mathf.Abs(maxRotationDelta) - rotatedAmount;
                float direction = Mathf.Sign(rotationStep);
                remaining = Mathf.Round(remaining);
                AffectedObject.transform.Rotate(0f, 0f, remaining * direction);
                shouldRotate = false; // Stop rotating
                rotatedAmount = maxRotationDelta; // At the peak
            }
        }

        if (shouldRotateBack && howareweaffectingtheobject == "roatations")
        {
            float rotationStep = angle * Time.deltaTime;
            if (rotatedAmount - Mathf.Abs(rotationStep) > 0f)
            {
                AffectedObject.transform.Rotate(0f, 0f, -rotationStep);
                rotatedAmount -= Mathf.Abs(rotationStep);
            }
            else
            {
                // Clamp back to original rotation
                float remaining = rotatedAmount;
                remaining = Mathf.Round(remaining);
                AffectedObject.transform.Rotate(0f, 0f, -remaining);
                shouldRotateBack = false; // Stop rotating back
                rotatedAmount = 0f;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsOnLevor = !IsOnLevor;
            print("enter");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IsOnLevor = !IsOnLevor;
        print("leave");
    }
}
