using UnityEngine;

public class standingscripotgoon : MonoBehaviour
{
    // Variables \\
    // scripts \\
    public Movement movement;

    // floats \\
    public float degreeofstanding;

    // gameobjects \\
    public GameObject ground;
    public GameObject character;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(ground != null)
        {
            degreeofstanding = ground.transform.eulerAngles.z;
        }
        float zRotation = character.transform.eulerAngles.z;
        if (movement.isGrounded && Mathf.Abs(zRotation - degreeofstanding) > 0.01f)
        {
            ground.transform.rotation = Quaternion.Euler(0f, 0f, degreeofstanding);
            character.transform.rotation = Quaternion.Euler(0f, 0f, degreeofstanding);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform"))
        {
            ground = collision.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform"))
        {
            ground = null;
            degreeofstanding = 0f;
        }
    }
}
