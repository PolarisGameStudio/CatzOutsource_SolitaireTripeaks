#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0) && !ENABLE_IL2CPP
#define ACTK_UWP_NO_IL2CPP
#endif

namespace CodeStage.AntiCheat.ObscuredTypes
{
	using Common;

	using System;
	using UnityEngine;
	using System.Runtime.InteropServices;
	using UnityEngine.Serialization;
	using Utils;

	/// <summary>
	/// Use it instead of regular <c>double</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredDouble : IFormattable, IEquatable<ObscuredDouble>, IComparable<ObscuredDouble>, IComparable<double>, IComparable
	{
		private static long cryptoKey = 210987L;

#if UNITY_EDITOR
		// For internal Editor usage only (may be useful for drawers).
		public static long cryptoKeyEditor = cryptoKey;
		public string migratedVersion;
#endif

		[SerializeField]
		private long currentCryptoKey;

		[SerializeField]
		private long hiddenValue;

		[SerializeField]
		[FormerlySerializedAs("hiddenValue")]
#pragma warning disable 414
		private ACTkByte8 hiddenValueOldByte8;
#pragma warning restore 414

		[SerializeField]
		private bool inited;

		[SerializeField]
		private double fakeValue;

		[SerializeField]
		private bool fakeValueActive;

		private ObscuredDouble(double value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = InternalEncrypt(value);
			hiddenValueOldByte8 = default(ACTkByte8);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
			migratedVersion = null;
#else
			var detectorRunning = Detectors.ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = detectorRunning ? value : 0L;
			fakeValueActive = detectorRunning;
#endif

			inited = true;
		}

		/// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetNewCryptoKey(long newKey)
		{
			cryptoKey = newKey;
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any double value, uses default crypto key.
		/// </summary>
		public static long Encrypt(double value)
		{
			return Encrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any double value, uses passed crypto key.
		/// </summary>
		public static long Encrypt(double value, long key)
		{
			var u = new DoubleLongBytesUnion {d = value};
			u.l = u.l ^ key;
			u.b8.Shuffle();
			return u.l;
		}

		private static long InternalEncrypt(double value, long key = 0L)
		{
			var currentKey = key;
			if (currentKey == 0L)
			{
				currentKey = cryptoKey;
			}

			return Encrypt(value, currentKey);
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(double) back to double, uses default crypto key.
		/// </summary>
		public static double Decrypt(long value)
		{
			return Decrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(double) back to double, uses passed crypto key.
		/// </summary>
		/// Make sure you're using key at least of 1000000000000000000 value to improve security.
		public static double Decrypt(long value, long key)
		{
			var u = new DoubleLongBytesUnion {l = value};
			u.b8.UnShuffle();
			u.l ^= key;
			return u.d;
		}

		/// <summary>
		/// Allows to update the encrypted value to the newer encryption format.
		/// </summary>
		/// Use when you have some encrypted values saved somewhere with previous ACTk version
		/// and you wish to set them using SetEncrypted() to the newer ACTk version obscured type.
		/// Current migration variants:
		/// from 0 or 1 to 2 - migrate obscured type from ACTk 1.5.2.0-1.5.8.0 to the 1.5.9.0+ format
		/// <param name="encrypted">Encrypted value you got from previous ACTk version obscured type with GetEncrypted().</param>
		/// <param name="fromVersion">Source format version.</param>
		/// <param name="toVersion">Target format version.</param>
		/// <returns>Migrated raw encrypted value which you may use for SetEncrypted(0 later.</returns>
		public static long MigrateEncrypted(long encrypted, byte fromVersion = 0, byte toVersion = 2)
		{
			var u = new DoubleLongBytesUnion {l = encrypted};

			if (fromVersion < 2 && toVersion == 2)
			{
				u.b8.Shuffle();
			}

			return u.l;
		}

		/// <summary>
		/// Creates and fills obscured variable with raw encrypted value previously got from GetEncrypted().
		/// </summary>
		/// Literally does same job as SetEncrypted() but makes new instance instead of filling existing one,
		/// making it easier to initialize new variables from saved encrypted values.
		///
		/// Make sure this obscured type currently has same crypto key as when encrypted value was got with GetEncrypted().
		/// It will be same (default) if you did not used SetNewCryptoKey().
		/// <param name="encrypted">Raw encrypted value you got from GetEncrypted().</param>
		/// <returns>New obscured variable initialized from specified encrypted value.</returns>
		/// \sa GetEncrypted(), SetEncrypted()
		public static ObscuredDouble FromEncrypted(long encrypted)
		{
			var instance = new ObscuredDouble();
			instance.SetEncrypted(encrypted);
			return instance;
		}

		/// <summary>
		/// Use it after SetNewCryptoKey() to re-encrypt current instance using new crypto key.
		/// </summary>
		public void ApplyNewCryptoKey()
		{
			if (currentCryptoKey != cryptoKey)
			{
				hiddenValue = InternalEncrypt(InternalDecrypt(), cryptoKey);
				currentCryptoKey = cryptoKey;
			}
		}

		/// <summary>
		/// Allows to change current crypto key to the new random value and re-encrypt variable using it.
		/// Use it for extra protection against 'unknown value' search.
		/// Just call it sometimes when your variable doesn't change to fool the cheater.
		/// </summary>
		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();
			currentCryptoKey = ThreadSafeRandom.Next(100000, 999999);
			hiddenValue = InternalEncrypt(decrypted, currentCryptoKey);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public long GetEncrypted()
		{
			ApplyNewCryptoKey();
			return hiddenValue;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(long encrypted)
		{
			inited = true;
			hiddenValue = encrypted;

			if (currentCryptoKey == 0)
			{
				currentCryptoKey = cryptoKey;
			}

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				fakeValueActive = false;
				fakeValue = InternalDecrypt();
				fakeValueActive = true;
			}
			else
			{
				fakeValueActive = false;
			}
		}

		/// <summary>
		/// Alternative to the type cast, use if you wish to get decrypted value 
		/// but can't or don't want to use cast to the regular type.
		/// </summary>
		/// <returns>Decrypted value.</returns>
		public double GetDecrypted()
		{
			return InternalDecrypt();
		}

		private double InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0);
				fakeValue = 0;
				fakeValueActive = false;
				inited = true;

				return 0;
			}

#if ACTK_OBSCURED_AUTO_MIGRATION
			if (hiddenValueOldByte8.b1 != 0 ||
			    hiddenValueOldByte8.b2 != 0 ||
			    hiddenValueOldByte8.b3 != 0 ||
			    hiddenValueOldByte8.b4 != 0 ||
			    hiddenValueOldByte8.b5 != 0 ||
			    hiddenValueOldByte8.b6 != 0 ||
			    hiddenValueOldByte8.b7 != 0 ||
			    hiddenValueOldByte8.b8 != 0)
			{
				var union = new DoubleLongBytesUnion() { b8 = hiddenValueOldByte8 };
				union.b8.Shuffle();
				hiddenValue = union.l;

				hiddenValueOldByte8.b1 = 0;
				hiddenValueOldByte8.b2 = 0;
				hiddenValueOldByte8.b3 = 0;
				hiddenValueOldByte8.b4 = 0;
				hiddenValueOldByte8.b5 = 0;
				hiddenValueOldByte8.b6 = 0;
				hiddenValueOldByte8.b7 = 0;
				hiddenValueOldByte8.b8 = 0;
			}
#endif

			var decrypted = Decrypt(hiddenValue, currentCryptoKey);
			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue) > Detectors.ObscuredCheatingDetector.Instance.doubleEpsilon)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

#region operators, overrides, interface implementations

		//! @cond
		public static implicit operator ObscuredDouble(double value)
		{
			return new ObscuredDouble(value);
		}

		public static implicit operator double(ObscuredDouble value)
		{
			return value.InternalDecrypt();
		}

		public static explicit operator ObscuredDouble(ObscuredFloat f)
		{
			return (float)f;
		}

		public static ObscuredDouble operator ++(ObscuredDouble input)
		{
			var decrypted = input.InternalDecrypt() + 1d;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				input.fakeValue = decrypted;
				input.fakeValueActive = true;
			}
			else
			{
				input.fakeValueActive = false;
			}

			return input;
		}

		public static ObscuredDouble operator --(ObscuredDouble input)
		{
			var decrypted = input.InternalDecrypt() - 1d;
			input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
			{
				input.fakeValue = decrypted;
				input.fakeValueActive = true;
			}
			else
			{
				input.fakeValueActive = false;
			}

			return input;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// 
		/// <returns>
		/// A 32-bit signed integer hash code.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string ToString()
		{
			return InternalDecrypt().ToString();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation, using the specified format.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="format"/>.
		/// </returns>
		/// <param name="format">A numeric format string (see Remarks).</param><exception cref="T:System.FormatException"><paramref name="format"/> is invalid. </exception><filterpriority>1</filterpriority>
		public string ToString(string format)
		{
			return InternalDecrypt().ToString(format);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified culture-specific format information.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="provider"/>.
		/// </returns>
		/// <param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information. </param><filterpriority>1</filterpriority>
		public string ToString(IFormatProvider provider)
		{
			return InternalDecrypt().ToString(provider);
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of the value of this instance as specified by <paramref name="format"/> and <paramref name="provider"/>.
		/// </returns>
		/// <param name="format">A numeric format string (see Remarks).</param><param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information. </param><filterpriority>1</filterpriority>
		public string ToString(string format, IFormatProvider provider)
		{
			return InternalDecrypt().ToString(format, provider);
		}

		/// <summary>
		/// Returns a value indicating whether this instance is equal to a specified object.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is an instance of ObscuredDouble and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredDouble))
				return false;
			return Equals((ObscuredDouble)obj);
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified ObscuredDouble object represent the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is equal to this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An ObscuredDouble object to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredDouble obj)
		{
			return obj.InternalDecrypt().Equals(InternalDecrypt());
		}

		public int CompareTo(ObscuredDouble other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(double other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
#if !ACTK_UWP_NO_IL2CPP
			return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is double)) throw new ArgumentException("Argument must be double");
			return CompareTo((double)obj);
#endif
		}
		//! @endcond

#endregion

		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleLongBytesUnion
		{
			[FieldOffset(0)]
			public double d;

			[FieldOffset(0)]
			public long l;

			[FieldOffset(0)]
			public ACTkByte8 b8;
		}
	}
}