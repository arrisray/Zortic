using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TSDC;

public class ZorticAssetUtility : MonoBehaviour 
{
	[MenuItem( "Zortic/Create Game Rule" )]
	public static void CreateGameRule()
	{
		GameRule gameRule = GameRuleEditor.CreateRule();
		NullCondition defaultCondition = GameRuleEditor.CreateCondition<NullCondition>();
		gameRule.condition = defaultCondition;
	}

	[MenuItem( "Zortic/Create Tag" )]
	public static void CreateTag()
	{
		CustomAssetUtility.CreateAsset<EntityTag>();
	}
}
