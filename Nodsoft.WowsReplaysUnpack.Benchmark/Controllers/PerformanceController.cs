﻿using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Controllers;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Benchmark.Controllers
{
	internal class PerformanceController : AReplayControllerBase<PerformanceController>
	{
		public PerformanceController(IDefinitionStore definitionStore, ILogger<Entity> entityLogger) : base(definitionStore, entityLogger)
		{
		}
		public override void HandleNetworkPacket(ANetworkPacket networkPacket, ReplayUnpackerOptions options)
		{
			if (networkPacket is EntityMethodPacket em)
				OnEntityMethod(em);
		}
	}
}
