using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTeeth : MonoBehaviour
{
	private Vector3 startTeethPos;
	private Vector3 raisedTeethPos;
	private float speed = 3f;

	private bool isTriggerEnemy = false;

	void Start()
    {
		startTeethPos = transform.position;
		raisedTeethPos = new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z);
	}

	private void Update()
	{
		if(isTriggerEnemy)
		{
			transform.position = Vector3.Lerp(transform.position, raisedTeethPos, Time.deltaTime * speed);
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, startTeethPos, Time.deltaTime * speed);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			isTriggerEnemy = true;

			StartCoroutine(ReturnTeethPos());
		}
	}

	IEnumerator ReturnTeethPos()
	{
		yield return new WaitForSeconds(3.5f);

		isTriggerEnemy = false;
	}
}
