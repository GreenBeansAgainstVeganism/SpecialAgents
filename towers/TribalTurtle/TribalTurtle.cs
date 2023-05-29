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

namespace SpecialAgents.towers.TribalTurtle
{
  class TribalTurtle : ModTower<SpecialAgentSet>
  {
    public override string BaseTower => TowerType.DartMonkey;
    public override int Cost => 720;

    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 1;
    public override int BottomPathUpgrades => 0;
    public override string Description => "Tribal Turtle can live on land or water. Throws spears and coconuts, coconuts do extra damage to ceramic bloons and can pop frozen and lead bloons.";

    public override string Icon => "TribalTurtle-000";

    public override bool Use2DModel => true;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
      towerModel.radius = 7.0f;
      towerModel.IncreaseRange(8.0f);
      towerModel.areaTypes = new AreaType[2] { AreaType.water, AreaType.land };

      var attack = towerModel.GetAttackModel();
      attack.weapons[0].rate = 1.0f;
      attack.weapons[0].projectile.pierce = 3.0f;
      attack.weapons[0].ejectY = 8f;

      var coconutWeapon = attack.weapons[0].Duplicate();
      coconutWeapon.projectile.pierce = 1;
      coconutWeapon.projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
      coconutWeapon.animationOffset = 0.5f;
      coconutWeapon.ejectX += 5f;

      var coconutSmack = Game.instance.model.GetTowerFromId("PatFusty").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.Duplicate();
      coconutSmack.GetDamageModel().damage = 1;
      coconutSmack.pierce = 5f;
      coconutSmack.GetBehavior<DamageModifierForTagModel>().damageAddative = 3;
      coconutWeapon.projectile.AddBehavior(new CreateProjectileOnContactModel("CreateProjectileOnContactModel_", coconutSmack,
        new SingleEmissionModel("SingleEmissionModel_",null),false,false,false));

      attack.weapons[0].projectile.ApplyDisplay<SpearDisplay>();
      coconutWeapon.projectile.ApplyDisplay<CoconutDisplay>();

      attack.AddWeapon(coconutWeapon);
    }
  }

  class TribalTurtlePro : ModUpgrade<TribalTurtle>
  {
    public override int Path => MIDDLE;
    public override int Tier => 1;
    public override int Cost => 675;
    public override string Icon => "TribalTurtle-010";

    public override string Description => "Elite level training allows the Turtle Pro to throw spear and coconut simultaneously for maximum bloon hurt. Coconuts do extra damage to ceramic bloons and can pop frozen and lead bloons.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
      towerModel.GetAttackModel().weapons[0].rate *= 0.5f;
      towerModel.GetAttackModel().weapons[1].rate *= 0.5f;
      towerModel.GetAttackModel().weapons[1].animationOffset = 0f;
    }
  }

  class SpearDisplay : ModDisplay
  {
    public override string BaseDisplay => Generic2dDisplay;
    public override float Scale => 2f;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
      Set2DTexture(node, "spear");
    }
  }
  class CoconutDisplay : ModDisplay
  {
    public override string BaseDisplay => Generic2dDisplay;
    public override float Scale => 2f;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
      Set2DTexture(node, "coconut");
    }
  }
}
