using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Bonus
{
	none,
	hp,
	damage,
	coins
}

public class Enemy : MonoBehaviour
{
	private Transform target;

	

	private EnemySpawn enemySpawn;
	private float targetZ = -10;
	private CameraController cam;
	private GameManager gameManager;
	[SerializeField]
	private float speed = 5f;
	public int getBallCount = 0;

	public Bonus bonus;

	public int damage = 2; //”рон от шара

	public int giveCoins = 100;

	private void Start()
	{
		enemySpawn = FindObjectOfType<EnemySpawn>();
		cam = FindObjectOfType<CameraController>();
		gameManager = FindObjectOfType<GameManager>();
	}

	private void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, targetZ), Time.deltaTime * speed);

		if (transform.position.z <= -10)
		{
			gameManager.DecreaseHealth(damage);
			Death();
			cam.LightShake();
		}

		speed += Time.deltaTime * 0.2f;
	}

	public void Death()
	{
		enemySpawn.ShowFXDeath(transform.position);

		if(bonus == Bonus.none)
		{
			enemySpawn.freeEnemies.Add(gameObject);
			enemySpawn.busyEnemies.RemoveAt(0);
		}
		else
		{
			enemySpawn.freeBonuses.Add(gameObject);
			enemySpawn.busyBonuses.RemoveAt(0);
		}

		gameObject.SetActive(false);

		if(getBallCount > 0)
		{
			enemySpawn.ShowCoins(getBallCount, transform.position);
		}
	}
}
