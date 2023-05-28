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
  class Pontoon : ModTower<SpecialAgentSet>
  {
    public override string BaseTower => TowerType.Pontoon;
    public override int Cost => 1250;

    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 1;
    public override int BottomPathUpgrades => 0;
    public override string Description => "Place almost any land tower on water with the Pontoon! Deploy the Pontoon on water, then place your land tower on top.";

    public override string Icon => "Pontoon-000";

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
      towerModel.GetBehavior<AddMakeshiftAreaModel>().points = points;
    }
  }

  class PontoonPro : ModUpgrade<Pontoon>
  {
    public override int Path => MIDDLE;
    public override int Tier => 1;
    public override int Cost => 1000;
    public override string Icon => "Pontoon-010";

    public override string Description => "Primo Pontoon! In addition to luxury fittings, the Primo Pontoon has an uplink to Monkey Town HQ, granting any attached tower a range bonus. Place on water then place your land tower on top.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var rangebuff = Game.instance.model.GetTower(TowerType.MonkeyVillage, 0, 0, 0).GetBehavior<RangeSupportModel>().Duplicate();

      rangebuff.multiplier = 0.2f;
      rangebuff.mutatorId = "PontoonPro-Range";
      rangebuff.ApplyBuffIcon<PontoonProBuffIcon>();

      towerModel.AddBehavior(rangebuff);
    }
  }

  class PontoonProBuffIcon : ModBuffIcon
  {
    public override string Icon => "PontoonPro-Portrait";
  }
}
