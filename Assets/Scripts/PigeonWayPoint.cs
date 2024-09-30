using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PigeonWayPoint : MonoBehaviour
{
    public GameObject[] wayPoints;
    public float speed;
    public Animator anim;
    public BoxCollider2D collider;

    private int _currentWayPointIndex = 0;
    public bool shouldMove = false;


    private void Start()
    {
        anim = GetComponent<Animator>();
        collider.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            WayPoint();
            shouldMove = true;
            anim.SetTrigger("fly");
        }
    }


    public void WayPoint()
    {
        if (Vector2.Distance(wayPoints[_currentWayPointIndex].transform.position, transform.position) < .1f)
        {
            _currentWayPointIndex++;

            if (_currentWayPointIndex >= wayPoints.Length)
            {
                _currentWayPointIndex = 0;
            }

            transform.position = Vector2.MoveTowards(transform.position,
                wayPoints[_currentWayPointIndex].transform.position, speed * Time.deltaTime);
        }
    }
}