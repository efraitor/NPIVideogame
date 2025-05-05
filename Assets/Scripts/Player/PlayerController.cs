using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Velocidad del jugador
    public float moveSpeed;
    // El rigidbody del jugador
    public Rigidbody2D theRB;
    // El Sprite Renderer
    public SpriteRenderer theSR;


    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        theRB.velocity = new Vector2(moveSpeed * Input.GetAxis("Horizontal"), theRB.velocity.y);

        if(theRB.velocity.x < 0)
            theSR.flipX = true;
        else
            theSR.flipX = false;
    }
}
