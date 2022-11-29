using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
	private GameManager gameManager;
	private BallController ballController;
	private Boss boss;

	private int countTriggerWall = 0;

	private void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
		ballController = FindObjectOfType<BallController>();
		boss = FindObjectOfType<Boss>();
	}

	private void Update()
	{
		if(countTriggerWall > ballController.maximumReflectionCount)
		{
			ballController.ShowFlashEffect(gameObject);
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Wall")
		{
			gameManager.PlaySoundOnCollision(gameObject.transform.position, 1);
			countTriggerWall++;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Fencing")
		{
			ballController.ShowFlashEffect(gameObject);

			if (!Boss.isDeath)
			{
				boss.AddDamage(ballController.damage);
			}
		}

		if(other.tag == "Enemy")
		{
			gameManager.PlaySoundOnCollision(gameObject.transform.position, 0);
			Enemy enemy = other.GetComponent<Enemy>();

			switch (enemy.bonus)
			{
				case Bonus.damage:
					ballController.AddTimeDD();
					break;
				case Bonus.hp:
					gameManager.AddHP(25);
					break;
				case Bonus.coins:
					gameManager.AddTimeMultiplyCoins();
					break;
			}

			gameManager.AddPoints(enemy.giveCoins);

			ballController.AddBall(enemy.getBallCount);

			Destroy(gameObject);
			enemy.Death();
		}

		if(other.tag == "Boss")
		{
			FindObjectOfType<Boss>().PlaySoundTeeth();
			ballController.ShowFlashEffect(gameObject);
		}
	}
}
