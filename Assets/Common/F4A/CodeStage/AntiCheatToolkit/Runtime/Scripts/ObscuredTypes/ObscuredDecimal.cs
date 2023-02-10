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
	using System.Runtime.InteropServices;
	using UnityEngine;
	using Utils;

	/// <summary>
	/// Use it instead of regular <c>decimal</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public struct ObscuredDecimal : IFormattable, IEquatable<ObscuredDecimal>, IComparable<ObscuredDecimal>, IComparable<decimal>, IComparable
	{
		private static long cryptoKey = 209208L;

#if UNITY_EDITOR
		// For internal Editor usage only (may be useful for drawers).
		public static long cryptoKeyEditor = cryptoKey;
#endif

		[SerializeField]
		private long currentCryptoKey;

		[SerializeField]
		private ACTkByte16 hiddenValue;

		[SerializeField]
		private bool inited;

		private decimal fakeValue;

		[SerializeField]
		private bool fakeValueActive;

		private ObscuredDecimal(decimal value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = InternalEncrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
			var detectorRunning = Detectors.ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = detectorRunning ? value : 0m;
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
		/// Use this simple encryption method to encrypt any decimal value, uses default crypto key.
		/// </summary>
		public static decimal Encrypt(decimal value)
		{
			return Encrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use this simple encryption method to encrypt any decimal value, uses passed crypto key.
		/// </summary>
		public static decimal Encrypt(decimal value, long key)
		{
			var u = new DecimalLongBytesUnion();
			u.d = value;
			u.l1 = u.l1 ^ key;
			u.l2 = u.l2 ^ key;

			return u.d;
		}

		private static ACTkByte16 InternalEncrypt(decimal value)
		{
			return InternalEncrypt(value, 0L);
		}

		private static ACTkByte16 InternalEncrypt(decimal value, long key)
		{
			var currentKey = key;
			if (currentKey == 0L)
			{
				currentKey = cryptoKey;
			}

			var union = new DecimalLongBytesUnion();
			union.d = value;
			union.l1 = union.l1 ^ currentKey;
			union.l2 = union.l2 ^ currentKey;

			return union.b16;
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(decimal) back to decimal, uses default crypto key.
		/// </summary>
		public static decimal Decrypt(decimal value)
		{
			return Decrypt(value, cryptoKey);
		}

		/// <summary>
		/// Use it to decrypt long you got from Encrypt(decimal) back to decimal, uses passed crypto key.
		/// </summary>
		public static decimal Decrypt(decimal value, long key)
		{
			var u = new DecimalLongBytesUnion();
			u.d = value;
			u.l1 = u.l1 ^ key;
			u.l2 = u.l2 ^ key;
			return u.d;
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
		public static ObscuredDecimal FromEncrypted(decimal encrypted)
		{
			var instance = new ObscuredDecimal();
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
			currentCryptoKey = ThreadSafeRandom.Next();
			hiddenValue = InternalEncrypt(decrypted, currentCryptoKey);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public decimal GetEncrypted()
		{
			ApplyNewCryptoKey();

			var union = new DecimalLongBytesUnion();
			union.b16 = hiddenValue;

			return union.d;
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(decimal encrypted)
		{
			inited = true;
			var union = new DecimalLongBytesUnion();
			union.d = encrypted;

			hiddenValue = union.b16;

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
		public decimal GetDecrypted()
		{
			return InternalDecrypt();
		}

		private decimal InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(0m);
				fakeValue = 0m;
				fakeValueActive = false;
				inited = true;

				return 0m;
			}

			var union = new DecimalLongBytesUnion();
			union.b16 = hiddenValue;

			union.l1 = union.l1 ^ currentCryptoKey;
			union.l2 = union.l2 ^ currentCryptoKey;

			var decrypted = union.d;

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && decrypted != fakeValue)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return decrypted;
		}

#region operators, overrides, interface implementations
		//! @cond
		public static implicit operator ObscuredDecimal(decimal value)
		{
			return new ObscuredDecimal(value);
		}

		public static implicit operator decimal(ObscuredDecimal value)
		{
			return value.InternalDecrypt();
		}

		public static explicit operator ObscuredDecimal(ObscuredFloat f)
		{
			return (decimal)(float)f;
		}

		public static ObscuredDecimal operator ++(ObscuredDecimal input)
		{
			var decrypted = input.InternalDecrypt() + 1m;
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

		public static ObscuredDecimal operator --(ObscuredDecimal input)
		{
			var decrypted = input.InternalDecrypt() - 1m;
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
		/// true if <paramref name="obj"/> is an instance of ObscuredDecimal and equals the value of this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An object to compare with this instance. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredDecimal))
				return false;
			return Equals((ObscuredDecimal)obj);
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="T:System.Decimal"/> object represent the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is equal to this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">A <see cref="T:System.Decimal"/> object to compare to this instance.</param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredDecimal obj)
		{
			return obj.InternalDecrypt().Equals(InternalDecrypt());
		}

		public int CompareTo(ObscuredDecimal other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(decimal other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
#if !ACTK_UWP_NO_IL2CPP
			return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is decimal)) throw new ArgumentException("Argument must be decimal");
			return CompareTo((decimal)obj);
#endif
		}

		//! @endcond
#endregion

		[StructLayout(LayoutKind.Explicit)]
		private struct DecimalLongBytesUnion
		{
			[FieldOffset(0)]
			public decimal d;

			[FieldOffset(0)]
			public long l1;

			[FieldOffset(8)]
			public long l2;

			[FieldOffset(0)]
			public ACTkByte16 b16;
		}
	}
}