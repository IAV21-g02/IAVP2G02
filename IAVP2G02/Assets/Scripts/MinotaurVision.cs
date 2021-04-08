using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinotaurVision : MonoBehaviour
{
    // Start is called before the first frame update
    private MinotaurMovable mov;
    private Vector3 initDest;

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
           //Se guarda la posicion en el momento en el que empieza a seguir a Teseo
           initDest = other.gameObject.GetComponent<Transform>().position;
           gameObject.GetComponent<SphereCollider>().radius = 15.0f;
       }
    }

    private void OnTriggerStay(Collider other)
    {
        //si choca con el jugador
        if (other.gameObject.tag == "Player")
        {
            Vector3 posTeseo = other.gameObject.GetComponent<Transform>().position;
        
            //A partir de la posicion en la que comienza a seguir
            //si Teseo cambia de posición, entonces se recalcula el camino
            if (posTeseo != initDest) {
                mov.AddPath();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
      
        //si choca con el jugador
        if (other.gameObject.tag == "Player")
        {
            mov.startFollow(false);
            gameObject.GetComponent<SphereCollider>().radius = 2.5f;
        }
    }

}
