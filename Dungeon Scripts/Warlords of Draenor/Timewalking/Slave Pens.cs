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

namespace Bots.DungeonBuddy.Dungeon_Scripts.Burning_Crusade
{
	public class SlavePens : CoilfangDungeon
	{
		#region Overrides of Dungeon


		/// <summary>
		///     The mapid of this dungeon.
		/// </summary>
		/// <value> The map identifier. </value>
		public override uint DungeonId
		{
			get { return 1015; }
		}

		private readonly WoWPoint _entrance = new WoWPoint(740.8317, 7013.667, -72.97391);
		public override WoWPoint Entrance
		{
			get { return _entrance; }
		}

		public override WoWPoint ExitLocation
		{
			get { return new WoWPoint(118.778, -139.3521, -0.1595907);  }
		}

		#endregion
		
		const uint WastewalkerWorkerId = 17964;
		const uint WastewalkerSlaveId = 17963;
		private const uint MennuTheBetrayer = 17941;
		private const uint CoilfangSoothsayerId = 17960;
		private const uint CoilfangScaleHealerId = 21126;
		private const uint CoilfangRayId = 21128;
		private const uint CoilfangChampionId = 17957;
		private const uint MennuHealingWard = 20208;
		private const uint TaintedStoneskinTotem = 18177;

		private readonly WoWPoint _quagmirranFollowerSpot = new WoWPoint(-166.5996, -729.2723, 37.89237);
		private readonly WoWPoint _quagmirranTankSpot = new WoWPoint(-198.9592, -708.0936, 37.89237);
		private WoWUnit _quagmirran;

		/// <summary>
		///     Dungeon specific unit target removal.
		/// </summary>
		/// <param name="units"> The incomingunits. </param>
		public override void RemoveTargetsFilter(List<WoWObject> units)
		{
			units.RemoveAll(
				u =>
				{
					var unit = u as WoWUnit;
					if (unit != null)
					{
						if (unit is WoWPlayer)
							return true;
						if ( (unit.Entry == WastewalkerSlaveId || unit.Entry == WastewalkerWorkerId) &&  (!unit.Combat || !unit.GotTarget))
							return false;
					}
					return false;
				});
		}

		// for the mennu bossfight.

		public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
		{
			foreach (WoWObject obj in incomingunits)
			{
				if (obj.Entry == MennuHealingWard || obj.Entry == TaintedStoneskinTotem)
				{
					outgoingunits.Add(obj);
				}
			}
		}

		/// <summary>
		///     Dungeon specific unit weighting.
		/// </summary>
		/// <param name="units"> The units. </param>
		public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
		{
			/*
			Information about Tainted Stoneskin Totem
			Name = Tainted Stoneskin Totem
			Wowhead Id = 18177
			Faction = 74 [Naga]
			
			Information about Mennu's Healing Ward
			Name = Mennu's Healing Ward
			Wowhead Id = 20208
			Faction = 74 [Naga]
			
			#1 Boss
			Information about Mennu the Betrayer
			Name = Mennu the Betrayer
			Wowhead Id = 17941
			Faction = 74 [Naga]
			
			#2 Boss
			Information about Rokmar the Crackler
			Name = Rokmar the Crackler
			Wowhead Id = 17991
			Faction = 16 [Monster]
			*/

			foreach (Targeting.TargetPriority p in units)
			{
				var unit = p.Object as WoWUnit;
				if (unit == null) continue;

				if (unit.Entry == MennuHealingWard)
					p.Score += 4000;
				else if (unit.Entry == TaintedStoneskinTotem)
					p.Score += 1000;
				else if (unit.Entry == CoilfangChampionId && Me.IsDps())
					p.Score += 1000;
				else if (unit.Entry == CoilfangSoothsayerId && Me.IsDps())
					p.Score += 850;
				else if (unit.Entry == CoilfangScaleHealerId && Me.IsDps())
					p.Score += 800;
				else if (unit.Entry == CoilfangRayId && Me.IsDps())
					p.Score += 900;
			}
		}

		[EncounterHandler(54668, "Nahuud", Mode = CallBehaviorMode.Proximity, BossRange = 30)]
		public Composite QuestPickupHandler()
		{
			WoWUnit unit = null;
			return new PrioritySelector(
				ctx => unit = ctx as WoWUnit,
				new Decorator(
					ctx => !Me.Combat && !ScriptHelpers.WillPullAggroAtLocation(unit.Location) && unit.QuestGiverStatus == QuestGiverStatus.Available,
					ScriptHelpers.CreatePickupQuest(ctx => unit)),
				new Decorator(
					ctx => !Me.Combat && !ScriptHelpers.WillPullAggroAtLocation(unit.Location) && unit.QuestGiverStatus == QuestGiverStatus.TurnIn,
					ScriptHelpers.CreateTurninQuest(ctx => unit)));
		}

		[EncounterHandler(54667, "Watcher Jhang", Mode = CallBehaviorMode.Proximity, BossRange = 30)]
		public Composite WatcherJhangQuestHandler()
		{
			const int theHeartOfTheMatterQuestId = 29565;

			WoWUnit unit = null;
			return new PrioritySelector(
				ctx => unit = ctx as WoWUnit,
				new Decorator(
					ctx =>
					!Me.Combat && !ScriptHelpers.WillPullAggroAtLocation(unit.Location) && unit.QuestGiverStatus == QuestGiverStatus.Available &&
					Me.QuestLog.GetQuestById(theHeartOfTheMatterQuestId) == null && Me.QuestLog.GetCompletedQuests().All(q => q != theHeartOfTheMatterQuestId),
					ScriptHelpers.CreatePickupQuest(ctx => unit, theHeartOfTheMatterQuestId)),
				new Decorator(
					ctx => !Me.Combat && !ScriptHelpers.WillPullAggroAtLocation(unit.Location) && unit.QuestGiverStatus == QuestGiverStatus.TurnIn,
					ScriptHelpers.CreateTurninQuest(ctx => unit)));
		}

		[EncounterHandler(17942, "Quagmirran", BossRange = 100, Mode = CallBehaviorMode.Proximity)]
		public Composite QuagmirranEncounter()
		{
			return new PrioritySelector(
				ctx => _quagmirran = ctx as WoWUnit,
				new Decorator(
					ctx => StyxWoW.Me.IsTank(),
					new PrioritySelector(
						new Decorator(
							ctx => _quagmirran.IsCasting,
							new PrioritySelector(
				// taunt boss when he does the acid spray thing to make him face tank.
								new Decorator(ctx => StyxWoW.Me.Class == WoWClass.Warrior && SpellManager.CanCast("Taunt"), new Action(ctx => SpellManager.Cast("Taunt"))),
								new Decorator(
									ctx => StyxWoW.Me.Class == WoWClass.DeathKnight && SpellManager.CanCast("Dark Command"),
									new Action(ctx => SpellManager.Cast("Dark Command"))),
								new Decorator(ctx => StyxWoW.Me.Class == WoWClass.Druid && SpellManager.CanCast("Growl"), new Action(ctx => SpellManager.Cast("Growl"))),
								new Decorator(
									ctx => StyxWoW.Me.Class == WoWClass.Paladin && SpellManager.CanCast("Hand of Reckoning"),
									new Action(ctx => SpellManager.Cast("Hand of Reckoning"))),
								new Decorator(ctx => StyxWoW.Me.Class == WoWClass.Monk && SpellManager.CanCast("Provoke"), new Action(ctx => SpellManager.Cast("Provoke"))))),
						new Decorator(ctx => Targeting.Instance.FirstUnit == null, new Action(ctx => ScriptHelpers.SetLeaderMoveToPoiPS(_quagmirran.Location))),
						ScriptHelpers.CreateTankUnitAtLocation(ctx => _quagmirranTankSpot, 17f),
						ScriptHelpers.CreateAvoidUnitAnglesBehavior(
							ctx => !Me.IsTank() && _quagmirran.CurrentTargetGuid != Me.Guid && _quagmirran.IsCasting && !_quagmirran.IsMoving,
							ctx => _quagmirran,
							new ScriptHelpers.AngleSpan(0, 180)),
						ScriptHelpers.CreateTankFaceAwayGroupUnit(40))),
				new Decorator(
					ctx => StyxWoW.Me.IsFollower(),
					new PrioritySelector(
						new Decorator(
							ctx =>
							Targeting.Instance.IsEmpty() && ScriptHelpers.Tank.Location.DistanceSqr(_quagmirranTankSpot) > 17 * 17 &&
							StyxWoW.Me.Location.DistanceSqr(_quagmirranFollowerSpot) > 5 * 5,
							new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(_quagmirranFollowerSpot)))),
						ScriptHelpers.CreateSpreadOutLogic(ctx => true, 20))));
		}
	}

	public abstract class CoilfangDungeon : Dungeon
	{
		protected LocalPlayer Me
		{
			get { return StyxWoW.Me; }
		}

		#region corpse run behavior

		private readonly CircularQueue<WoWPoint> _tunnelPath = new CircularQueue<WoWPoint>()
													{
														new WoWPoint(563.1432f, 6943.247f, -1.951628f),
														new WoWPoint(568.424f, 6941.722f, -24.89723f),
														new WoWPoint(582.709f, 6936.663f, -38.11215f),
														new WoWPoint(602.8282f, 6915.195f, -45.36474f),
														new WoWPoint(611.301f, 6895.487f, -51.30582f),
														new WoWPoint(637.5763f, 6869.115f, -79.11536f),
														new WoWPoint(667.0811f, 6865.27f, -81.14445f),
														new WoWPoint(667.0811f, 6865.27f, -70.73276f),
													};

		readonly WoWPoint _corpseRunSubmergeStart = new WoWPoint(561.1754f, 6944.772f, 16.60149f);

		public override async Task<bool> HandleMovement(WoWPoint location)
		{
			// Coilfang corpse run. 
			if (Me.IsGhost)
			{
				var myLoc = Me.Location;
				// Outside and above water logic
				if (Me.Z > 12)
				{
					// move to a point on the outside water surface just above underwater tunnel 
				    if (myLoc.DistanceSqr(_corpseRunSubmergeStart) > 10*10)
				        return (await CommonCoroutines.MoveTo(_corpseRunSubmergeStart)).IsSuccessful();

					if (_tunnelPath.Peek() != _tunnelPath.First)
						_tunnelPath.CycleTo(_tunnelPath.First);

					// submerge. We can only break through the water's surface if player's vertical pitch is facing down. 
					if (!Me.IsSwimming)
						Lua.DoString("VehicleAimIncrement(-1)");
					else
						Navigator.PlayerMover.MoveTowards(_tunnelPath.First);
				    return true;
				}
				// tunnel navigation.
				if (Me.IsSwimming)
				{
					var moveTo = _tunnelPath.Peek();
					if (myLoc.DistanceSqr(moveTo) < 5 * 5)
					{
						_tunnelPath.Dequeue();
						moveTo = _tunnelPath.Peek();
						// Tunnel path ends at the water surface of the underwater pool. Jump to walk on the surface.
						if (moveTo == _tunnelPath.First)
						{
							WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend);
							WoWMovement.MoveStop(WoWMovement.MovementDirection.JumpAscend);
							return true;
						}
					}
					Navigator.PlayerMover.MoveTowards(moveTo);
					return true;
				}

			}
		    return false;
		}

		public override void OnEnter()
		{
			// if coming back from a DC and a ghost then make sure current point in _tunnelPath is closest one.
			if (Me.IsGhost && Me.IsSwimming)
			{
				var myLoc = Me.Location;
				_tunnelPath.CycleTo(_tunnelPath.OrderBy(loc => loc.DistanceSqr(myLoc)).FirstOrDefault());
			}
			base.OnEnter();
		}

		#endregion

	}

	
}