#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

// add this line in order to use any obscured types from code:
using CodeStage.AntiCheat.ObscuredTypes;

namespace CodeStage.AntiCheat.Examples
{
	using System.Text;
	using UnityEngine;

	public class ObscuredTypesExamples : MonoBehaviour
	{
		[Header("Regular variables")]
		public string regularString = "I'm regular string";

		public int regularInt = 1987;
		public float regularFloat = 2013.0524f;
		public Vector3 regularVector3 = new Vector3(10.5f, 11.5f, 12.5f);

		// you can declare obscured variables like this and they will
		// appear in the inspector
		//
		// you can change values in inspector and they will be
		// automatically encrypted under the hood
		[Header("Obscured (secure) variables")]
		public ObscuredString obscuredString = "I'm obscured string";
		public ObscuredInt obscuredInt = 1987;
		public ObscuredFloat obscuredFloat = 2013.0524f;
		public ObscuredVector3 obscuredVector3 = new Vector3(10.5f, 11.5f, 12.5f);
		public ObscuredBool obscuredBool = true;
		public ObscuredLong obscuredLong = 945678987654123345L;
		public ObscuredDouble obscuredDouble = 9.45678987654d;
		public ObscuredVector2 obscuredVector2 = new Vector2(8.5f, 9.5f);
		public ObscuredDecimal obscuredDecimal = 503.4521m;

#if UNITY_2017_2_OR_NEWER
		public ObscuredVector2Int obscuredVector2Int = new Vector2Int(8, 9);
		public ObscuredVector3Int obscuredVector3Int = new Vector3Int(15, 16, 17);
#endif

		private readonly StringBuilder logBuilder = new StringBuilder();

		private void Awake()
		{
			ObscuredStringExample();
			ObscuredIntExample();
		}

		private void Start()
		{
			// values set in inspector can be used as usual

			logBuilder.Length = 0;
			logBuilder.AppendLine("ObscuredDecimal value from inspector: " + obscuredDecimal);
			logBuilder.AppendLine("ObscuredBool value from inspector: " + obscuredBool);
			logBuilder.AppendLine("ObscuredLong value from inspector: " + obscuredLong);
			logBuilder.AppendLine("ObscuredDouble value from inspector: " + obscuredDouble);
			logBuilder.AppendLine("ObscuredVector2 value from inspector: " + obscuredVector2);

#if UNITY_2017_2_OR_NEWER
			logBuilder.AppendLine("ObscuredVector2Int value from inspector: " + obscuredVector2Int);
			logBuilder.AppendLine("ObscuredVector3Int value from inspector: " + obscuredVector3Int);
#endif
			Debug.Log(logBuilder);

			// to make it harder to find encrypted value with unknown value search,
			// you can change encrypted value at the unexpected moments so cheater
			// can't guess if it was changed or not
			//
			// check RandomizeCryptoKey() API docs for more details
			Invoke("RandomizeObscuredVars", Random.Range(1f, 10f));
		}

		private void RandomizeObscuredVars()
		{
			obscuredInt.RandomizeCryptoKey();
			obscuredFloat.RandomizeCryptoKey();
			obscuredString.RandomizeCryptoKey();
			obscuredVector3.RandomizeCryptoKey();

			// change interval each time to make it inconsistent
			Invoke("RandomizeObscuredVars", Random.Range(1f, 10f));
		}

		private void ObscuredStringExample()
		{
			/* -------------- usage example -------------- */

			// you can change default crypto key using this method
			// it will be automatically used by any new ObscuredString
			// instances and it will be applied to any existing instance
			// on the value change
			ObscuredString.SetNewCryptoKey("I LOVE MY GIRLz");

			// hey, Daniele! ;D
			var regular = "the Goscurry is not a lie ;)";

			// obscured <-> regular conversion is implicit
			ObscuredString obscured = regular;

			// you can get raw encrypted value at any time
			// and save it somewhere for example
			var encryptedValueRaw = obscured.GetEncrypted();

			// to construct new obscured instance from it after loading it back
			var newObscured = ObscuredString.FromEncrypted(encryptedValueRaw);

			// all other obscured types have similar usage pipeline and APIs

			/* -------------- logs-------------- */

			logBuilder.Length = 0;
			logBuilder.AppendLine("[ ObscuredString example ]");
			logBuilder.AppendLine("Original value:\n" + regular);
			logBuilder.AppendLine("Obscured value in memory:\n" + newObscured.GetEncrypted());
			Debug.Log(logBuilder);
		}

		private void ObscuredIntExample()
		{
			/* -------------- usage example -------------- */

			var regular = 5;
			
			// obscured <-> regular conversion is implicit
			var obscured = (ObscuredInt)regular;

			// you can change default crypto key at any time
			// it will be automatically used by any new instances
			//
			// it will be automatically applied to the existing
			// instances when you change their value
			ObscuredInt.SetNewCryptoKey(666);

			// and you can apply it manually to the instance,
			// so you may change its crypto key
			// even if value did not change
			obscured.ApplyNewCryptoKey();

			// all usual operations are supported
			regular = obscured;
			obscured = regular;
			obscured -= 2;
			obscured = obscured + regular + 10;
			obscured = obscured / 2;
			obscured++;
			obscured--;

			// all other obscured types have similar usage pipeline and APIs

			/* -------------- logs-------------- */

			logBuilder.Length = 0;
			logBuilder.AppendLine("[ ObscuredInt example ]");
			logBuilder.AppendLine("Original lives count: " + regular);
			logBuilder.AppendLine("Obscured lives count in memory: " + ((ObscuredInt)regular).GetEncrypted());
			logBuilder.AppendLine("Lives count after few operations with obscured value: " + obscured + " (" + obscured.ToString("X") + "h)");

			Debug.Log(logBuilder);
		}
	}
}