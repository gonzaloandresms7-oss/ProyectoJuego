using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CollectibleManagerS : MonoBehaviour
{
    [Header("SecuenciaMovimiento (Bob)")]
    public float amplitud = 0.25f;     // altura del movimiento arriba/abajo
    public float speed = 2f;           // velocidad del bob
    public bool desfasarObjetos = true; // cada coleccionable empieza en un punto distinto del bob

    [Header("SistemaRecoleccion")]
    public TextMeshProUGUI itemCounter;
    public int totalItemScene = 2;
    public string collectibleTad = "Colectible";
    private int itemsCollected = 0;

    [Header("Lista De Objetos")]
    public List<Transform> collectibles = new List<Transform>();
    private Dictionary<Transform, Vector3> startposition = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, float> phase = new Dictionary<Transform, float>();

    public TimeSystem timer;

    [Header("Sonido")]
    public AudioClip collectClip;
    [Range(0f, 1f)] public float collectVolume = 0.8f;

    void Start()
    {
        foreach (var obj in collectibles)
        {
            if (obj != null)
            {
                startposition[obj] = obj.position;
                phase[obj] = desfasarObjetos ? Random.Range(0f, Mathf.PI * 2f) : 0f;

                Collider col = obj.GetComponent<Collider>();
                if (col == null)
                    col = obj.gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;

                if (obj.GetComponent<PlayerCollectibleDetector>() == null)
                    obj.gameObject.AddComponent<PlayerCollectibleDetector>().Init(this);
            }
        }
        UpdatedCounterUI();
    }

    void Update()
    {
        foreach (var obj in collectibles)
        {
            if (obj == null) continue;

            Vector3 startPos = startposition[obj];
            float newY = startPos.y + Mathf.Sin(Time.time * speed + phase[obj]) * amplitud;
            obj.position = new Vector3(startPos.x, newY, startPos.z);
        }
    }

    public void Collect(Transform obj)
    {
        if (!collectibles.Contains(obj)) return;
        collectibles.Remove(obj);
        itemsCollected++;
        if (timer != null)
            timer.AddTime(timer.timeBonus);
        UpdatedCounterUI();
        if (collectClip != null)
            AudioSource.PlayClipAtPoint(collectClip, Camera.main.transform.position, collectVolume);
        Destroy(obj.gameObject);
    }

    void UpdatedCounterUI()
    {
        if (itemCounter != null)
        {
            itemCounter.text = $"{itemsCollected} / {totalItemScene}";
        }
    }
}