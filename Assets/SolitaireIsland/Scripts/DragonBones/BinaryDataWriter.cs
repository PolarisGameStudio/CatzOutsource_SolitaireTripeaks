using System.IO;
using System.Text;

namespace DragonBones
{
	internal class BinaryDataWriter : BinaryWriter
	{
		private long Length => BaseStream.Length;

		public BinaryDataWriter()
			: base(new MemoryStream(256))
		{
		}

		internal BinaryDataWriter(Stream stream)
			: base(stream)
		{
		}

		public BinaryDataWriter(Encoding encoding)
			: base(new MemoryStream(256), encoding)
		{
		}

		internal BinaryDataWriter(Stream stream, Encoding encoding)
			: base(stream, encoding)
		{
		}

		public virtual void Write(bool[] value)
		{
			foreach (bool value2 in value)
			{
				base.Write(value2);
			}
		}

		public override void Write(byte[] value)
		{
			foreach (byte value2 in value)
			{
				base.Write(value2);
			}
		}

		public override void Write(char[] value)
		{
			foreach (char ch in value)
			{
				base.Write(ch);
			}
		}

		public virtual void Write(decimal[] value)
		{
			foreach (decimal value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(double[] value)
		{
			foreach (double value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(short[] value)
		{
			foreach (short value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(int[] value)
		{
			foreach (int value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(long[] value)
		{
			foreach (long value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(sbyte[] value)
		{
			foreach (sbyte value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(float[] value)
		{
			foreach (float value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(string[] value)
		{
			foreach (string value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(ushort[] value)
		{
			foreach (ushort value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(uint[] value)
		{
			foreach (uint value2 in value)
			{
				base.Write(value2);
			}
		}

		public virtual void Write(ulong[] value)
		{
			foreach (ulong value2 in value)
			{
				base.Write(value2);
			}
		}
	}
}
