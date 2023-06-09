﻿using Photon.Deterministic;

namespace Quantum
{
	public unsafe abstract partial class AIFunctionFPVector2
	{
		public abstract FPVector2 Execute(Frame frame, EntityRef entity);
	}

	[BotSDKHidden]
	[System.Serializable]
	public unsafe partial class DefaultAIFunctionFPVector2 : AIFunctionFPVector2
	{
		public override FPVector2 Execute(Frame frame, EntityRef entity)
		{
			return FPVector2.Zero;
		}
	}
}
