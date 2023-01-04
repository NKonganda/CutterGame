using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    private int value;
    public bool missed = false;
    public void SetSize(int value) {
        this.value = value;
        Renderer rend = gameObject.GetComponent<Renderer>();
        Material mat = rend.material;
        if(value == 1) {
            mat.color = new Color(0, 1, 0);
            transform.localScale = new Vector3(0.35f,0.05f,0.37f);
        }
        else if(value == 2) {
            mat.color = new Color(0, 0, 1);
            transform.localScale = new Vector3(0.35f,0.04f,0.37f);
        }
        else if(value == 3) {
            mat.color = new Color(1, 0.4117647f, 0.7058824f);
            transform.localScale = new Vector3(0.35f,0.03f,0.37f);
        }
        else if(value == 4) {
            mat.color = new Color(1, 0, 0);
            transform.localScale = new Vector3(0.35f,0.02f,0.37f);
        }
        else if(value == 5) {
            mat.color = new Color(1, 0.8588235f, 0.3176471f);
            transform.localScale = new Vector3(0.35f,0.01f,0.37f);
        }
    }
    public int GetValue() {
        return value;
    }
}
