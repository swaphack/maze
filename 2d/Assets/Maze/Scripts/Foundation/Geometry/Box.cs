using System;
using System.Collections.Generic;

using UnityEngine;

namespace Geometry
{
	public interface IBoxItem
	{
		T GetKey<T>();
	}

	public class Box<T> where T : IBoxItem
	{			
		private HashSet<T> _items;
		
		public Box ()
		{
			_items = new HashSet<T> ();
		}

		public void Add(T item)
		{
			_items.Add (item);
		}

		public bool Contains(T item)
		{
			return _items.Contains (item);
		}

		public void Remove(T item)
		{
			_items.Remove (item);
		}

		public void Clear()
		{
			_items.Clear ();
		}
	}

	public class BoxContains<T> where T : IBoxItem
	{
		internal class BoxInfo
		{
			/// <summary>
			/// 索引
			/// </summary>
			private int _index;
			/// <summary>
			/// 范围
			/// </summary>
			private Range _range;
			/// <summary>
			/// 盒子物品数据
			/// </summary>
			private Box<T> _box;

			public int Index { get { return _index; } set  { _index = value;} }
			public Range Range { get { return _range; } set  { _range = value;} }
			public Box<T> Box { get { return _box; } }


			public BoxInfo(float min, float max)
			{
				_index = -1;
				_range = new Range(min, max);
				_box = new Box<T>();
			}
		}

		/// <summary>
		/// 每组箱子数据
		/// </summary>
		private List<BoxInfo> _boxInfos;

		/// <summary>
		/// 每个箱子的物品数量
		/// </summary>
		private int _boxItemCount;
		/// <summary>
		/// 箱子间的距离
		/// </summary>
		private float _boxDistance;
		/// <summary>
		/// 每个箱子的物品数量
		/// </summary>
		/// <value>The box item count.</value>
		public int BoxItemCount { get {  return _boxItemCount;} set { _boxItemCount = value; } }
		/// <summary>
		/// 箱子间的距离
		/// </summary>
		/// <value>The box distance.</value>
		public float BoxDistance { get {  return _boxDistance;} set { _boxDistance = value; } }

		public BoxContains()
		{
			_boxInfos = new List<BoxInfo> ();
		}

		public void Add(T item)
		{
			/*
			int index = -1;
			if (_boxInfos.Count == 0) {
				float min = item.GetKey<float> ();
				float max = min + BoxDistance;
				_boxInfos.Add (new BoxInfo (min, max));
				_boxInfos [i].Box.Add (item);
			} else {
				float value = item.GetKey<float> ();
				int count = _boxInfos.Count;
				for (int i = 0; i < count; i++) {
					if (_boxInfos[i].Range.Contains(value)) {
						_boxInfos [i].Box.Add (item);
						return;
					}

					if (value < _boxInfos [i].Range.Min) {
						index = i;
						break;
					}
				}
			}
			if (index == -1) {
				return;
			}
			*/
		}

		public bool Contains(T item)
		{
			foreach(var boxInfo in _boxInfos) {
				if (boxInfo.Box.Contains(item)) {
					return true;
				}
			}
			return false;
		}

		public void Remove(T item)
		{
			for(int i = 0; i < _boxInfos.Count; i++) {
				_boxInfos[i].Box.Remove (item);
			}
		}

		public void Clear()
		{
			_boxInfos.Clear ();
		}
	}
}

