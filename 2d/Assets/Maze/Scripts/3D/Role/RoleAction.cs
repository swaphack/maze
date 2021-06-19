using UnityEngine;
using System.Collections;

public enum ActionType
{
	Take,
	Attack1,
	Attack2,
	Attack3,
	Attack4,
	Death,
	Flail,
	Healward,
	Idle1,
	Idle2,
	Spain,
	Stun,
	Walk
}

public class RoleAction
{
	public static string[] ActionName =  {
		"Take 001",
		"attack1",
		"attack2",
		"attack3",
		"attack4",
		"death",
		"flail",
		"healward",
		"idle1",
		"idle2",
		"spain",
		"stun",
		"walk",
	};

	public static string GetActionName(ActionType actionType)
	{
		return ActionName [(int)actionType];
	}
}

