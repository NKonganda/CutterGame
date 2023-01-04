using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPiece : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Delete",3);
    }
    void Delete() {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
