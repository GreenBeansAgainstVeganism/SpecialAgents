using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using MelonLoader;
using Il2CppAssets.Scripts.Unity.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Utils;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.PlacementBehaviors;
using Il2CppAssets.Scripts.Models.Map;

namespace SpecialAgents.towers.PortableLake
{
  class PortableLake : ModTower<SpecialAgentSet>
  {
    public override string BaseTower => TowerType.PortableLake;
    public override int Cost => 1250;

    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 1;
    public override int BottomPathUpgrades => 0;
    public override string Description => "Nowhere to float your boat? Smart monkeys know they can place a Portable Lake on land, allowing any water unit to be deployed within its waters.";

    public override string Icon => "PortableLake-000";

    public override bool Use2DModel => true;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
      towerModel.displayScale *= 1.8f;

      // shift the water region down to match the flat sprite
      var points = towerModel.GetBehavior<AddMakeshiftAreaModel>().points.ToArray();
      for (int i = 0; i < points.Length; i++)
      {
        points[i].y += 6.0f;
      }
    }
  }

  class PortableLakePro : ModUpgrade<PortableLake>
  {
    public override int Path => MIDDLE;
    public override int Tier => 1;
    public override int Cost => 1000;
    public override string Icon => "PortableLake-010";

    public override string Description => "Pro Upgrade - Sea Monster! New Activated Ability temporarily awakens the Sea Monster, which bashes nearby bloons, popping 5 layers and sometimes stunning them. Any water unit can be placed in the Lake.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
      towerModel.range = 30f;

      var attack = Game.instance.model.GetTowerFromId("PatFusty").GetAttackModel().Duplicate();
      attack.range = towerModel.range;

      var smash = attack.weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile;
      smash.pierce = 100f;
      smash.radius = 12f;
      smash.GetBehavior<DisplayModel>().scale = 1.5f;
      smash.GetDamageModel().damage = 5f;
      smash.GetBehavior<DamageModifierForTagModel>().damageAddative = 5f;
      smash.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Moabs", "Moabs", 1f, 10f, true, false));

      var stun = Game.instance.model.GetTower("BombShooter", 5, 0, 0).GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().Duplicate();

      stun.lifespan = 0.4f;
      smash.AddBehavior(stun);
      smash.collisionPasses = new int[2] { -1, 0 };

      var ability = Game.instance.model.GetTower("DartlingGunner", 0, 4, 0).GetAbility().Duplicate();

      ability.GetBehavior<ActivateAttackModel>().attacks[0] = attack;
      ability.GetBehavior<ActivateAttackModel>().lifespan = 5f;
      ability.name = "AbilityModel_Tentacles";
      ability.icon = GetSpriteReference("PortableLakePro-Portrait");
      ability.cooldown = 25f;

      towerModel.AddBehavior(ability);
    }
  }
}
