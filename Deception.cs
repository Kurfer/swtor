// Copyright (C) 2011-2017 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
	internal class Deception : RotationBase
	{
		public override string Name
		{
			get { return "Assassin Deception"; }
		}

		public override Composite Buffs
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Mark of Power"),
					Spell.Buff("Stealth", ret => !Rest.KeepResting() && !DefaultCombat.MovementDisabled && !Me.IsMounted)
					);
			}
		}

		public override Composite Cooldowns
		{
			get
			{
				return new PrioritySelector(
					Spell.Buff("Unbreakable Will", ret => Me.IsStunned),
					Spell.Buff("Overcharge Saber", ret => Me.HealthPercent <= 85),
					Spell.Buff("Deflection", ret => Me.HealthPercent <= 60),
					Spell.Buff("Force Shroud", ret => Me.HealthPercent <= 50),
					Spell.Buff("Recklessness", ret => Me.BuffCount("Static Charge") < 1 && Me.InCombat)
					);
			}
		}

		public override Composite SingleTarget
		{
			get
			{
				return new PrioritySelector(
					Spell.Cast("Phantom Stride", ret => !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

					//Movement
					CombatMovement.CloseDistance(Distance.Melee),

					//Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
					Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 3f),
					Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Flamethrower", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
					Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

					//Rotation
					Spell.Cast("Discharge", ret => Me.BuffCount("Static Charge") == 3),
					Spell.Cast("Ball Lightning", ret => Me.BuffCount("Induction") == 2),
					Spell.Cast("Maul", ret => Me.HasBuff("Duplicity") && Me.BuffCount("Static Charge") < 3),
					Spell.Cast("Assassinate", ret => Me.CurrentTarget.HealthPercent <= 30),
					Spell.Cast("Reaping Strike", ret => Me.BuffCount("Static Charge") < 3),
					Spell.Cast("Voltaic Slash"),
					Spell.Cast("Saber Strike", ret => Me.ForcePercent <= 25)
					);
			}
		}

		public override Composite AreaOfEffect
		{
			get
			{
				return new PrioritySelector(
					new Decorator(ret => Targeting.ShouldAoe,
						new PrioritySelector(
							Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 4f))), //--will only be active when user initiates Heroic Moment--
						new Decorator(ret => Targeting.ShouldPbaoe,
						Spell.Cast("Lacerate", ret => Me.ForcePercent >= 60))
					);
			}
		}
	}
}