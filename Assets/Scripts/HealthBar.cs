using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    Vector3 originalScale;
    private GameObject fill;
    private GameObject scale;
    private static float currentHealth = 100;
    private static float maxHealth = 100;
    // Start is called before the first frame update
    void Start()
    {
        fill = GameObject.Find("HealthBarFill");
        scale = GameObject.Find("HealthBarScale");
        originalScale = scale.transform.localScale;
        fill.GetComponent<Renderer>().material.color = new Color(0,1,0);
    }

    // Update is called once per frame
    void Update()
    {
        float healthPercent = currentHealth/maxHealth;
        scale.transform.localScale = new Vector3(originalScale.x,originalScale.y,healthPercent*originalScale.z);
        Color currentColor = new Color(Mathf.Lerp(1,0,healthPercent),Mathf.Lerp(0,1,healthPercent),0);
        fill.GetComponent<Renderer>().material.color = currentColor;
    }
    public static void SetCurrentHealth(float current) {
        if(current > maxHealth) {
            current = maxHealth;
        }
        if(current < 0) {
            current = 0;
        }
        currentHealth = current;
    }
    public static float GetCurrentHealth() {
        return currentHealth;
    }
    public static void Reset() {
        currentHealth = 100;
    }
}
