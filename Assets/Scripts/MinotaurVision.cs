using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinotaurVision : MonoBehaviour
{
    // Start is called before the first frame update
    private MinotaurMovable mov;
    private void Start()
    {
        mov = this.GetComponentInParent<MinotaurMovable>();
    }

    private void OnTriggerEnter(Collider other)
    {
      
        //si choca con el jugador
        if (other.gameObject.tag == "Player")
        {
            mov.startFollow(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
      
        //si choca con el jugador
        if (other.gameObject.tag == "Player")
        {
            mov.startFollow(false);
        }
    }


}
