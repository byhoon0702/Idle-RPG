using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Testss : MonoBehaviour
{

	public Transform target;
	public Transform left;
	public Transform right;

	public float angle;
	public int seperate;

	public NewSkill obj;
	void Start()
	{

	}
	public void OnTest()
	{
		var dd = Instantiate(obj);
		Debug.Log($"{obj.audioClip} : {dd.audioClip}");

	}


	private void Update()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Vector3 angled = direction.GetAngledVector3(angle);
		left.position = angled.normalized;

		angled = direction.GetAngledVector3(angle, true);
		right.position = angled.normalized;
	}


	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Vector2 direction = (target.position - transform.position).normalized;
		float sep_angle = angle / Mathf.Max(1, seperate - 1);

		int count = seperate - 1;

		float startAngle = 90;
		if (count > 0)
		{
			startAngle = startAngle - (angle / 2);
		}
		for (int i = 0; i < seperate; i++)
		{
			Vector2 right = direction.GetRightVector2();

			Vector2 angled = right.GetAngledVector2(startAngle + (sep_angle * i));
			Gizmos.DrawLine(transform.position, angled);
			angled = right.GetAngledVector2(startAngle + (sep_angle * i), true);
			Gizmos.DrawLine(transform.position, angled);
			//if (i + 1 < seperate)
			//{
			//	Vector3 angled = direction.GetAngledVector3(angle - (sep_angle * i));
			//	Gizmos.DrawLine(transform.position, angled);
			//	angled = direction.GetAngledVector3(angle - (sep_angle * i), true);
			//	Gizmos.DrawLine(transform.position, angled);
			//}
			//else
			//{
			//	Vector3 angled = direction.GetAngledVector3(0);
			//	Gizmos.DrawLine(transform.position, angled);
			//}
		}
	}
}
