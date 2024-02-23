using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplerBoxController : MonoBehaviour
{
    public int multipler;
    [SerializeField] private TextMeshPro multiplerText; 
    // Start is called before the first frame update
    void Start()
    {
        multiplerText.text = "X " + multipler;
    }

  
}
