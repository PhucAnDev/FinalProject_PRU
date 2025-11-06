using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundedTrigger : MonoBehaviour
{
    [Tooltip("Event invoked when player comes in contact with ground.")]
    public UnityEvent PlayerGroundedEvent;

    [Tooltip("Event invoked when player comes out of contact with ground.")]
    public UnityEvent PlayerAirbornEvent;

    [Tooltip("GameObjects to interact with.")]
    public GameObject[] TriggerCandidates;

    [Header("Jump Reset Settings")]
    public IntVariable RemainingJumps; // ScriptableObject ho?c bi?n dùng chung
    public int MaxJumps = 3; // reset v? 3 l?n nh?y khi ch?m ??t

    private HashSet<GameObject> triggerCandidates;

    private void Awake()
    {
        this.triggerCandidates = new HashSet<GameObject>(this.TriggerCandidates);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.triggerCandidates.Contains(other.gameObject))
        {
            this.PlayerGroundedEvent.Invoke();

            // ? Reset s? l?n nh?y khi ch?m ??t
            if (RemainingJumps != null)
            {
                RemainingJumps.SetValue(MaxJumps);
#if UNITY_EDITOR
                Debug.Log($"GroundedTrigger: Reset RemainingJumps = {MaxJumps}");
#endif
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (this.triggerCandidates.Contains(other.gameObject))
        {
            this.PlayerAirbornEvent.Invoke();
        }
    }
}
