using UnityEngine;
using System.Collections;

public static class UIExtensions
{
	/// <summary>
	/// 获取子节点
	/// </summary>
	/// <returns>The child.</returns>
	/// <param name="t">T.</param>
	/// <param name="name">Name.</param>
	public static Component GetChild(this Component t, string name)
	{
		if (t == null) {
			return null;
		}

		var childCount = t.transform.childCount;
		for (var i = 0; i < childCount; i++) {
			var child = t.transform.GetChild (i);
			if (child.name == name) {
				return child;
			}
		}

		return null;
	}

	/// <summary>
	///  递归查找子节点
	/// </summary>
	/// <returns>The child with recursive.</returns>
	/// <param name="t">T.</param>
	/// <param name="name">Name.</param>
	public  static Component GetChildWithRecursive(this Component t, string name)
	{
		if (t == null) {
			return null;
		}
		var childCount = t.transform.childCount;
		for (var i = 0; i < childCount; i++) {
			var child = t.transform.GetChild (i);
			if (child.name == name) {
				return child;
			}  else {
				var target = child.GetChildWithRecursive (name);
				if (target != null) {
					return target;
				}
			}
		}
		return null;
	}
	/// <summary>
	/// 查找子节点
	/// </summary>
	/// <returns>The child by name.</returns>
	/// <param name="t">T.</param>
	/// <param name="name">Name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public  static T GetChild<T>(this Component t, string name) where T : Component
	{
		var child = t.GetChild (name);
		if (child == null) {
			return null;
		}
		return child.GetComponent<T>();
	}

	/// <summary>
	/// 递归查找子节点
	/// </summary>
	/// <returns>The child recursive by name.</returns>
	/// <param name="t">T.</param>
	/// <param name="name">Name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public  static T GetChildWithRecursive<T>(this Component t, string name) where T : Component
	{
		var child = t.GetChildWithRecursive (name);
		if (child == null) {
			return null;
		}
		return child.GetComponent<T>();
	}

	/// <summary>
	/// 添加子节点
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="child">Child.</param>
	public static void AddChild(this Component t, Component child)
	{
		if (t == null || child == null) {
			return;
		}

		child.transform.SetParent (t.transform);
	}

	/// <summary>
	/// 移除子节点
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="child">Child.</param>
	/// <param name="destroy">If set to <c>true</c> destroy.</param>
	public static void RemoveChild(this Component t, Component child, bool destroy = true)
	{
		if (t == null || child == null) {
			return;
		}

		var count = t.transform.childCount;
		for (var i = 0; i < count; i++) {
			if (t.transform.GetChild (i) == child) {
				child.transform.SetParent (null);
				if (destroy) {
					Object.Destroy (child.gameObject);
				}
				break;
			}
		}
	}

	/// <summary>
	/// 移除所有子节点
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="destroy">If set to <c>true</c> destroy.</param>
	public static void RemoveAllChildren(this Component t, bool destroy = true)
	{
		if (t == null) {
			return;
		}

		var count = t.transform.childCount;
		for (var i = count - 1; i >=0; i--) {
			var child = t.transform.GetChild (i);
			child.SetParent (null);
			Object.Destroy (child.gameObject);
		}
	}

	/// <summary>
	/// 重父节点移除
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="destory">If set to <c>true</c> destory.</param>
	public static void  RemoveFromParent(this Component t, bool destory = true)
	{
		if (t == null) {
			return;
		}

		t.transform.SetParent (null);
		Object.Destroy (t.gameObject);
	}

	/// <summary>
	/// 添加组件
	/// </summary>
	/// <returns>The component.</returns>
	/// <param name="t">T.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T AddComponent<T>( this Component t) where T : Component
	{
		if (t == null) {
			return null;
		}

		return t.gameObject.AddComponent<T> ();
	}

	/// <summary>
	/// 设置可见性
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="visible">If set to <c>true</c> visible.</param>
	public  static void SetVisible(this Component t, bool visible)
	{
		if (t == null) {
			return;
		}

		t.gameObject.SetActive (visible);
	}

	/// <summary>
	/// 是否可见
	/// </summary>
	/// <returns><c>true</c> if is visible the specified t; otherwise, <c>false</c>.</returns>
	/// <param name="t">T.</param>
	public  static bool IsVisible(this Component t)
	{
		if (t == null) {
			return false;
		}
		return t.gameObject.activeInHierarchy;
	}
}

