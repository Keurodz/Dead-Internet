using UnityEngine;

public class EnterDungeonDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SokobanDungeonManager.Instance.EnterDungeon();
        }
    }
}
