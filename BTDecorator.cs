﻿using Photon.Deterministic;

namespace Quantum
{
	public abstract unsafe partial class BTDecorator : BTNode
	{
		[BotSDKHidden] public AssetRefBTNode Child;
		protected BTNode _childInstance;
		public BTAbort AbortType;

		public BTNode ChildInstance
		{
			get
			{
				return _childInstance;
			}
		}

		public override BTNodeType NodeType
		{
			get
			{
				return BTNodeType.Decorator;
			}
		}

		public override void OnReset(BTParams btParams)
		{
			base.OnReset(btParams);

			OnExit(btParams);

			if (_childInstance != null)
				_childInstance.OnReset(btParams);

			BTManager.OnDecoratorReset?.Invoke(btParams.Entity, Guid.Value);
		}

		public override void OnExit(BTParams btParams)
		{
			base.OnExit(btParams);
		}

		protected override BTStatus OnUpdate(BTParams btParams)
		{
			if (DryRun(btParams) == true)
			{
				BTManager.OnDecoratorChecked?.Invoke(btParams.Entity, Guid.Value, true);

				if (_childInstance != null)
				{
					var childResult = _childInstance.RunUpdate(btParams);
					if (childResult == BTStatus.Abort)
					{
						EvaluateAbortNode(btParams);
						SetStatus(btParams.Frame, BTStatus.Abort, btParams.Agent);
						return BTStatus.Abort;
					}

					return childResult;
				}

				return BTStatus.Success;
			}

			BTManager.OnDecoratorChecked?.Invoke(btParams.Entity, Guid.Value, false);

			return BTStatus.Failure;
		}

		public override bool OnDynamicRun(BTParams btParams)
		{
			var result = DryRun(btParams);
			if (result == false)
			{
				return false;
			}
			else if (ChildInstance.NodeType != BTNodeType.Decorator)
			{
				return true;
			}
			else
			{
				return ChildInstance.OnDynamicRun(btParams);
			}
		}

		public override void Loaded(IResourceManager resourceManager, Native.Allocator allocator)
		{
			base.Loaded(resourceManager, allocator);

			// Cache the child
			_childInstance = (BTNode)resourceManager.GetAsset(Child.Id);
			_childInstance.Parent = this;
		}
	}
}