using Rewired;
using UnityEngine;

public class InputBinding
{
	public KeyCode m_Code;

	public ModifierKey m_ModifierKey;

	public InputBinding(KeyCode Code, ModifierKey ModifierKey)
	{
		m_Code = Code;
		m_ModifierKey = ModifierKey;
	}
}
