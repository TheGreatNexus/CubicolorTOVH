using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour
{
	Transform m_Transform;

	Material[] m_Materials;

	public float Height { get { return GetComponentInChildren<BoxCollider>().bounds.extents.y * 2; } }

	private void Awake()
	{
		m_Transform = transform;
		FetchMaterials();
	}

	protected void FetchMaterials()
	{
		m_Materials = GetComponentsInChildren<MeshRenderer>().Select(item => item.material).ToArray();
	}

	public void TranslateTile(Vector3 vect)
	{
		StartCoroutine(TranslateCoroutine(.5f,vect));
	}

	public void RotateTile(Vector3 axis,float angle)
	{
		StartCoroutine(RotateCoroutine(.5f, axis,angle));
	}

	public void DestroyTile(float duration=.5f, bool move=true)
	{
		StartCoroutine(FadeOutMoveAndDestroyCoroutine(duration, move?.5f:0));
	}

	void ChangeMaterialsAlpha(float alpha)
	{
		foreach (var material in m_Materials)
			material.color = new Color(material.color.r, material.color.g, material.color.b,alpha);
	}

	IEnumerator FadeOutMoveAndDestroyCoroutine(float duration,float distance)
	{
		float elapsedTime = 0;
		Vector3 startPos = m_Transform.position;
		Vector3 endPos = startPos - Vector3.up * distance;

		while (elapsedTime<duration)
		{
			elapsedTime += Time.deltaTime;
			float k = elapsedTime / duration;
			ChangeMaterialsAlpha(1 - k);
			m_Transform.position = Vector3.Lerp(startPos, endPos, k);
			yield return null;
		}

		Destroy(gameObject);
	}

	IEnumerator TranslateCoroutine(float duration, Vector3 vect)
	{
		float elapsedTime = 0;
		Vector3 startPos = m_Transform.position;
		Vector3 endPos = startPos +vect;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float k = elapsedTime / duration;
			m_Transform.position = Vector3.Lerp(startPos, endPos, k);
			yield return null;
		}

		m_Transform.position = endPos;
	}

	IEnumerator RotateCoroutine(float duration, Vector3 axis,float angle)
	{
		float elapsedTime = 0;
		Quaternion startRot = m_Transform.rotation;
		Quaternion endRot = Quaternion.AngleAxis(angle,axis)*startRot;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float k = elapsedTime / duration;
			m_Transform.rotation = Quaternion.Slerp(startRot, endRot, k);
			yield return null;
		}

		m_Transform.rotation = endRot;
	}
}
