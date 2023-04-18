using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float speed = 4f;
    float TimeToDisabled = 10f;

    void Start()
    {
        StartCoroutine(SetDisabled());
    }

    
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
    IEnumerator SetDisabled()
    {
        yield return new WaitForSeconds(TimeToDisabled);
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopCoroutine(SetDisabled());
        gameObject.SetActive(false);
    }
}
