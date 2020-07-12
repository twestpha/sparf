using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContactComponent : MonoBehaviour {
    public float deathTime;
    private float timeSinceLastContact;
    private int trackLayerMask;

    void Start() {
        timeSinceLastContact = 0;
        trackLayerMask = 1 << LayerMask.NameToLayer("Track");
    }

    void Update(){
        // Keep track of last time contacting the ground
        // If too much time (2 seconds?) until last time contacted, fade out and game over sequence
        // Also stop counting if velocity is very very low

        // Spherecast with length 0 or very small
        // Filter cast with 1 << Track layer
        // Use Time.time for last hit time

        // if last hit time > x,
        // call ResetGame() on PlayerContactComponent instance


        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 0, transform.forward, out hit, 3, trackLayerMask)) {
            timeSinceLastContact = 0;
        } else {
            timeSinceLastContact += Time.deltaTime;
        }

        if (timeSinceLastContact >= deathTime) {
            timeSinceLastContact = -50;
            PlayerTrackSpawnComponent.instance.ResetGame();
        }
    }
}
