using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody2D rigid;
    public float  movespeed;
    public float pullForce = 100f;
    public float rotateSpeed = 360f;
    private GameObject closestTower;
    private GameObject hookedTower;
    private bool isPulled = false;
    public UI uiControl;
    public AudioSource myAudio;
    private bool isCrashed = false;
    private Vector2 startposition;
        private Vector3 startrotation;

    // Start is called before the first frame update
    void Start()
    {
        startposition = this.transform.position;
        startrotation = this.transform.localEulerAngles;
        rigid = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!uiControl.stop)
        {
            if (isCrashed)
            {
                if (!myAudio.isPlaying)
                {
                    restartPosition();
                }
            }
            else
            {
                rigid.velocity = -transform.up * movespeed;
            }

            if (Input.GetMouseButton(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if ((hit.collider.tag == "tomboltower" && !isPulled) && closestTower != null)
                {
                    if (hit.transform.parent.name == closestTower.transform.name)
                    {
                        if (closestTower != null && hookedTower == null)
                        {
                            hookedTower = closestTower;
                        }
                        if (hookedTower)
                        {
                            float distance = Vector2.Distance(transform.position, hookedTower.transform.position);
                            Vector3 pullDirection = (hookedTower.transform.position - transform.position).normalized;
                            float newPullForce = Mathf.Clamp(pullForce / distance, 20, 50);
                            rigid.AddForce(pullDirection * newPullForce);
                            rigid.angularVelocity = -rotateSpeed / distance;
                            isPulled = true;
                        }
                    }
                }

            }
            else
            {
                rigid.angularVelocity = 0;
                isPulled = false;
                hookedTower = null;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (!isCrashed)
            {
                myAudio.Play();
                rigid.velocity = new Vector3(0f, 0f, 0f);
                rigid.angularVelocity = 0f;
                isCrashed = true;
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("selesai");
            uiControl.endGame();
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tower")
        {
            closestTower = collision.gameObject;
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isPulled) return;

        if (collision.gameObject.tag == "Tower")
        {
            closestTower = null;
            hookedTower = null;
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

  


    public void restartPosition()
    {
        this.transform.position = startposition;
        this.transform.rotation = Quaternion.Euler(startrotation);
        
        isCrashed = false;

        if (closestTower)
        {
            closestTower.GetComponent<SpriteRenderer>().color = Color.white;
            closestTower = null;
        }

    }
}
