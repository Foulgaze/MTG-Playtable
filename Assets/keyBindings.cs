using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyBindings : MonoBehaviour
{
    public playtableGenerator playtableGenerator;
    // Start is called before the first frame update


    KeyCode expandField = KeyCode.T;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(expandField))
        {
            //playtableGenerator.expandTable();
        }
    }
}
