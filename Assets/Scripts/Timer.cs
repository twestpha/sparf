using UnityEngine;

public class Timer {
    private float duration;
    private float startTime;
    private bool finishedLastFrame;

    public Timer(){
        duration = 0.0f;
    }

    public Timer(float duration_){
        duration = duration_;
        // Timer starts "finished"
        startTime = -duration;
    }

    public void Start(){
        finishedLastFrame = false;
        startTime = Time.time;
    }

    public float Elapsed(){
        return Time.time - startTime;
    }

    public float Parameterized(){
        return Mathf.Max(Mathf.Min(Elapsed() / duration, 1.0f), 0.0f);
    }

    public bool Finished(){
        return Elapsed() >= duration;
    }

    public bool FinishedThisFrame(){
        if(!finishedLastFrame && Finished()){
            finishedLastFrame = true;
            return true;
        }

        return false;
    }

    public void SetParameterized(float value){
        startTime = Time.time - (value * duration);
    }

    public void SetDuration(float duration_){
        duration = duration_;
    }
};
