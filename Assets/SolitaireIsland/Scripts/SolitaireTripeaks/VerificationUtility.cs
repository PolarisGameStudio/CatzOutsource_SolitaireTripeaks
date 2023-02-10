namespace SolitaireTripeaks
{
	public class VerificationUtility
	{
		private static VerificationUtility verification;

		public string CheatId;

		public static VerificationUtility Get()
		{
			if (verification == null)
			{
				verification = new VerificationUtility();
			}
			return verification;
		}

		public void SetCheatId(string id)
		{
			CheatId = id;
		}

		public bool IsCheatShowing()
		{
			if (string.IsNullOrEmpty(CheatId))
			{
				return false;
			}
			return !AuxiliaryData.Get().HasView(CheatId);
		}

		public void SetViewCheat()
		{
			if (!string.IsNullOrEmpty(CheatId))
			{
				AuxiliaryData.Get().PutView(CheatId);
			}
		}
	}
}
