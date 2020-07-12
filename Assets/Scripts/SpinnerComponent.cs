using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerComponent : MonoBehaviour {
    void OnCollisionEnter(Collision collision){
        if(collision.collider.tag == "Player"){
            PlayerTrackSpawnComponent.instance.DiscardAllCards();
        }
    }
}
