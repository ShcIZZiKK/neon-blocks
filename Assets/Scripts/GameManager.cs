using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text comboText;
    public Animator comboTextAnim;
    private BallController ballController;

    public Text pointsText;
    public int points = 0;

    private float timeToCombo = 2.5f;
    private float poplovok;
    private int oldPoints = 0;

    [Tooltip("Переносимый по карте источник звука")]
    public AudioSource audioSource;
    [Tooltip("Звуки смерти врага")]
    public List<AudioClip> soundDeathEnemy;
    private AudioSource foneMusic; //Музыка для фона
    [Tooltip("Музыка победы")]
    public AudioClip winMusic;
    [Tooltip("Музыка отскока")]
    public AudioClip reboundSound;
    [Tooltip("Урон по боссу")]
    public AudioClip damageBoss;
    [Tooltip("Блок боссом")]
    public AudioClip damageTeeth;

    private int hp = 100; //Здоровье
    public GameObject launchPoint; //Зона от куда стреляем, которая будет показывать цвет здоровь
    [SerializeField]
    private Color startColorHp; //Начальное здоровье
    public Color middleColorHp; //Серидина здоровья
    public Color endColorHp; //Цвет здоровья в конце
    private float timeToChangeHpColor = 0.5f;
    public GameObject endGameManager;

    private float colorHpOpacity = 0.57f;

    public Text countBallText; //Воводит кол-во шаров на экран

    //Отобрание двойного урона в интерфейсе
    public GameObject DDText;
    public float timeActiveDDText = 0;

    //Бонус очков
    private float timeActiveMultiplyCoins = 0;
    private float timeActiveMultiplyCoinsAdd = 4f;
    public bool isMultiplyCoins = false;

    private bool isPaused = false;
    public GameObject pausePanel;
    private bool isDeath = false;

    public List<GameObject> hideGameObjects;

    void Start()
    {
        ballController = FindObjectOfType<BallController>();
        foneMusic = GetComponent<AudioSource>();

        startColorHp = launchPoint.GetComponent<Renderer>().material.color;
        startColorHp.a = colorHpOpacity;
        middleColorHp.a = colorHpOpacity - 0.37f;
        endColorHp.a = colorHpOpacity + 0.1f;

        launchPoint.GetComponent<Renderer>().material.color = startColorHp;
    }

    void Update()
    {
        HPManager();
        ShowDDText();
        MultiplyCoins();
        PauseGame();
    }

    public void SoundPlay(AudioClip sound)
	{
        foneMusic.PlayOneShot(sound);
	}

    private void PauseGame()
	{
        if(!isDeath)
		{
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    Time.timeScale = 0;
                    isPaused = true;
                    foneMusic.Pause();
                    pausePanel.SetActive(true);
                }
                else
                {
                    isPaused = false;
                    Time.timeScale = 1;
                    foneMusic.Play();
                    pausePanel.SetActive(false);
                }

            }
        }
	}

    private void MultiplyCoins()
	{
        timeActiveMultiplyCoins -= Time.deltaTime;
        if (timeActiveMultiplyCoins > 0)
        {
            isMultiplyCoins = true;
        }
        else
        {
            isMultiplyCoins = false;
        }
    }

    public void AddTimeMultiplyCoins()
    {
        if (timeActiveMultiplyCoins > 0)
        {
            timeActiveMultiplyCoins += timeActiveMultiplyCoinsAdd;
        }
        else
        {
            timeActiveMultiplyCoins = timeActiveMultiplyCoinsAdd;
        }
    }

    private void ShowDDText()
	{
        timeActiveDDText -= Time.deltaTime;
        if (timeActiveDDText > 0)
        {
            DDText.SetActive(true);
        }
        else
        {
            DDText.SetActive(false);
        }
    }

    public void SetCountBallText(int countBall)
	{
        countBallText.text = countBall.ToString();
    }

    public void AddPoints(int addPoints)
	{
        if (isMultiplyCoins)
            addPoints *= 2;

        if (Time.time < poplovok)
		{
            poplovok = Time.time + timeToCombo;

            oldPoints += addPoints;
            points += oldPoints;

            ShowComboText(oldPoints);
        }
        else
		{
            poplovok = Time.time + timeToCombo;

            points += addPoints;
            oldPoints = addPoints;

            ShowComboText(addPoints);
        }

        pointsText.text = points.ToString();
    }

    public void DecreaseHealth(int damage)
	{
        hp -= damage;

        if (hp <= 0)
            Death();
    }

    public void AddHP(int addHp)
    {
        hp += addHp;
    }

    private void HPManager()
	{
        if (hp > 100)
            hp = 100;

        if (hp <= 65 && hp >= 35)
		{
            startColorHp = Color.Lerp(startColorHp, middleColorHp, Time.deltaTime * timeToChangeHpColor);
        }
        else if(hp < 35 && hp > 15)
		{
            startColorHp = Color.Lerp(startColorHp, endColorHp, Time.deltaTime * timeToChangeHpColor);
        }
        else if(hp <= 15)
		{
            startColorHp = Color.Lerp(startColorHp, endColorHp, Time.deltaTime * timeToChangeHpColor);
        }

        launchPoint.GetComponent<Renderer>().material.color = startColorHp;
        launchPoint.GetComponent<Renderer>().material.SetColor("_EmissionColor", startColorHp);
    }

    public void ShowComboText(int points)
	{
        comboText.text = points.ToString();
        comboTextAnim.SetTrigger("comboPoints");
    }

    // type - звук. 0 - попадание по кубику | 1 - по стене | 2 - по щиту врага | 3 - по боссу
    public void PlaySoundOnCollision(Vector3 pos, int type)
	{
        audioSource.transform.position = pos;

        switch(type)
		{
            case 0:
                audioSource.PlayOneShot(soundDeathEnemy[Random.Range(0, soundDeathEnemy.Count - 1)]);
                break;
            case 1:
                audioSource.PlayOneShot(reboundSound);
                break;
            case 2:
                audioSource.PlayOneShot(damageTeeth);
                break;
            case 3:
                audioSource.PlayOneShot(damageBoss);
                break;

        }
    }

    public void Death()
	{
        isDeath = true;

        HideUIDeath();

        endGameManager.SetActive(true);
        endGameManager.GetComponent<Animator>().SetTrigger("death");

    }

    public void HideUIDeath()
	{
        for (int i = 0; i < hideGameObjects.Count; i++)
        {
            hideGameObjects[i].gameObject.SetActive(false);
        }
    }

    public void WinGame()
	{
        foneMusic.clip = winMusic;
        foneMusic.Play();
    }

    public void ReloadScene()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
