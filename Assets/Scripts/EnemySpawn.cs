using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
	private EnemyBonus enemyBonus;

    public List<GameObject> freeEnemies; //Список врагов
	public List<GameObject> busyEnemies; //Неактивные враги

	public List<GameObject> freeBonuses; //Список бонусов
	public List<GameObject> busyBonuses; //Неактивные бонусы

	public List<GameObject> deathAnimPrefabFree; //Список эффектов при смерти
	public List<GameObject> deathAnimPrefabBusy; //Список неактивных эффектов при смерти

	public float maxRangeX = 13; //Максимальная позиция по Х для создания

	private float timeSpawn = 2f; //Частота появление врагов
	private float nextSpawn = 0; //Отслеживает следующий выстрел

	private int chanceBonus = 20; //Шанс что создастя куб с бонусом


	[Header("Настройки бонусов с оружия")]
	[Header("Настройки шансов оружия")]
	[Tooltip("Шанс что выпадет дополнительный мяч")]
	public int chanceToGetBall = 80;
	[Tooltip("Шанс что выпадет двойной урон")]
	public int chanceToDD = 3;
	[Tooltip("Минимальное и максимальное значение выпадаемых шаров")]
	public int minBall = 0, maxBall = 2;

	public int countBall = 0;
	public bool isDD = false;

	public GameObject numberOne;
	public GameObject numberTwo;
	public GameObject numberThree;

	private void Start()
	{
		enemyBonus = GetComponent<EnemyBonus>();
	}

	private void Update()
	{
		if(Time.time > nextSpawn)
		{
			nextSpawn = Time.time + timeSpawn;

			if(chanceBonus >= Random.Range(1, 100))
			{
				GameObject objBonus = freeBonuses[Random.Range(0, freeBonuses.Count - 1)];
				Enemy bonusEnemy = objBonus.GetComponent<Enemy>();
				bonusEnemy.transform.position = new Vector3(Random.Range(-maxRangeX, maxRangeX), 0.5f, transform.position.z);

				busyBonuses.Add(objBonus);
				freeBonuses.Remove(objBonus);

				bonusEnemy.gameObject.SetActive(true);
			}
			else
			{
				GameObject objEnemy = freeEnemies[Random.Range(0, freeEnemies.Count - 1)];
				Enemy enemy = objEnemy.GetComponent<Enemy>();
				enemy.transform.position = new Vector3(Random.Range(-maxRangeX, maxRangeX), 0.5f, transform.position.z);

				busyEnemies.Add(objEnemy);
				freeEnemies.Remove(objEnemy);

				enemy.getBallCount = GeneratePrizeBall();

				enemy.gameObject.SetActive(true);
			}

			
		}
	}

	private int GeneratePrizeBall()
	{
		countBall = 0;

		//Генерируем количество шаров во враге
		int changeSize = Random.Range(1, 100);
		if (changeSize <= chanceToGetBall)
		{
			countBall = Random.Range(minBall, maxBall);
		}

		return countBall;
	}

	public void ShowCoins(int giveCoins, Vector3 position)
	{
		switch (giveCoins)
		{
			case 1:
				numberOne.GetComponent<Transform>().position = position;
				numberOne.SetActive(true);
				break;
			case 2:
				numberTwo.GetComponent<Transform>().position = position;
				numberTwo.SetActive(true);
				break;
			case 3:
				numberThree.GetComponent<Transform>().position = position;
				numberThree.SetActive(true);
				break;
		}
	}

	public void ShowFXDeath(Vector3 pos)
	{
		GameObject objFX = deathAnimPrefabFree[0];

		deathAnimPrefabFree.RemoveAt(0);
		deathAnimPrefabBusy.Add(objFX);

		objFX.transform.position = pos;
		objFX.SetActive(true);

		StartCoroutine(HideFXDeath(objFX));
	}

	IEnumerator HideFXDeath(GameObject objFX)
	{
		yield return new WaitForSeconds(0.7f);

		objFX.SetActive(false);

		deathAnimPrefabBusy.Remove(objFX);
		deathAnimPrefabFree.Add(objFX);
	}
}
