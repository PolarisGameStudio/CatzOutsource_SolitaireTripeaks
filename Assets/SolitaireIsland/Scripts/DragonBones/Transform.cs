using System;

namespace DragonBones
{
	public class Transform
	{
		public static readonly float PI = 3.141593f;

		public static readonly float PI_D = PI * 2f;

		public static readonly float PI_H = PI / 2f;

		public static readonly float PI_Q = PI / 4f;

		public static readonly float RAD_DEG = 180f / PI;

		public static readonly float DEG_RAD = PI / 180f;

		public float x;

		public float y;

		public float skew;

		public float rotation;

		public float scaleX = 1f;

		public float scaleY = 1f;

		public static float NormalizeRadian(float value)
		{
			value = (value + PI) % (PI * 2f);
			value += ((!(value > 0f)) ? PI : (0f - PI));
			return value;
		}

		public override string ToString()
		{
			return "[object dragonBones.Transform] x:" + x + " y:" + y + " skew:" + (double)skew * 180.0 / (double)PI + " rotation:" + (double)rotation * 180.0 / (double)PI + " scaleX:" + scaleX + " scaleY:" + scaleY;
		}

		public Transform CopyFrom(Transform value)
		{
			x = value.x;
			y = value.y;
			skew = value.skew;
			rotation = value.rotation;
			scaleX = value.scaleX;
			scaleY = value.scaleY;
			return this;
		}

		public Transform Identity()
		{
			x = (y = 0f);
			skew = (rotation = 0f);
			scaleX = (scaleY = 1f);
			return this;
		}

		public Transform Add(Transform value)
		{
			x += value.x;
			y += value.y;
			skew += value.skew;
			rotation += value.rotation;
			scaleX *= value.scaleX;
			scaleY *= value.scaleY;
			return this;
		}

		public Transform Minus(Transform value)
		{
			x -= value.x;
			y -= value.y;
			skew -= value.skew;
			rotation -= value.rotation;
			scaleX /= value.scaleX;
			scaleY /= value.scaleY;
			return this;
		}

		public Transform FromMatrix(Matrix matrix)
		{
			float num = scaleX;
			float num2 = scaleY;
			x = matrix.tx;
			y = matrix.ty;
			float num3 = (float)Math.Atan((0f - matrix.c) / matrix.d);
			rotation = (float)Math.Atan(matrix.b / matrix.a);
			if (float.IsNaN(num3))
			{
				num3 = 0f;
			}
			if (float.IsNaN(rotation))
			{
				rotation = 0f;
			}
			scaleX = (float)((!(rotation > 0f - PI_Q) || !(rotation < PI_Q)) ? ((double)matrix.b / Math.Sin(rotation)) : ((double)matrix.a / Math.Cos(rotation)));
			scaleY = (float)((!(num3 > 0f - PI_Q) || !(num3 < PI_Q)) ? ((double)(0f - matrix.c) / Math.Sin(num3)) : ((double)matrix.d / Math.Cos(num3)));
			if (num >= 0f && scaleX < 0f)
			{
				scaleX = 0f - scaleX;
				rotation -= PI;
			}
			if (num2 >= 0f && scaleY < 0f)
			{
				scaleY = 0f - scaleY;
				num3 -= PI;
			}
			skew = num3 - rotation;
			return this;
		}

		public Transform ToMatrix(Matrix matrix)
		{
			if (rotation == 0f)
			{
				matrix.a = 1f;
				matrix.b = 0f;
			}
			else
			{
				matrix.a = (float)Math.Cos(rotation);
				matrix.b = (float)Math.Sin(rotation);
			}
			if (skew == 0f)
			{
				matrix.c = 0f - matrix.b;
				matrix.d = matrix.a;
			}
			else
			{
				matrix.c = 0f - (float)Math.Sin(skew + rotation);
				matrix.d = (float)Math.Cos(skew + rotation);
			}
			if (scaleX != 1f)
			{
				matrix.a *= scaleX;
				matrix.b *= scaleX;
			}
			if (scaleY != 1f)
			{
				matrix.c *= scaleY;
				matrix.d *= scaleY;
			}
			matrix.tx = x;
			matrix.ty = y;
			return this;
		}
	}
}
