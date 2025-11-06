using UnityEngine;

public class GroundSpeed : MonoBehaviour
{
    public FloatVariable DefaultSpeed;
    public FloatVariable PlayerSpeed; 

    private SurfaceEffector2D surfaceEffector2D;

    void Start()
    {
        this.surfaceEffector2D = this.GetComponent<SurfaceEffector2D>();
        this.surfaceEffector2D.speed = this.DefaultSpeed.Value;
        PlayerSpeed.Value = this.surfaceEffector2D.speed;
    }

    void Update()
    {
        PlayerSpeed.Value = this.surfaceEffector2D.speed; 
    }

    public float GetGroundSpeed()
    {
        return this.surfaceEffector2D.speed;
    }

    public float SetGroundSpeed(float groundSpeed)
    {
        PlayerSpeed.Value = groundSpeed;
        return this.surfaceEffector2D.speed = groundSpeed;
    }
}
