using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class BallController : MonoBehaviour
{
	//Поля устанавливаемые в Unity
	[Header("Set in Inspector")]
	public GameObject prefabProjectile;
	public float velocityMult = 8f;

	//Поля устанавливаемые динамически
	[Header("Set Dynamically")]
	public GameObject projectile;
	public GameObject launchPoint;
	public Vector3 launchPos;
	public bool aimingMode;
	private GameManager gameManager;

	private Rigidbody projectileRigidbody;
	public GameObject flassEffect;

	[HideInInspector]
	public int qttyBall = 0; //Количество шаров
	private float toToNextBall = 2f; //Время до создания нового шара
	private float toToNextBallTime; //Попловок

	public int damage = 20;

	//Бонус силы
	private float timeActiveDD = 0;
	private float timeActiveAdd = 4f;
	public bool isDD = false;

	[Tooltip("Цвет шара под ДД")]
	public Color colorBallOnDD;
	[Tooltip("Дефолтный цвет шара")]
	public Color defaultColorBall;
	[Tooltip("Emission для шара")]
	public Color defaultColorBallEmission;

	LineRenderer lineRenderer;
	InputController inputController;

	[Tooltip("Количество отскоков")]
	public int maximumReflectionCount = 2;
	private float maximumRayCastDistance = 80f;

	List<Vector3> reflectionPositions = new List<Vector3>();
	Vector3 targetDirection = new Vector3();


	void Awake()
	{
		Transform launchPointTrans = transform.Find("LaunchPoint");
		launchPoint = launchPointTrans.gameObject;
		launchPos = launchPointTrans.position;

		gameManager = FindObjectOfType<GameManager>();
		lineRenderer = GetComponent<LineRenderer>();
		inputController = FindObjectOfType<InputController>();

		// A simple 2 color gradient with a fixed alpha of 1.0f.
		float alpha = 1.0f;
		Gradient gradient = new Gradient();
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
		);
		lineRenderer.colorGradient = gradient;
	}

	public void CreateProjectile()
	{
		if (qttyBall > 0)
		{
			//Игрок нажал кнопку мыши когда указатель находится над рогаткой
			aimingMode = true;
			//Создать снаряд
			projectile = Instantiate(prefabProjectile) as GameObject;

			//Перекрашиывем
			SetColor(projectile);

			//Поместить в точку launchPoint
			projectile.transform.position = launchPos;
			//Сделать его кинетическим
			projectileRigidbody = projectile.GetComponent<Rigidbody>();
			projectileRigidbody.isKinematic = true;
		}
	}

	void Update()
	{
		if(Time.time >= toToNextBallTime)
		{
			AddBall(1);

			toToNextBallTime = Time.time + toToNextBall;
		}

		timeActiveDD -= Time.deltaTime;
		if (timeActiveDD > 0)
		{
			damage = 40;
			isDD = true;
		}
		else
		{
			isDD = false;
		}

		//Если рогатка не в режиме прицеливания, не выполнять этот код
		if (!aimingMode)
			return;

		projectile.transform.position = new Vector3(inputController.Horizontal, 0, Mathf.Abs(inputController.Horizontal) - 5f);

		targetDirection = new Vector3(-inputController.Horizontal, 0, -projectile.transform.position.z);

		DrawCurrentTrajectory();
	}

	public void Shoot()
	{
		if (projectile == null)
			return;

		lineRenderer.enabled = false;
		//Кнопка мыши отпущена
		aimingMode = false;
		projectileRigidbody.isKinematic = false;

		projectileRigidbody.AddForce(targetDirection * 100f, ForceMode.Impulse);
		projectile = null;

		qttyBall--;
		gameManager.SetCountBallText(qttyBall);
	}

	private void DrawCurrentTrajectory()
	{
		lineRenderer.enabled = true;
		int layerMask = 1 << 8;

		reflectionPositions.Clear();

		Vector3 position = projectile.transform.position;
		Vector3 direction = -position * maximumRayCastDistance;

		reflectionPositions.Add(position);

		for (int i = 0; i <= maximumReflectionCount; ++i)
		{
			RaycastHit hit;

			if (Physics.Raycast(position, direction, out hit, Mathf.Infinity, layerMask))
			{
				position = hit.point + hit.normal * 0.00001f;
				direction = Vector3.Reflect(direction, hit.normal);

				reflectionPositions.Add(position);

				if (hit.collider.tag == "Fencing" || hit.collider.tag == "Enemy")
					break;
			}
		}

		lineRenderer.positionCount = reflectionPositions.Count;
		lineRenderer.SetPositions(reflectionPositions.ToArray());
	}

	private void SetColor(GameObject ball)
	{
		Renderer rendererBall = ball.GetComponent<Renderer>();
		Transform transformBall = ball.GetComponent<Transform>();
		GameObject blueTrail = transformBall.Find("BlueTrail").gameObject;
		GameObject redTrail = transformBall.Find("RedTrail").gameObject;

		if (isDD)
		{
			rendererBall.material.color = colorBallOnDD;
			rendererBall.material.SetColor("_EmissionColor", colorBallOnDD);
			blueTrail.SetActive(false);
			redTrail.SetActive(true);
		}
		else
		{
			rendererBall.material.color = defaultColorBall;
			rendererBall.material.SetColor("_EmissionColor", defaultColorBallEmission);
			blueTrail.SetActive(true);
			redTrail.SetActive(false);
		}
	}

	public void AddTimeDD()
	{
		gameManager.timeActiveDDText = 1f;

		if (timeActiveDD > 0)
		{
			timeActiveDD += timeActiveAdd;
		}
		else
		{
			timeActiveDD = timeActiveAdd;
		}
	}

	public void AddBall(int countBall)
	{
		qttyBall += countBall;

		gameManager.SetCountBallText(qttyBall);
	}

	public void ShowFlashEffect(GameObject ball)
	{
		GameObject gameObject = Instantiate(flassEffect);

		Vector3 flashPos = ball.transform.position;
		flashPos.z -= 1f;
		Quaternion flashRot = new Quaternion(-180f, ball.transform.rotation.y, ball.transform.rotation.z, Time.deltaTime);

		gameObject.transform.position = flashPos;
		gameObject.transform.rotation = flashRot;

		Destroy(ball);
	}
}
