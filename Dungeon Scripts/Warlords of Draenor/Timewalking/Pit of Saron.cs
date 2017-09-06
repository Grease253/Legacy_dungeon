using System;using System.Collections.Generic;
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


namespace Bots.DungeonBuddy.Dungeon_Scripts.Wrath_of_the_Lich_King
{
	public class PitOfSaron : Dungeon
	{
		#region Overrides of Dungeon
		private LocalPlayer Me
		{
			get { return StyxWoW.Me; }
		}
		public override uint DungeonId
		{
			get { return 1153; }
		}
		
		public override bool IsFlyingCorpseRun
		{
			get { return true; }
		}
		
		public override WoWPoint Entrance
		{
			get { return new WoWPoint(5580.26f, 2003.42f, 798.18f); }
		}
		
		private readonly CircularQueue<WoWPoint> _corpseRun = new CircularQueue<WoWPoint>
															  {
																  new WoWPoint(6381.55, 2090.36, 618.97),
																  new WoWPoint(6384.21, 2111.36, 600.97),
																  new WoWPoint(6094.48, 2236.71, 669.97),
																  new WoWPoint(5693.51, 2247.83, 816.59),
																  new WoWPoint(5685.73, 2099.05, 798.06),
																  new WoWPoint(5640.28, 2098.30, 798.06),																   
																  new WoWPoint(5585.26f, 2005.42f, 798.18f)
															  };
															  
		public override CircularQueue<WoWPoint> CorpseRunBreadCrumb
		{
			get { return _corpseRun; }
		}
		
		public override WoWPoint ExitLocation
		{
			get { return new WoWPoint(425.54, 212.15, 529.91); }
		}


public override async Task<bool> HandleMovement(WoWPoint location)
 {
Styx.CommonBot.Profiles.ProfileManager.CurrentProfile.UseMount = false;
if (Me.Mounted) {await CommonCoroutines.Dismount();}
   return false;
 }	
 

[EncounterHandler(36494, "Schmiedemeister Garfrost", Mode = CallBehaviorMode.Proximity, BossRange = 50)]
public Func<WoWUnit, Task<bool>> KZ1()
{
			var isTank = Me.IsTank();
if(!isTank) {
AddAvoidObject(25, o => o.Entry == 36494 && o.ToUnit().Combat, o => o.Location.RayCast(o.Rotation, 20)); //don't stand in front
AddAvoidObject(ctx => true, 10, 68786); 
}

return async boss =>		 { return false;					 };		
}


[EncounterHandler(36477, "Krick", Mode = CallBehaviorMode.Proximity, BossRange = 50)]
public Func<WoWUnit, Task<bool>> KZ2()
{
AddAvoidObject(ctx => true, 10, 69024); 
return async boss =>		 { return false;					 };		
}

[EncounterHandler(36658, "scourgelord-tyrannus", Mode = CallBehaviorMode.Proximity, BossRange = 50)]
public Func<WoWUnit, Task<bool>> KZ3()
{
AddAvoidObject(ctx => true, 10, 69024); 
return async boss =>		 { return false;					 };		
}




	


#endregion
	}
	
}