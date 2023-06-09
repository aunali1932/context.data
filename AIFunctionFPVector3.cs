﻿using Photon.Deterministic;

namespace Quantum
{
	public unsafe abstract partial class AIFunctionFPVector3
	{
		public abstract FPVector3 Execute(Frame frame, EntityRef entity);
	}

	[BotSDKHidden]
	[System.Serializable]
	public unsafe partial class DefaultAIFunctionFPVector3 : AIFunctionFPVector3
	{
		public override FPVector3 Execute(Frame frame, EntityRef entity)
		{
			return FPVector3.Zero;
		}
	}
}
