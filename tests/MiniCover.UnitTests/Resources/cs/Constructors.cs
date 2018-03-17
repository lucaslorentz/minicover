public class AClassWithATryFinallyInConstructor
{
	private readonly int value;
	public AClassWithATryFinallyInConstructor()
	{
		try
		{
			value = 5;
		}
		finally
		{
			Exit();
		}
	}

	private void Exit()
	{
	}
}

public class AClassWithFieldInitializedOutsideConstructor
{
	private readonly int value = 5;
}
