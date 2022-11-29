using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Animator anim;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	public void LightShake()
	{
		anim.SetTrigger("lightShake");
	}
}
