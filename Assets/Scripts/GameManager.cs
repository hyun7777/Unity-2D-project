using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float invincibleTime = 1.0f;
    [SerializeField]
    private bool isInvincible = false;

    [SerializeField]
    public int stagepoint = 0;
    [SerializeField]
    private int totalpoint = 0;
    [SerializeField]
    private int stageIndex;
    [SerializeField]
    public int health;
    [SerializeField]
    private Player player;
    [SerializeField]
    private GameObject[] Stages;

    [SerializeField]
    private GameObject Character;
    [SerializeField] 
    private GameObject UI;
    [SerializeField]
    private GameObject Game;

    [SerializeField]
    private TextMeshProUGUI UIhealth;
    [SerializeField]
    private TextMeshProUGUI UIpoint;
    [SerializeField]
    private TextMeshProUGUI UIstage;
    [SerializeField]
    private GameObject End;

    public void gameStart()
    {
        UI.SetActive(false);
        Stages[stageIndex].SetActive(true);
        Character.SetActive(true);
        Game.SetActive(true);
    }

    void Update()
    {
        UIpoint.text = (totalpoint + stagepoint).ToString();
        UIhealth.text = health.ToString();
    }

    public void NextStage()
    {
        if (stageIndex >= Stages.Length - 1)
        {
            Time.timeScale = 0;
            TextMeshProUGUI btnText = End.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Clear!";
            End.SetActive(true);
            return;
        }

        Stages[stageIndex].SetActive(false);
        stageIndex++;
        Stages[stageIndex].SetActive(true);
        PlayerReposition();
        UIstage.text = "STAGE " + (stageIndex + 1);

        totalpoint += stagepoint;
        stagepoint = 0;
    }

    public void HealthDown()
    {
        if (isInvincible) return;

        health--;
        StartCoroutine(InvincibleRoutine());

        if (health <= 0)
        {
            player.OnDie();

            Time.timeScale = 0;
            TextMeshProUGUI btnText = End.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Game Over...";
            End.SetActive(true);
        }
    }

    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(health > 1)
            {
                PlayerReposition();
            }

            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-7, -1, 0);
        player.VelocityZero();
    }

    public void reStart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void quit()
    {
        Application.Quit();
    }
}
