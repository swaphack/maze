using System;

namespace Geometry
{
	public class Constraints
	{
		/// <summary>
		/// 误差范围
		/// </summary>
		public const float Eps = 0.001f;
	}

	public struct Range
	{
		/// <summary>
		/// 包含类型
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// 包含
			/// </summary>
			Include,
			/// <summary>
			/// 不包含
			/// </summary>
			Exclude,
		}
		
		/// <summary>
		/// 最小值
		/// </summary>
		public float Min;
		/// <summary>
		/// 最大值
		/// </summary>
		public float Max;
		/// <summary>
		/// 最小值包含类型
		/// </summary>
		public Type MinType;
		/// <summary>
		/// 最大值包含类型
		/// </summary>
		public Type MaxType;

		/// <summary>
		/// 默认左包含，右不包含
		/// </summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public Range(float min, float max)
			:this(min,max, Type.Include, Type.Exclude)
		{
		}

		public Range(float min, float max, Type minType, Type maxType)
		{
			this.Min = min;
			this.Max = max;
			this.MinType = minType;
			this.MaxType = maxType;
		}

		public bool Contains(float value)
		{
			if (value < Min || value > Max) {
				return false;
			}
			if (MinType == Type.Include && value != Min) {
				return false;
			}

			if (MaxType == Type.Include && value != Max) {
				return false;
			}

			return true;
		}
	}
}

