using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance;

	Transform m_Transform;

	public static Quaternion Rotation { get { return Instance.m_Transform.rotation; } }

	private void Awake()
	{
		Instance = this;
		m_Transform = transform;
	}
}
