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
	using System;
	using UnityEngine;
	using Utils;

	/// <summary>
	/// Use it instead of regular <c>string</c> for any cheating-sensitive variables.
	/// </summary>
	/// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
	[Serializable]
	public sealed class ObscuredString : IComparable<ObscuredString>, IComparable<string>, IComparable
	{
		private static string cryptoKey = "4441";

#if UNITY_EDITOR
		// For internal Editor usage only (may be useful for drawers).
		public static string cryptoKeyEditor = cryptoKey;
#endif

		[SerializeField]
		private string currentCryptoKey;

		[SerializeField]
		private byte[] hiddenValue;

		[SerializeField]
		private bool inited;

		[SerializeField]
		private string fakeValue;

		[SerializeField]
		private bool fakeValueActive;

		// for serialization purposes
		private ObscuredString(){}

		private ObscuredString(string value)
		{
			currentCryptoKey = cryptoKey;
			hiddenValue = InternalEncrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
			var detectorRunning = Detectors.ObscuredCheatingDetector.ExistsAndIsRunning;
			fakeValue = detectorRunning ? value : null;
			fakeValueActive = detectorRunning;
#endif

			inited = true;
		}

		/// <summary>
		/// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
		/// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
		/// </summary>
		public static void SetNewCryptoKey(string newKey)
		{
			cryptoKey = newKey;
		}

		/// <summary>
		/// Simple symmetric encryption, uses default crypto key.
		/// </summary>
		/// <returns>Encrypted or decrypted <c>string</c> (depending on what <c>string</c> was passed to the function)</returns>
		public static string EncryptDecrypt(string value)
		{
			return EncryptDecrypt(value, string.Empty);
		}

		/// <summary>
		/// Simple symmetric encryption, uses passed crypto key.
		/// </summary>
		/// <returns>Encrypted or decrypted <c>string</c> (depending on what <c>string</c> was passed to the function)</returns>
		public static string EncryptDecrypt(string value, string key)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}

			if (string.IsNullOrEmpty(key))
			{
				key = cryptoKey;
			}

			var keyLength = key.Length;
			var valueLength = value.Length;

			var result = new char[valueLength];

			for (var i = 0; i < valueLength; i++)
			{
				result[i] = (char)(value[i] ^ key[i % keyLength]);
			}

			return new string(result);
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
		public static ObscuredString FromEncrypted(string encrypted)
		{
			var instance = new ObscuredString();
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
				hiddenValue = InternalEncrypt(InternalDecrypt());
				currentCryptoKey = cryptoKey;
			}
		}

		/// <summary>
		/// Allows to change current crypto key to the new random value and re-encrypt variable using it.
		/// Use it for extra protection against 'unknown value' search.
		/// Just call it sometimes when your variable doesn't change to fool the cheater.
		/// </summary>
		/// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly produces some garbage, be careful when using it!</strong>
		public void RandomizeCryptoKey()
		{
			var decrypted = InternalDecrypt();

			currentCryptoKey = ThreadSafeRandom.Next().ToString();
			hiddenValue = InternalEncrypt(decrypted, currentCryptoKey);
		}

		/// <summary>
		/// Allows to pick current obscured value as is.
		/// </summary>
		/// Use it in conjunction with SetEncrypted().<br/>
		/// Useful for saving data in obscured state.
		public string GetEncrypted()
		{
			ApplyNewCryptoKey();
			return GetString(hiddenValue);
		}

		/// <summary>
		/// Allows to explicitly set current obscured value. Crypto key should be same as when encrypted value was got with GetEncrypted().
		/// </summary>
		/// Use it in conjunction with GetEncrypted().<br/>
		/// Useful for loading data stored in obscured state.
		/// \sa FromEncrypted()
		public void SetEncrypted(string encrypted)
		{
			inited = true;
			hiddenValue = GetBytes(encrypted);

			if (string.IsNullOrEmpty(currentCryptoKey))
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
		public string GetDecrypted()
		{
			return InternalDecrypt();
		}

		private static byte[] InternalEncrypt(string value)
		{
			return InternalEncrypt(value, cryptoKey);
		}

		private static byte[] InternalEncrypt(string value, string key)
		{
			return GetBytes(EncryptDecrypt(value, key));
		}

		private string InternalDecrypt()
		{
			if (!inited)
			{
				currentCryptoKey = cryptoKey;
				hiddenValue = InternalEncrypt(string.Empty);
				fakeValue = string.Empty;
				fakeValueActive = false;
				inited = true;

				return string.Empty;
			}

			var key = currentCryptoKey;
			if (string.IsNullOrEmpty(key))
			{
				key = cryptoKey;
			}

			var result = EncryptDecrypt(GetString(hiddenValue), key);

			if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && result != fakeValue)
			{
				Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}

			return result;
		}

#region operators and overrides

		// ----------------------------------------------------------------------------
		// operators, overrides, proxies
		// ----------------------------------------------------------------------------

		//! @cond

		public int Length
		{
			get { return hiddenValue.Length / sizeof(char); }
		}

		public static implicit operator ObscuredString(string value)
		{
			return value == null ? null : new ObscuredString(value);
		}

		public static implicit operator string(ObscuredString value)
		{
			return value == null ? null : value.InternalDecrypt();
		}

		/// <summary>
		/// Determines whether two specified ObscuredStrings have the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if the value of <paramref name="a"/> is the same as the value of <paramref name="b"/>; otherwise, false.
		/// </returns>
		/// <param name="a">An ObscuredString or null. </param><param name="b">An ObscuredString or null. </param><filterpriority>3</filterpriority>
		public static bool operator ==(ObscuredString a, ObscuredString b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			if ((object)a == null || (object)b == null)
			{
				return false;
			}

			if (a.currentCryptoKey == b.currentCryptoKey)
			{
				return ArraysEquals(a.hiddenValue, b.hiddenValue);
			}

			return string.Equals(a.InternalDecrypt(), b.InternalDecrypt());
		}

		/// <summary>
		/// Determines whether two specified ObscuredStrings have different values.
		/// </summary>
		/// 
		/// <returns>
		/// true if the value of <paramref name="a"/> is different from the value of <paramref name="b"/>; otherwise, false.
		/// </returns>
		/// <param name="a">An ObscuredString or null. </param><param name="b">An ObscuredString or null. </param><filterpriority>3</filterpriority>
		public static bool operator !=(ObscuredString a, ObscuredString b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Returns the hash code for this ObscuredString.
		/// </summary>
		/// 
		/// <returns>
		/// A 32-bit signed integer hash code.
		/// </returns>
		public override int GetHashCode()
		{
			return InternalDecrypt().GetHashCode();
		}

		/// <summary>
		/// Overrides default ToString to provide easy implicit conversion to the <c>string</c>.
		/// </summary>
		public override string ToString()
		{
			return InternalDecrypt();
		}

		/// <summary>
		/// Determines whether this instance of ObscuredString and a specified object, which must also be a ObscuredString object, have the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if <paramref name="obj"/> is a ObscuredString and its value is the same as this instance; otherwise, false.
		/// </returns>
		/// <param name="obj">An <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (!(obj is ObscuredString))
				return false;

			return Equals((ObscuredString)obj);
		}

		/// <summary>
		/// Determines whether this instance and another specified ObscuredString object have the same value.
		/// </summary>
		/// 
		/// <returns>
		/// true if the value of the <paramref name="value"/> parameter is the same as this instance; otherwise, false.
		/// </returns>
		/// <param name="value">A ObscuredString. </param><filterpriority>2</filterpriority>
		public bool Equals(ObscuredString value)
		{
			if (value == null) return false;

			if (currentCryptoKey == value.currentCryptoKey)
			{
				return ArraysEquals(hiddenValue, value.hiddenValue);
			}

			return string.Equals(InternalDecrypt(), value.InternalDecrypt());
		}

		/// <summary>
		/// Determines whether this string and a specified ObscuredString object have the same value. A parameter specifies the culture, case, and sort rules used in the comparison.
		/// </summary>
		/// 
		/// <returns>
		/// true if the value of the <paramref name="value"/> parameter is the same as this string; otherwise, false.
		/// </returns>
		/// <param name="value">An ObscuredString to compare.</param><param name="comparisonType">A value that defines the type of comparison. </param><exception cref="T:System.ArgumentException"><paramref name="comparisonType"/> is not a <see cref="T:System.StringComparison"/> value. </exception><filterpriority>2</filterpriority>
		public bool Equals(ObscuredString value, StringComparison comparisonType)
		{
			if (value == null) return false;

			return string.Equals(InternalDecrypt(), value.InternalDecrypt(), comparisonType);
		}

		public int CompareTo(ObscuredString other)
		{
			return InternalDecrypt().CompareTo(other.InternalDecrypt());
		}

		public int CompareTo(string other)
		{
			return InternalDecrypt().CompareTo(other);
		}

		public int CompareTo(object obj)
		{
#if !ACTK_UWP_NO_IL2CPP
			return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is string)) throw new ArgumentException("Argument must be string");
			return CompareTo((string)obj);
#endif
		}

		//! @endcond
#endregion

		private static byte[] GetBytes(string str)
		{
			var bytes = new byte[str.Length * sizeof(char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		private static string GetString(byte[] bytes)
		{
			var chars = new char[bytes.Length / sizeof(char)];
			Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}

		private static bool ArraysEquals(byte[] a1, byte[] a2)
		{
			if (a1 == a2)
			{
				return true;
			}

			if ((a1 != null) && (a2 != null))
			{
				if (a1.Length != a2.Length)
				{
					return false;
				}
				for (var i = 0; i < a1.Length; i++)
				{
					if (a1[i] != a2[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}
}