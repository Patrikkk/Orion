﻿using System;
using NUnit.Framework;
using Orion.Projectiles.Events;

namespace Orion.Tests.Projectiles.Events
{
	[TestFixture]
	public class ProjectileUpdatedAIEventArgsTests
	{
		[Test]
		public void Constructor_NullProjectile_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ProjectileUpdatedAIEventArgs(null));
		}
	}
}
