using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContactComponent : MonoBehaviour {
    public float deathTime;
    public float raycastDistance = 5;
    public int smaLength = 120;
    public float loopDeathOnSMABelow = 1;

    private float timeSinceLastContact;
    private int trackLayerMask;
    private string currentTrackTag;

    private Rigidbody rigidbody;
    private int velocitySMASampleCount;
    private float velocitySMA;

    void Start() {
        timeSinceLastContact = 0;
        trackLayerMask = 1 << LayerMask.NameToLayer("Track");

        rigidbody = GetComponent<Rigidbody>();
    }

    void Update(){
        if (PlayerTrackSpawnComponent.instance.runningTimer) {
            calcSMA();
            detectCollisions();
            if (shouldReset()) {
                Debug.Log("timeSinceLastContact: " + timeSinceLastContact);
                Debug.Log("currentTrackTag: " + currentTrackTag);
                Debug.Log("velocitySMA: " + velocitySMA);
                reset();
                PlayerTrackSpawnComponent.instance.ResetGame();
            }
        }

    }

    private void calcSMA() {
        velocitySMASampleCount++;
        if (velocitySMASampleCount > smaLength) {
            velocitySMA = velocitySMA + (rigidbody.velocity.magnitude - velocitySMA) / (smaLength + 1);
        } else {
            velocitySMA += rigidbody.velocity.magnitude;

            if (velocitySMASampleCount == smaLength) {
                velocitySMA += velocitySMA / velocitySMASampleCount;
            }
        }
    }

    private void detectCollisions() {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 1.2f, Vector3.up * -1.0f, out hit, raycastDistance, trackLayerMask)) {
                timeSinceLastContact = 0;
                currentTrackTag = hit.collider.tag;
            } else {
                timeSinceLastContact += Time.deltaTime;
                currentTrackTag = "";
            }
    }

    private bool shouldReset() {
        bool isOffTrackTooLong = timeSinceLastContact >= deathTime;
        bool isStalledOnLoop = currentTrackTag == "loop" && velocitySMA <= loopDeathOnSMABelow;
        return isOffTrackTooLong || isStalledOnLoop;
    }

    private void reset() {
        velocitySMA = 0;
        velocitySMASampleCount = 0;
        currentTrackTag = "";
        timeSinceLastContact = float.MinValue;
    }
}
