using UnityEngine;
using System.Collections.Generic;

public delegate void MazeLoopDelegate(Maze maze);

public enum PathMark
{
	/// <summary>
	/// 不可行走
	/// </summary>
	NotWalkable,
	/// <summary>
	/// 可行走
	/// </summary>
	Walkable,
}

/// <summary>
/// 迷宫抽象类
/// </summary>
public abstract class Maze 
{
	internal PathMark[,] _map;
	internal Vector2 _originPos;
	internal int _hight;
	internal int _width;

	/// <summary>
	/// 格子数据
	/// 0-不可走，1-可走
	/// 坐标y,x
	/// </summary>
	/// <value>The map.</value>
	public PathMark[,] map 
	{
		get
		{ 
			return _map;
		}
	}

	/// <summary>
	/// 初始位置
	/// </summary>
	/// <value>The origin position.</value>
	public Vector2 originPos 
	{
		get
		{
			return _originPos;
		}
	}

	/// <summary>
	/// 高度
	/// </summary>
	/// <value>The height.</value>
	public int height 
	{
		get
		{
			return _hight;
		}
	}
	/// <summary>
	/// 宽度
	/// </summary>
	/// <value>The width.</value>
	public int width
	{
		get 
		{
			return _width;
		}
	}

	public abstract void GenerateMaze(int width, int height, Vector2 originPos);
}

/// <summary>
/// 方向
/// </summary>
enum BlockDirection {
	None,
	Left,
	Right,
	Up,
	Down,
}

/// <summary>
/// 迷宫格子块
/// </summary>
struct MazeBlock
{
	/// <summary>
	/// 横坐标
	/// </summary>
	public int x;
	/// <summary>
	/// 纵坐标
	/// </summary>
	public int y;
	/// <summary>
	/// 方向
	/// </summary>
	public BlockDirection direction;

	public MazeBlock (int y, int x, BlockDirection direction)
	{
		this.x = x;
		this.y = y;
		this.direction = direction;
	}

	public MazeBlock (int y, int x)
		: this(y, x, BlockDirection.None)
	{
	}
}

/// <summary>
/// 循环监听
/// </summary>
internal static class LoopListener
{
	/// <summary>
	/// 计数
	/// </summary>
	private static int loopCount = 0;
	/// <summary>
	/// 上限值
	/// </summary>
	public static int MaxLoopCount = 50;

	/// <summary>
	/// 增加计数值
	/// </summary>
	public static void Increase()
	{
		loopCount++;
	}

	/// <summary>
	/// 重置
	/// </summary>
	public static void Reset()
	{
		loopCount = 0;
	}
	/// <summary>
	/// 是否溢出
	/// </summary>
	/// <returns><c>true</c> if is over flow; otherwise, <c>false</c>.</returns>
	public static bool IsOverFlow()
	{
		return loopCount >= MaxLoopCount;
	}
}

/// <summary>
/// 随机普利姆算法迷宫
/// </summary>
public class RandomizedPrimMaze : Maze 
{

	/// <summary>
	/// 生成随机普利姆算法迷宫
	/// 地图尺寸必须为奇数，坑！
	///  1. 让迷宫全是墙 
	///  2. 选一个格作为迷宫的通路，然后把它的邻墙放入列表 
	///  3. 当列表里还有墙时： 
	///  ——3.1.从列表里随机选一个墙，如果它对面的格子不是迷宫的通路 : 
	///  ————把墙打通，让对面的格子成为迷宫的通路 
	///  ————把那个格子的邻墙加入列表 
	///  ——3.2.如果对面的格子已经是通路了，那就从列表里移除这面墙
	/// </summary>
	public override void GenerateMaze(int width, int height, Vector2 originPos)
	{
		LoopListener.Reset ();

		// 设置迷宫数据
		_map = new PathMark[height, width];
		_width = width;
		_hight = height;
		_originPos = originPos;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				_map[i, j] = PathMark.NotWalkable;
			}
		}

		if (width % 2 == 0) {
			width -= 1;
		}

		if (height % 2 == 0) {
			height -= 1;
		}

		// 设置起点为目标个
		var x = (int)originPos.x;
		var y = (int)originPos.y;
		_map[y, x] = PathMark.Walkable;

		// 记录邻墙
		var rpBlockPos = new List<MazeBlock>();

		// 如果左方有临墙，将临墙加入列表
		if (x  > 1)
		{
			var block = new MazeBlock(y, x - 1, BlockDirection.Left);
			rpBlockPos.Add(block);
		}

		// 如果下方有临墙，将临墙加入列表
		if (y > 1)
		{
			var block = new MazeBlock(y - 1, x, BlockDirection.Down);
			rpBlockPos.Add(block);
		}

		// 如果右方有临墙，将临墙加入列表
		if (x < width - 1)
		{
			var block = new MazeBlock(y, x + 1, BlockDirection.Right);
			rpBlockPos.Add(block);
		}

		// 如果上方有临墙，将临墙加入列表
		if (y < height - 1)
		{
			var block = new MazeBlock(y + 1, x, BlockDirection.Up);
			rpBlockPos.Add(block);
		}

		while (rpBlockPos.Count > 0) 
		{
			// 随机选一面墙
			int blockIndex = Random.Range(0, rpBlockPos.Count);

			switch (rpBlockPos[blockIndex].direction)
			{
			case BlockDirection.Left:
				x = rpBlockPos[blockIndex].x - 1;
				y = rpBlockPos[blockIndex].y;
				break;
			case BlockDirection.Down:
				x = rpBlockPos[blockIndex].x;
				y = rpBlockPos[blockIndex].y - 1;
				break;
			case BlockDirection.Right:
				x = rpBlockPos[blockIndex].x + 1;
				y = rpBlockPos[blockIndex].y;
				break;
			case BlockDirection.Up:
				x = rpBlockPos[blockIndex].x;
				y = rpBlockPos[blockIndex].y + 1;
				break;
			}

			if (x < 0 || x >= width || y < 0 || y >= height) {
				rpBlockPos.RemoveAt(blockIndex);
				continue;
			}

			//如果目标墙尚未联通
			if (_map[y, x] == 0)
			{
				// 连通目标墙
				_map[rpBlockPos[blockIndex].y, rpBlockPos[blockIndex].x] = PathMark.Walkable;
				_map [y, x] = PathMark.Walkable;

				// 添加目标墙的临墙
				if (x >= 1 && _map[y, x - 1] == PathMark.NotWalkable)
				{
					var block = new MazeBlock(y, x - 1, BlockDirection.Left);
					rpBlockPos.Add(block);
				}

				if (y >= 1 && _map[y - 1, x] == PathMark.NotWalkable)
				{
					var block = new MazeBlock(y - 1, x, BlockDirection.Down);
					rpBlockPos.Add(block);
				}

				if (x < width - 1 && _map[y, x + 1] == PathMark.NotWalkable)
				{
					var block = new MazeBlock(y, x + 1, BlockDirection.Right);
					rpBlockPos.Add(block);
				}

				if (y < height - 1 && _map[y + 1, x] == PathMark.NotWalkable)
				{
					var block = new MazeBlock(y + 1, x, BlockDirection.Up);
					rpBlockPos.Add(block);
				}
			}

			// 移除目标墙
			rpBlockPos.RemoveAt(blockIndex);

			LoopListener.Increase ();
			if (LoopListener.IsOverFlow ()) {
				break;
			}
		}
	}
}

/// <summary>
/// 深度优先算法
/// </summary>
public class RecursiveBacktrackerMaze : Maze 
{
	/// <summary>
	/// 有bug,如果目标不确定，需要设定目标，才行，不然循环遍历下去，所有节点都变成道路了
	/// 深度优先算法的核心是： 
	/// 1. 将起点作为当前格并标记 
	/// 2. 当还存在未标记的格时： 
	/// ——2.1.如果当前格有未标记的邻格： 
	/// ————随机选择一个未标记的邻格 
	/// ————将当前格入栈 
	/// ————移除当前格与邻格的墙 
	/// ————标记邻格并用它作为当前格 
	/// ——2.2.反之，如果栈不空： 
	/// ————栈顶的格子出栈 
	/// ————令其成为当前格 
	/// ——2.3.反之，随机选择一个格子为当前格并标记
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	/// <param name="originPos">Origin position.</param>
	public override void GenerateMaze(int width, int height, Vector2 originPos) 
	{
		LoopListener.Reset ();

		// 设置迷宫数据
		_map = new PathMark[height, width];
		var visited = new bool[height, width];

		_width = width;
		_hight = height;
		_originPos = originPos;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				_map[i, j] = PathMark.NotWalkable;
				// 设置成未被访问
				visited [i, j] = false;
			}
		}
		// 设置起点为目标格
		var x = (int)originPos.x;
		var y = (int)originPos.y;
		_map[y, x] = PathMark.Walkable;
		visited [y, x] = true;

		var rbDirection = new List<BlockDirection>();
		var blockStack = new List<MazeBlock>();
		do
		{
			// 检测周围有没有未连通的格子
			rbDirection.Clear();
			// 检测下方
			if(y > 2 && _map[y - 1, x] == PathMark.NotWalkable && visited[y - 1, x] == false)
			{
				rbDirection.Add(BlockDirection.Down);
			}
			
			// 检测右方
			if(x < width - 2 && _map[y, x + 1] == PathMark.NotWalkable && visited[y, x + 1] == false)
			{
				rbDirection.Add(BlockDirection.Right);
			}
			// 检测上方
			if(y < height - 2 && _map[y + 1, x] == PathMark.NotWalkable && visited[y + 1, x] == false)
			{
				rbDirection.Add(BlockDirection.Up);
			}
			// 检测左方
			if(x > 2 && _map[y, x - 1] == PathMark.NotWalkable && visited[y, x - 1] == false)
			{
				rbDirection.Add(BlockDirection.Left);
			}

			// 选出下一个当前格
			if(rbDirection.Count > 0) 
			{
				int blockIndex = Random.Range(0, rbDirection.Count);
				// 将当前格入栈
				var block = new MazeBlock(y, x);
				blockStack.Add(block);
				// 连通邻格，并将邻格指定为当前格
				switch(rbDirection[blockIndex]) 
				{
				case BlockDirection.Up:
					_map[y + 1, x] = PathMark.Walkable;
					y += 1;
					break;
				case BlockDirection.Down:
					_map[y -1, x] =PathMark.Walkable;
					y -= 1;
					break;
				case BlockDirection.Left:
					_map[y, x - 1] = PathMark.Walkable;
					x -= 1;
					break;
				case BlockDirection.Right:
					_map[y, x + 1] = PathMark.Walkable;
					x += 1;
					break;
				}
				// 标记当前格
				if(y > 1 && y < height - 2 && x > 1 && x < width - 2)
				{
					_map[y, x] = PathMark.Walkable;
					visited[y, x] = true;
				}
			}
			else if(blockStack.Count > 0) 
			{
				// 将栈顶作为当前格，并移除栈顶
				var index = blockStack.Count - 1;
				y = blockStack[index].y;
				x = blockStack[index].x;
				_map[y, x] = PathMark.NotWalkable;
				blockStack.RemoveAt(index);
			}

			LoopListener.Increase ();

			if (LoopListener.IsOverFlow ()) {
				break;
			}
		} while(blockStack.Count > 0);
	}
}

/// <summary>
/// 十字分割法
/// </summary>
public class RecursiveDivisionMaze : Maze
{
	public void  InitRDMaze() 
	{
	}
	/// <summary>
	/// 十字分割法生成的迷宫会形成一个一个大小不一的“房间”，适合制作RPG游戏地图。
	/// 起生成原理及递归法，
	/// 先画一个十字分成四个部分，在三面墙上打洞，再在每个子部分中重复这一步骤，直至空间不够分割（这个值需要我们自行设置）
	/// 
	/// 奇数行开洞，偶数行设墙
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	/// <param name="originPos">Origin position.</param>
	public override void GenerateMaze(int width, int height, Vector2 originPos)
	{
		// 设置迷宫数据
		_map = new PathMark[height, width];
		_width = width;
		_hight = height;
		_originPos = originPos;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (i == 0 || j == 0 || i == height - 1 || j == width - 1) {
					_map[i, j] = PathMark.NotWalkable;
				} else {
					_map[i, j] = PathMark.Walkable;
				}
			}
		}

		LoopListener.Reset ();

		RecursiveRDMaze(1, 1, width - 2, height - 2);
	}

	private void OpenAdoor(int x1, int y1, int x2, int y2) {
		if (x1 > x2 || y1 > y2) {
			return;
		}
		int pos;
		if (x1 == x2) {
			pos = Random.Range(y1, y2 + 1);
			_map[pos, x1] = PathMark.Walkable;
		} else if (y1 == y2) {
			pos = Random.Range(x1, x2 + 1 );
			_map[y1, pos] = PathMark.Walkable;
		} 
	}
	/// <summary>
	/// 迷宫生成算法，采用递归方式实现，随机画横竖两条线，然后在线上随机开三扇门
	/// </summary>
	/// <param name="x">迷宫起点的x坐标.</param>
	/// <param name="y">迷宫起点的y坐标.</param>
	/// <param name="w">迷宫的高度.</param>
	/// <param name="h">迷宫的宽度.</param>
	private void RecursiveRDMaze(int x, int y, int w, int h) {
		if (LoopListener.IsOverFlow ()) {
			return;
		}

		int xPos, yPos;

		if ((x < 0 || x >= width - 1 || x + w >= width)
			|| (y < 0 || y >= height - 1 || y + h >= height)) {
			return;
		}

		if (h <= 2 || w <= 2)
			return;

		//横着画线
		yPos = y + Random.Range (1, h - 1);
		for (int i = x; i < x + w; i++) {
			_map [yPos, i] = PathMark.NotWalkable;
		}

		//竖着画一条线
		xPos = x + Random.Range (1, w - 1) ;
		for (int i = y; i < y + h; i++) {
			_map [i, xPos] = PathMark.NotWalkable;
		}

		//  先开与相邻门的洞

		//如果相邻的是门，就开门，左侧墙壁为1，逆时针旋转 0-左，1-下，2-右，3-上
		if ((x >= 1)  && _map[yPos, x - 1] == PathMark.Walkable) {
			_map [yPos, x] = PathMark.Walkable;
		}

		if ((x + w < width)  && _map[yPos, x + w] == PathMark.Walkable) {
			_map [yPos, x + w - 1] = PathMark.Walkable;
		}

		if ((y >= 1)  && _map[y - 1, xPos] == PathMark.Walkable) {
			_map [y, xPos] = PathMark.Walkable;
		}

		if ((y + h < height)  && _map[y + h, xPos] == PathMark.Walkable) {
			_map [y + h - 1, xPos] = PathMark.Walkable;
			Debug.LogFormat ("Open Door ({0},{1})", y + h - 1, xPos);
		}

		//随机开三扇门，左侧墙壁为1，逆时针旋转 0-左，1-下，2-右，3-上
		int isClosed = Random.Range (0, 4);
		if (isClosed != 0) {
			OpenAdoor (x, yPos, xPos - 1, yPos);// 1
		}
		if (isClosed != 1) {
			OpenAdoor (xPos, y, xPos, yPos - 1);// 2
		}
		if (isClosed != 2) {
			OpenAdoor (xPos + 1, yPos, x + w - 1, yPos);// 3
		}
		if (isClosed != 3) {
			OpenAdoor (xPos, yPos + 1, xPos, y + h - 1);// 4
		}

		LoopListener.Increase ();

		// 左下角
		RecursiveRDMaze (x, y, xPos - x, yPos - y);

		// 右下角
		RecursiveRDMaze (xPos + 1, y, w + x  - (xPos + 1), yPos - y);

		// 右上角
		RecursiveRDMaze (xPos + 1, yPos + 1, w + x  - (xPos + 1), h + y - (yPos + 1));
		// 左上角
		RecursiveRDMaze (x, yPos + 1, xPos - x, h + y - (yPos + 1));
	}
}