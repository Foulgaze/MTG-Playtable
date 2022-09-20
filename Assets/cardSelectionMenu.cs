using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardSelectionMenu : MonoBehaviour
{
    // Start is called before the first frame update
    cardPlaying handController;
    void Start()
    {
        handController = transform.GetComponent<cardPlaying>();
    }


}
