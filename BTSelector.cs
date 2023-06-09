﻿using System;

namespace Quantum
{

	/// <summary>
	/// The selector task is similar to an or operation. It will return success as soon as one of its child tasks return success.
	/// If a child task returns failure then it will sequentially run the next task. If no child task returns success then it will return failure.
	/// </summary>
	[Serializable]
	public unsafe partial class BTSelector : BTComposite
	{
		protected override BTStatus OnUpdate(BTParams btParams)
		{
			BTStatus status = BTStatus.Success;

			while (GetCurrentChild(btParams.Frame, btParams.Agent) < _childInstances.Length)
			{
				var currentChildId = GetCurrentChild(btParams.Frame, btParams.Agent);
				var child = _childInstances[currentChildId];
				status = child.RunUpdate(btParams);

				if (status == BTStatus.Abort && btParams.Agent->IsAborting == true)
				{
					return BTStatus.Abort;
				}

				if (status == BTStatus.Failure || status == BTStatus.Abort)
				{
					SetCurrentChild(btParams.Frame, currentChildId + 1, btParams.Agent);
				}
				else
					break;
			}

			return status;
		}

		internal override void ChildCompletedRunning(BTParams btParams, BTStatus childResult)
		{
			if (childResult == BTStatus.Abort)
			{
				return;
			}

			if (childResult == BTStatus.Failure)
			{
				var currentChild = GetCurrentChild(btParams.Frame, btParams.Agent);
				SetCurrentChild(btParams.Frame, currentChild + 1, btParams.Agent);
			}
			else
			{
				SetCurrentChild(btParams.Frame, _childInstances.Length, btParams.Agent);

				// If the child succeeded, then we already know that this sequence succeeded, so we can force it
				SetStatus(btParams.Frame, BTStatus.Success, btParams.Agent);

				// Trigger the debugging callbacks
				BTManager.OnNodeSuccess?.Invoke(btParams.Entity, Guid.Value);
				BTManager.OnNodeExit?.Invoke(btParams.Entity, Guid.Value);
			}
		}
	}
}