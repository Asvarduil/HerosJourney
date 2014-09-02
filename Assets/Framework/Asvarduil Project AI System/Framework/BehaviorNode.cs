using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implements a Behavior Tree Node, which can cause
/// a complex behavioral response based on series
/// of conditions.
/// 
/// A Behavior Tree is what we could consider a Hierarchical 
/// Finite State Machine (HFSM, for short).  At one level, 
/// a condition is evaluated.  If the condition for a node holds, 
/// either A) any children will then have their conditions 
/// evaluated, and/or B) their immediate behavior
/// will occur if there are no child nodes.  
/// 
/// It's possible for one node to trigger a number of behaviors 
/// in succession in this system, and to create very complex behaviors.
/// </summary>
public class BehaviorNode
{
	#region Variables / Properties
	
	public List<BehaviorNode> Children;
	public Func<bool> Condition;
	public Action Behavior = null;
	
	#endregion Variables / Properties
	
	#region Constructor
	
	public BehaviorNode(List<BehaviorNode> children, Func<bool> condition, Action behavior)
	{
		Children = children;
		Condition = condition;
		Behavior = behavior;
	}
	
	#endregion Constructor
	
	#region Methods
	
	public IEnumerator EvaluateNode()
	{
		// If the condition for behavior does not hold up,
		// stop here.
		if(! Condition())
			yield break;
		
		// If a behavior has been specified, execute that before
		// performing any other behaviors.
		if(Behavior != null)
			Behavior();
		
		for(int i = 0; i < Children.Count; i++)
		{
			BehaviorNode node = Children[i];
			yield return node.EvaluateNode();
		}
	}
	
	#endregion Methods
}
