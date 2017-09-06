using System;
using System.Collections.Generic;
using System.Linq;
using CommonBehaviors.Actions;
using Styx;
using Styx.CommonBot.Routines;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.TreeSharp;
using Bots.DungeonBuddy.Attributes;
using Bots.DungeonBuddy.Helpers;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Styx.CommonBot.Coroutines;
using Styx.CommonBot.POI;

using Action = Styx.TreeSharp.Action;

namespace Bots.DungeonBuddy.Dungeon_Scripts.Cataclysm
{
	public class RandomTimewalkingDungeonCATA : Dungeon
	{
		#region Overrides of Dungeon


		/// <summary>
		///     The mapid of this dungeon.
		/// </summary>
		/// <value> The map identifier. </value>
		public override uint DungeonId
		{
			get { return 1146; }
		}

		#endregion
		
	}

	
}