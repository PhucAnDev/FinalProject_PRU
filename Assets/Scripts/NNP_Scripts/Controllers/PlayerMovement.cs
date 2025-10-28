using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject Ground;

    public FloatVariable TorqueForce;
    public FloatVariable SpeedNormalizationRate;
    public FloatVariable MinSpeed;
    public FloatVariable MaxSpeed;
    public FloatVariable AccelerationRate;
    public FloatVariable DefaultSpeed;
    public BoolVariable IsAlive;

    private Rigidbody2D rigidBody;
    private GroundSpeed groundSpeed;

    private float inputVertical;
    private float inputHorizontal;

    // 🔥 Biến cheat toàn cục
    public static bool IsCheatModeActive = false;
    private bool isCheatMode = false;

    // 🧩 Biến theo dõi chuỗi phím cheat (P → O → I)
    private int cheatProgress = 0;

    private void Awake()
    {
        this.rigidBody = this.GetComponent<Rigidbody2D>();
        this.groundSpeed = this.Ground.GetComponent<GroundSpeed>();
        this.IsAlive.SetValue(true);
    }

    private void Update()
    {
        if (this.IsAlive.Value)
        {
            this.inputVertical = Input.GetAxisRaw("Vertical");
            this.inputHorizontal = Input.GetAxisRaw("Horizontal");
        }

        // 🧩 Cheat combo: phải bấm đúng thứ tự P -> O -> I
        if (Input.GetKeyDown(KeyCode.P) && cheatProgress == 0)
        {
            cheatProgress = 1;
            Debug.Log("Cheat sequence started: P ✅");
        }
        else if (Input.GetKeyDown(KeyCode.O) && cheatProgress == 1)
        {
            cheatProgress = 2;
            Debug.Log("Cheat sequence continued: PO ✅");
        }
        else if (Input.GetKeyDown(KeyCode.I) && cheatProgress == 2)
        {
            cheatProgress = 0; // reset
            ToggleCheat();
        }
        // Nếu bấm sai phím khác => reset chuỗi
        else if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.P) &&
                 !Input.GetKeyDown(KeyCode.O) && !Input.GetKeyDown(KeyCode.I))
        {
            cheatProgress = 0;
        }
    }

    private void ToggleCheat()
    {
        isCheatMode = !isCheatMode;
        IsCheatModeActive = isCheatMode;

        if (isCheatMode)
        {
            Debug.Log("🔥 CHEAT MODE ACTIVATED: Invincible + Max Speed!");
            this.IsAlive.SetValue(true);
            this.groundSpeed.SetGroundSpeed(this.MaxSpeed.Value);
        }
        else
        {
            Debug.Log("❄️ CHEAT MODE DEACTIVATED");
        }
    }

    private void FixedUpdate()
    {
        float defaultSpeed = this.IsAlive.Value ? this.DefaultSpeed.Value : 0;

        // Player rotation
        this.rigidBody.AddTorque(this.inputHorizontal * this.TorqueForce.Value);

        // User-input induced speed change
        if (this.inputVertical != 0)
        {
            float oldSpeed = this.groundSpeed.GetGroundSpeed();
            this.groundSpeed.SetGroundSpeed(this.getUpdatedSpeed(
                this.inputVertical,
                this.AccelerationRate.Value,
                this.groundSpeed.GetGroundSpeed(),
                this.MinSpeed.Value,
                this.MaxSpeed.Value
            ));
        }
        // Speed normalization
        else if (!this.isDefaultSpeed(this.groundSpeed.GetGroundSpeed(), defaultSpeed))
        {
            float oldSpeed = this.groundSpeed.GetGroundSpeed();
            this.groundSpeed.SetGroundSpeed(getNormalizedSpeed(
                this.groundSpeed.GetGroundSpeed(),
                defaultSpeed,
                this.SpeedNormalizationRate.Value
            ));
        }

        // 🚀 Nếu bật cheat mode: luôn giữ tốc độ max
        if (isCheatMode)
        {
            this.groundSpeed.SetGroundSpeed(this.MaxSpeed.Value);
        }

        // Cleanup
        this.inputVertical = 0;
        this.inputHorizontal = 0;
    }

    private float getUpdatedSpeed(float inputVertical, float accelerationRate, float currentSpeed, float minSpeed, float maxSpeed)
    {
        return Mathf.Min(
            Mathf.Max(minSpeed, currentSpeed + (inputVertical * Time.deltaTime * accelerationRate)),
            maxSpeed
        );
    }

    private bool isDefaultSpeed(float currentSpeed, float defaultSpeed)
    {
        return Mathf.Approximately(currentSpeed, defaultSpeed);
    }

    private float getNormalizedSpeed(float currentSpeed, float defaultSpeed, float speedNormalizationRate)
    {
        float speedDifferential = (currentSpeed - defaultSpeed) / speedNormalizationRate;
        return Mathf.Approximately(defaultSpeed, currentSpeed - speedDifferential)
            ? defaultSpeed
            : currentSpeed - speedDifferential;
    }

    public GroundSpeed GetGroundSpeed()
    {
        return groundSpeed;
    }
}
