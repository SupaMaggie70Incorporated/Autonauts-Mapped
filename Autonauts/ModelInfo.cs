using System.Collections.Generic;
using UnityEngine;

public class ModelInfo
{
	public bool m_RandomVariants;

	public List<GameObject> m_VariantModels;

	public ModelInfo(bool RandomVariants)
	{
		m_RandomVariants = RandomVariants;
		m_VariantModels = new List<GameObject>();
	}
}
