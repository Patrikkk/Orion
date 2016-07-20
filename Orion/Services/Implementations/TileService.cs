﻿using Orion.Framework;
using Orion.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OTAPI.Core;
using OTAPI.Tile;
using Terraria;

namespace Orion.Services.Implementations
{
	/// <summary>
	/// Implements the functionality in <see cref="TileService"/>. Mimics vanilla behaviour by storing tiles in a 2D tile
	/// array.
	/// </summary>
	[Service(Author = "Nyx Studios", Name = "Tile Service")]
	public class TileService : ServiceBase, ITileService
	{
		protected ITile[,] tileBuffer;

		/// <summary>
		/// Initializes a new instance of the <see cref="TileService"/> class.
		/// </summary>
		/// <param name="orion">The <see cref="Orion"/> instance.</param>
		public TileService(Orion orion) : base(orion)
		{
			Hooks.Tile.CreateCollection = () => this;
		}

		public ITile this[int x, int y]
		{
			get
			{
				if (tileBuffer == null)
				{
					tileBuffer = new ITile[Main.maxTilesX + 1, Main.maxTilesY + 1];
				}

				return tileBuffer[x, y];
			}
			set
			{
				tileBuffer[x, y] = value;
			}
		}
	}
}