using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
	private EnemyBonus enemyBonus;

    public List<GameObject> freeEnemies; //������ ������
	public List<GameObject> busyEnemies; //���������� �����

	public List<GameObject> freeBonuses; //������ �������
	public List<GameObject> busyBonuses; //���������� ������

	public List<GameObject> deathAnimPrefabFree; //������ �������� ��� ������
	public List<GameObject> deathAnimPrefabBusy; //������ ���������� �������� ��� ������

	public float maxRangeX = 13; //������������ ������� �� � ��� ��������

	private float timeSpawn = 2f; //������� ��������� ������
	private float nextSpawn = 0; //����������� ��������� �������

	private int chanceBonus = 20; //���� ��� �������� ��� � �������


	[Header("��������� ������� � ������")]
	[Header("��������� ������ ������")]
	[Tooltip("���� ��� ������� �������������� ���")]
	public int chanceToGetBall = 80;
	[Tooltip("���� ��� ������� ������� ����")]
	public int chanceToDD = 3;
	[Tooltip("����������� � ������������ �������� ���������� �����")]
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

		//���������� ���������� ����� �� �����
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
