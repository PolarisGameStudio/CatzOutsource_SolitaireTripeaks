using System;

namespace DragonBones
{
	internal class IKConstraint : Constraint
	{
		internal bool _scaleEnabled;

		internal bool _bendPositive;

		internal float _weight;

		protected override void _OnClear()
		{
			base._OnClear();
			_scaleEnabled = false;
			_bendPositive = false;
			_weight = 1f;
			_constraintData = null;
		}

		private void _ComputeA()
		{
			Transform global = _target.global;
			Transform global2 = _root.global;
			Matrix globalTransformMatrix = _root.globalTransformMatrix;
			float num = (float)Math.Atan2(global.y - global2.y, global.x - global2.x);
			if (global2.scaleX < 0f)
			{
				num += 3.14159274f;
			}
			global2.rotation += Transform.NormalizeRadian(num - global2.rotation) * _weight;
			global2.ToMatrix(globalTransformMatrix);
		}

		private void _ComputeB()
		{
			float length = _bone.boneData.length;
			Bone root = _root;
			Transform global = _target.global;
			Transform global2 = root.global;
			Transform global3 = _bone.global;
			Matrix globalTransformMatrix = _bone.globalTransformMatrix;
			float num = globalTransformMatrix.a * length;
			float num2 = globalTransformMatrix.b * length;
			float num3 = num * num + num2 * num2;
			float num4 = (float)Math.Sqrt(num3);
			float num5 = global3.x - global2.x;
			float num6 = global3.y - global2.y;
			float num7 = num5 * num5 + num6 * num6;
			float num8 = (float)Math.Sqrt(num7);
			float rotation = global3.rotation;
			float rotation2 = global2.rotation;
			float num9 = (float)Math.Atan2(num6, num5);
			num5 = global.x - global2.x;
			num6 = global.y - global2.y;
			float num10 = num5 * num5 + num6 * num6;
			float num11 = (float)Math.Sqrt(num10);
			float num12 = 0f;
			if (num4 + num8 <= num11 || num11 + num4 <= num8 || num11 + num8 <= num4)
			{
				num12 = (float)Math.Atan2(global.y - global2.y, global.x - global2.x);
				if (!(num4 + num8 <= num11) && num8 < num4)
				{
					num12 += 3.14159274f;
				}
			}
			else
			{
				float num13 = (num7 - num3 + num10) / (2f * num10);
				float num14 = (float)Math.Sqrt(num7 - num13 * num13 * num10) / num11;
				float num15 = global2.x + num5 * num13;
				float num16 = global2.y + num6 * num13;
				float num17 = (0f - num6) * num14;
				float num18 = num5 * num14;
				bool flag = false;
				Bone parent = root.parent;
				if (parent != null)
				{
					Matrix globalTransformMatrix2 = parent.globalTransformMatrix;
					flag = (globalTransformMatrix2.a * globalTransformMatrix2.d - globalTransformMatrix2.b * globalTransformMatrix2.c < 0f);
				}
				if (flag != _bendPositive)
				{
					global3.x = num15 - num17;
					global3.y = num16 - num18;
				}
				else
				{
					global3.x = num15 + num17;
					global3.y = num16 + num18;
				}
				num12 = (float)Math.Atan2(global3.y - global2.y, global3.x - global2.x);
			}
			float num19 = Transform.NormalizeRadian(num12 - num9);
			global2.rotation = rotation2 + num19 * _weight;
			global2.ToMatrix(root.globalTransformMatrix);
			float num20 = num9 + num19 * _weight;
			global3.x = global2.x + (float)Math.Cos(num20) * num8;
			global3.y = global2.y + (float)Math.Sin(num20) * num8;
			float num21 = (float)Math.Atan2(global.y - global3.y, global.x - global3.x);
			if (global3.scaleX < 0f)
			{
				num21 += 3.14159274f;
			}
			global3.rotation = global2.rotation + rotation - rotation2 + Transform.NormalizeRadian(num21 - num19 - rotation) * _weight;
			global3.ToMatrix(globalTransformMatrix);
		}

		public override void Init(ConstraintData constraintData, Armature armature)
		{
			if (_constraintData == null)
			{
				_constraintData = constraintData;
				_armature = armature;
				_target = _armature.GetBone(_constraintData.target.name);
				_root = _armature.GetBone(_constraintData.root.name);
				_bone = ((_constraintData.bone == null) ? null : _armature.GetBone(_constraintData.bone.name));
				IKConstraintData iKConstraintData = _constraintData as IKConstraintData;
				_scaleEnabled = iKConstraintData.scaleEnabled;
				_bendPositive = iKConstraintData.bendPositive;
				_weight = iKConstraintData.weight;
				_root._hasConstraint = true;
			}
		}

		public override void Update()
		{
			_root.UpdateByConstraint();
			if (_bone != null)
			{
				_bone.UpdateByConstraint();
				_ComputeB();
			}
			else
			{
				_ComputeA();
			}
		}

		public override void InvalidUpdate()
		{
			_root.InvalidUpdate();
			if (_bone != null)
			{
				_bone.InvalidUpdate();
			}
		}
	}
}
