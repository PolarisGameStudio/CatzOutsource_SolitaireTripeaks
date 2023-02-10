using System.IO;
using System.Text;

namespace DragonBones
{
	internal class BinaryDataReader : BinaryReader
	{
		private int i;

		private int readLength;

		private long Length => BaseStream.Length;

		internal BinaryDataReader(Stream stream)
			: base(stream)
		{
		}

		internal BinaryDataReader(Stream stream, Encoding encoding)
			: base(stream, encoding)
		{
		}

		public virtual void Seek(int offset, SeekOrigin origin = SeekOrigin.Current)
		{
			if (offset != 0)
			{
				BaseStream.Seek(offset, origin);
			}
		}

		public virtual bool[] ReadBooleans(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			bool[] array = new bool[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadBoolean();
			}
			return array;
		}

		public virtual byte[] ReadBytes(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			byte[] array = new byte[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadByte();
			}
			return array;
		}

		public virtual char[] ReadChars(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			char[] array = new char[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadChar();
			}
			return array;
		}

		public virtual decimal[] ReadDecimals(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			decimal[] array = new decimal[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadDecimal();
			}
			return array;
		}

		public virtual double[] ReadDoubles(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			double[] array = new double[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadDouble();
			}
			return array;
		}

		public virtual short[] ReadInt16s(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			short[] array = new short[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadInt16();
			}
			return array;
		}

		public virtual int[] ReadInt32s(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			int[] array = new int[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadInt32();
			}
			return array;
		}

		public virtual long[] ReadInt64s(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			long[] array = new long[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadInt64();
			}
			return array;
		}

		public virtual sbyte[] ReadSBytes(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			sbyte[] array = new sbyte[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadSByte();
			}
			return array;
		}

		public virtual float[] ReadSingles(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			float[] array = new float[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadSingle();
			}
			return array;
		}

		public virtual string[] ReadStrings(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			string[] array = new string[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadString();
			}
			return array;
		}

		public virtual ushort[] ReadUInt16s(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			ushort[] array = new ushort[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadUInt16();
			}
			return array;
		}

		public virtual uint[] ReadUInt32s(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			uint[] array = new uint[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadUInt32();
			}
			return array;
		}

		public virtual ulong[] ReadUInt64s(int offset, int readLength)
		{
			Seek(offset);
			this.readLength = readLength;
			ulong[] array = new ulong[this.readLength];
			for (i = 0; i < this.readLength; i++)
			{
				array[i] = base.ReadUInt64();
			}
			return array;
		}
	}
}
