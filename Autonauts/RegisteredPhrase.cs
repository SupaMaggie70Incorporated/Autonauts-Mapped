public class RegisteredPhrase
{
	public WorkerInterpreter m_Interpreter;

	public bool m_Triggered;

	public RegisteredPhrase(WorkerInterpreter Interpreter)
	{
		m_Interpreter = Interpreter;
		m_Triggered = false;
	}
}
