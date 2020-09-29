using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public float risingSpeed;
    public float deviationRange;
    private TextMeshPro tmp;
    private Rigidbody2D rb2d;

    void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void SetDamageVisual(int damage, bool heal)
    {
        if (heal)
        {
            tmp.SetText((-damage).ToString());
            tmp.color = new Color32(42, 255, 85, 255);
        }
        else
        {
            tmp.SetText(damage.ToString());
            tmp.color = new Color32(255, 0, 0, 255);
        }

        rb2d.velocity = new Vector3(Random.Range(deviationRange, -deviationRange), risingSpeed);

        Invoke("Seppuku", 1f);
    }

    // Update is called once per frame
    //void Update()
    //{
        //Make damage number float up.
        //transform.position += new Vector3(0f, 1f) * Time.deltaTime;
    //}

    void Seppuku()
    {
        Destroy(this.gameObject);
    }
}
