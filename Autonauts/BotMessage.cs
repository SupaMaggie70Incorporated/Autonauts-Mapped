public class BotMessage
{
	public Worker m_Bot;

	public string m_NewMessageTitle;

	public string m_NewMessage;

	public BotMessage(Worker NewBot, string NewMessageTitle, string NewMessage)
	{
		m_Bot = NewBot;
		m_NewMessageTitle = NewMessageTitle;
		m_NewMessage = NewMessage;
	}
}
