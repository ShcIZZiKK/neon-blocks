using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
	private Animator anim;
	private int hp = 500;
	public GameObject FX;
	public GameObject winGameText;

	private BallController ballController;
	private EnemySpawn enemySpawn;
	private GameManager gameManager;

	public Slider healthSlider; 

	public static bool isDeath = false;

	private void Start()
	{
		anim = GetComponent<Animator>();
		ballController = FindObjectOfType<BallController>();
		enemySpawn = FindObjectOfType<EnemySpawn>();
		gameManager = FindObjectOfType<GameManager>();
		healthSlider.maxValue = hp;
		healthSlider.value = hp;
	}

	private void Update()
	{
		
	}

	public void AddDamage(int damage)
	{
		if(!isDeath)
		{
			hp -= damage;

			healthSlider.value = hp;

			gameManager.PlaySoundOnCollision(transform.position, 3);
			anim.SetTrigger("damage");

			if (hp <= 0)
				Death();
		}
	}

	public void PlaySoundTeeth()
	{
		gameManager.PlaySoundOnCollision(transform.position, 2);
	}

	private void Death()
	{
		isDeath = true;

		FindObjectOfType<GameManager>().HideUIDeath();

		FX.GetComponent<Transform>().position = new Vector3(transform.position.x, transform.position.y + 16f, transform.position.z);
		FX.SetActive(true);

		ballController.gameObject.SetActive(false);
		enemySpawn.gameObject.SetActive(false);
		winGameText.SetActive(true);

		Destroy(gameObject);

		FindObjectOfType<GameManager>().WinGame();
	}
}
