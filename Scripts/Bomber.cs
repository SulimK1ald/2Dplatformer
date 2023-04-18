using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour
{

    public GameObject bullet;
    public Transform shoot;
    public float timeShoot = 4f;

    void Start()
    {
        shoot.transform.position = new Vector3(transform.position.x, transform.position.y -1f, transform.position.z);
        StartCoroutine(Shooting());
    }

    void Update()
    {
        
    }

    IEnumerator Shooting()
    {
        yield return new WaitForSeconds(timeShoot);
        Instantiate(bullet, shoot.transform.position, transform.rotation);
        StartCoroutine(Shooting());

    }
}