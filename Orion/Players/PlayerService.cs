﻿using System;
using System.Collections.Generic;
using System.Linq;
using Orion.Framework;
using Orion.Players.Events;
using OTAPI;

namespace Orion.Players
{
	/// <summary>
	/// Manages players.
	/// </summary>
	[Service("Player Service", Author = "Nyx Studios")]
	public class PlayerService : SharedService, IPlayerService
	{
		private readonly IPlayer[] _players;

		/// <inheritdoc/>
		public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;

		/// <inheritdoc/>
		public event EventHandler<PlayerJoiningEventArgs> PlayerJoining;

		/// <inheritdoc/>
		public event EventHandler<PlayerQuitEventArgs> PlayerQuit;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlayerService"/> class.
		/// </summary>
		/// <param name="orion">The parent <see cref="Orion"/> instance.</param>
		public PlayerService(Orion orion) : base(orion)
		{
			_players = new IPlayer[Terraria.Main.player.Length];
			Hooks.Net.RemoteClient.PreReset = InvokePlayerQuit;
			Hooks.Player.PreGreet = InvokePlayerJoin;
			// TODO: change this to use net hooks, so we can have separate greeting hooks
		}

		/// <inheritdoc/>
		/// <remarks>
		/// The players are cached in an array. Calling this method multiple times will result in the same instances as
		/// long as Terraria's player array remains unchanged.
		/// </remarks>
		public IEnumerable<IPlayer> FindPlayers(Predicate<IPlayer> predicate = null)
		{
			var players = new List<IPlayer>();
			for (var i = 0; i < _players.Length; i++)
			{
				if (_players[i]?.WrappedPlayer != Terraria.Main.player[i])
				{
					_players[i] = new Player(Terraria.Main.player[i]);
				}
				players.Add(_players[i]);
			}
			return players.Where(p => p.WrappedPlayer.active && (predicate?.Invoke(p) ?? true));
		}

		private HookResult InvokePlayerJoin(ref int playerId)
		{
			if (_players[playerId]?.WrappedPlayer != Terraria.Main.player[playerId])
			{
				_players[playerId] = new Player(Terraria.Main.player[playerId]);
			}
			IPlayer player = _players[playerId];
			var preArgs = new PlayerJoiningEventArgs(player);
			PlayerJoining?.Invoke(this, preArgs);
			if (preArgs.Handled)
			{
				return HookResult.Cancel;
			}

			var postArgs = new PlayerJoinedEventArgs(player);
			PlayerJoined?.Invoke(this, postArgs);
			return HookResult.Continue;
		}

		private HookResult InvokePlayerQuit(Terraria.RemoteClient remoteClient)
		{
			if (remoteClient.Socket == null)
			{
				return HookResult.Continue;
			}

			if (_players[remoteClient.Id]?.WrappedPlayer != Terraria.Main.player[remoteClient.Id])
			{
				_players[remoteClient.Id] = new Player(Terraria.Main.player[remoteClient.Id]);
			}
			IPlayer player = _players[remoteClient.Id];
			var args = new PlayerQuitEventArgs(player);
			PlayerQuit?.Invoke(this, args);
			return HookResult.Continue;
		}
	}
}
