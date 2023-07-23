using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Core.Network;
using Nodsoft.WowsReplaysUnpack.Core.Network.Packets;

namespace Nodsoft.WowsReplaysUnpack.Services;

/// <summary>
/// Represents the default implementation for a data parser.
/// </summary>
public class DefaultReplayDataParser : IReplayDataParser
{
	private readonly ILogger<DefaultReplayDataParser> _logger;
	private readonly MemoryStream _packetBuffer = new();
	private readonly BinaryReader _packetBufferReader;

	public DefaultReplayDataParser(ILogger<DefaultReplayDataParser> logger) => (_logger, _packetBufferReader) = (logger, new(_packetBuffer));

	/// <inheritdoc />
	public virtual IEnumerable<NetworkPacketBase> ParseNetworkPackets(MemoryStream replayDataStream, ReplayUnpackerOptions options, Version gameVersion)
	{
		int packetIndex = 0;
		using BinaryReader binaryReader = new(replayDataStream);
		while (replayDataStream.Position != replayDataStream.Length)
		{
			uint packetSize = binaryReader.ReadUInt32();
			uint packetType = binaryReader.ReadUInt32();
			float packetTime = binaryReader.ReadSingle(); // Time in seconds from battle start

			_logger.LogDebug("Packet parsed of type '{PacketType}' with size '{PacketSize}' and timestamp '{PacketTime}'",
				NetworkPacketTypes.GetTypeName(packetType, gameVersion), packetSize, packetTime);

			byte[] packetData = binaryReader.ReadBytes((int)packetSize);

			// Reset packet buffer, write current data and set position to start
			_packetBuffer.SetLength(packetSize);
			_packetBuffer.Seek(0, SeekOrigin.Begin);
			_packetBuffer.Write(packetData);
			_packetBuffer.Seek(0, SeekOrigin.Begin);

			yield return packetType switch
			{
				NetworkPacketTypes.BasePlayerCreate => new BasePlayerCreatePacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.CellPlayerCreate => new CellPlayerCreatePacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityControl => new EntityControlPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityEnter => new EntityEnterPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityLeave => new EntityLeavePacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityCreate => new EntityCreatePacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityProperty => new EntityPropertyPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.EntityMethod => new EntityMethodPacket(packetIndex, packetTime, _packetBufferReader),
				NetworkPacketTypes.Map => new MapPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.NestedProperty => new NestedPropertyPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.Position => new PositionPacket(packetIndex, _packetBufferReader),
				NetworkPacketTypes.PlayerPosition => new PlayerPositionPacket(packetIndex, _packetBufferReader),
				_ => new UnknownPacket(packetIndex, _packetBufferReader)
			};
			packetIndex++;
		}
	}

	/// <summary>
	/// Disposes the data parser.
	/// </summary>
	public void Dispose()
	{
		_packetBufferReader.Dispose();
		GC.SuppressFinalize(this);
	}
}
