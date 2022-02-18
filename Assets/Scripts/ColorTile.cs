using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ColorTile : Tile
{
	[SerializeField] protected COLOR m_Color;
	[SerializeField] protected Material[] m_ColorMaterials;
	public COLOR Color { get { return m_Color; } set { m_Color = value; SetColorMaterial(m_Color); } }

	[SerializeField] MeshRenderer[] m_ColorRenderers;

	void SetColorMaterial(COLOR color)
	{
		foreach (var rd in m_ColorRenderers)
		{
			rd.material = m_ColorMaterials[(int)color];
		}

		if (this.gameObject.scene.rootCount != 0)
			FetchMaterials();
	}

	private void OnValidate()
	{
		if (m_ColorRenderers != null && m_ColorRenderers.Length > 0)
			SetColorMaterial(m_Color);
	}
}
