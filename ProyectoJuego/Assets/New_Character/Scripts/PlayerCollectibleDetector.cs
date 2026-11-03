using UnityEngine;

public class PlayerCollectibleDetector : MonoBehaviour
{
    private CollectibleManagerS manager;
    public void Init(CollectibleManagerS manager)
    {
        this.manager=manager;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            manager.Collect(transform);
        }
    }
}
