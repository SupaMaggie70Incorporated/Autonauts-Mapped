using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ModUI
{
	public void ShowPopup(string Title, string Description)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.ModsPopup);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsPopup>().SetInformation(Title, Description);
	}

	public void ShowPopupConfirm(string Title, string Description, DynValue CallbackOK, DynValue CallbackCancel)
	{
		if (CallbackOK.Function == null || CallbackOK.Function == null)
		{
			string descriptionOverride = "Error: ModUI.ShowPopupConfirm '" + Title + "' - Callback Function is invalid";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			GameStateManager.Instance.PushState(GameStateManager.State.ModsPopupConfirm);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsPopupConfirm>().SetInformation(Title, Description, CallbackOK, CallbackCancel, ModManager.Instance.GetLastCalledScript());
		}
	}
}
