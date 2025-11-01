using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeadTrigger : MonoBehaviour
{
    [Header("References")]
    public BoolVariable IsAlive;
    public IntVariable Lives; // dùng IntVariable thay cho int th??ng

    [Header("Events")]
    public UnityEvent HeadCollisionEvent;

    [Header("Trigger Settings")]
    public GameObject[] TriggerCandidates;

    private HashSet<GameObject> triggerCandidates;

    private void Awake()
    {
        this.triggerCandidates = new HashSet<GameObject>(this.TriggerCandidates);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (PlayerMovement.IsCheatModeActive)
        {
            Debug.Log("🛡️ Cheat active - no head collision damage!");
            return;
        }

        if (this.triggerCandidates.Contains(other.gameObject) && this.IsAlive.Value)
        {
            Lives.Value--;
            Debug.Log("Player hit! Remaining lives: " + Lives.Value);

            if (Lives.Value <= 0)
            {
                Lives.Value = 0;
                this.HeadCollisionEvent.Invoke();
            }
        }
    }
}
