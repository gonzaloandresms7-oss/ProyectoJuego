using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class CollectibleManagerS : MonoBehaviour
{
    [Header("SecuenciaMovimiento")]
    public float amplitud=0.25f;
    public float speed=2f;
    public float rotationSpeed=45f;

    [Header("SistemaRecoleccion")]
    public TextMeshProUGUI itemCounter;
    public int totalItemScene=2;
    public string collectibleTad="Colectible";
    private int itemsCollected=0;

    [Header("Lista De Objetos")]
    public List<Transform>collectibles =new List<Transform>();
    private Dictionary<Transform,Vector3> startposition= new Dictionary<Transform, Vector3>();

    public TimeSystem timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(var obj in collectibles)
        {
            if(obj!=null)
            {
                startposition[obj]=obj.position;
                Collider col = obj.GetComponent<Collider>();
                if(col==null)
                    col=obj.gameObject.AddComponent<BoxCollider>();
                col.isTrigger=true;

                if(obj.GetComponent<PlayerCollectibleDetector>()==null)
                obj.gameObject.AddComponent<PlayerCollectibleDetector>().Init(this);

            }
        }
        UpdatedCounterUI();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var obj in collectibles)
        {
            if(obj==null) continue;
            Vector3 StartPos=startposition[obj];
            float newY=StartPos.y+Mathf.Sin(Time.deltaTime*speed)*amplitud;
            obj.position=new Vector3(StartPos.x,newY,StartPos.z);
            obj.Rotate(Vector3.up, rotationSpeed*Time.deltaTime, Space.World);
        }
    }

    public void Collect(Transform obj)
    {
        if(!collectibles.Contains(obj)) return;
        collectibles.Remove(obj);
        itemsCollected++;
        if (timer != null)
        timer.AddTime(timer.timeBonus);        
        UpdatedCounterUI();
        Destroy(obj.gameObject);
    }

    void UpdatedCounterUI()
    {
        if(itemCounter != null)
        {
            itemCounter.text=$"{itemsCollected} / {totalItemScene}";
        }
    }
}
