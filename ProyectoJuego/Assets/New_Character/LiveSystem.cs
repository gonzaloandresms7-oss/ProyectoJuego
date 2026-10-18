using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class LiveSystem : MonoBehaviour
{
    [Header("Vidas")]
    public int maxLives=3;
    private int currentLives;
    public Transform respawnPoint;
    public TextMeshProUGUI livesText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLives=maxLives;
        UpdateLivesUI();
        if(respawnPoint==null)
        {
            respawnPoint=this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLivesUI();
    }

    void UpdateLivesUI()
    {
        if(livesText !=null)
        {
            livesText.text=$"{currentLives}/{maxLives}";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DeathZone"))
        {
            currentLives--;
            Debug.Log("Vidas Restantes: "+currentLives);
            if(currentLives<=-1)
            {
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                transform.position=respawnPoint.position;
            }
        }
    }
}
