using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerComponent : MonoBehaviour {

    public Rigidbody playerRigidBody;

    public float distanceOffset;
    public float heightOffset;
    public float viewHeightOffset;

    public float positionSeekTime;
    public float viewSeekTime;

    private Vector3 cameraPositionVelocity;

    private Vector3 cameraLookTarget;
    private Vector3 cameraLookVelocity;

    void Update(){
        // Get ball velocity
        Vector3 ballVelocity = playerRigidBody.velocity;
        ballVelocity.Normalize();

        // Find position behind that and up using the offsets
        Vector3 cameraTargetPosition = playerRigidBody.transform.position - (ballVelocity * distanceOffset);
        cameraTargetPosition.y += heightOffset;

        // Seek position and look-at over time
        transform.position = Vector3.SmoothDamp(transform.position, cameraTargetPosition, ref cameraPositionVelocity, positionSeekTime);
        cameraLookTarget = Vector3.SmoothDamp(cameraLookTarget, playerRigidBody.transform.position + Vector3.up * viewHeightOffset, ref cameraLookVelocity, viewSeekTime);
        transform.LookAt(cameraLookTarget);
    }
}
