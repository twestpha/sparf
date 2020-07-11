using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerComponent : MonoBehaviour {

    public Rigidbody playerRigidBody;

    public float distanceOffset;
    public float heightOffset;
    public float viewHeightOffset;

    public float positionSeekTime;
    public float viewSeekSpeed;

    private Vector3 cameraVelocity;

    void Update(){
        // Get ball velocity
        Vector3 ballVelocity = playerRigidBody.velocity;
        ballVelocity.Normalize();

        // Find position behind that and up using the offsets
        Vector3 cameraTargetPosition = playerRigidBody.transform.position - (ballVelocity * distanceOffset);
        cameraTargetPosition.y += heightOffset;

        // Seek this position and look at ball over time
        transform.position = Vector3.SmoothDamp(transform.position, cameraTargetPosition, ref cameraVelocity, positionSeekTime);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation((playerRigidBody.transform.position + Vector3.up * viewHeightOffset) - transform.position),
            Time.deltaTime * viewSeekSpeed
        );

    }
}
