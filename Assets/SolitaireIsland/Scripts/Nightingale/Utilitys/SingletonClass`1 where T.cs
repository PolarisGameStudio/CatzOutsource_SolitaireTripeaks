namespace Nightingale.Utilitys
{
	public class SingletonClass<T> where T : class, new()
	{
		private static T _instance;

		public static T Get()
		{
			if (_instance == null)
			{
				_instance = new T();
			}
			return _instance;
		}

		public static void Dispose()
		{
			_instance = (T)null;
		}
	}
}
