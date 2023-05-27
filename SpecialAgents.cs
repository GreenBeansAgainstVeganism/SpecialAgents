using MelonLoader;
using BTD_Mod_Helper;
using SpecialAgents;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Models.TowerSets;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(SpecialAgents.SpecialAgents), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace SpecialAgents
{
  public class SpecialAgents : BloonsTD6Mod
  {
      public override void OnApplicationStart()
      {
          ModHelper.Msg<SpecialAgents>("SpecialAgents loaded!");
      }
  }

  public class SpecialAgentSet : ModTowerSet
  {
    public override string DisplayName => "Special Agents";

    public override bool AllowInRestrictedModes => true; 

    public override int GetTowerSetIndex(List<TowerSet> towerSets) => towerSets.IndexOf(TowerSet.Support) + 1;
  }
}
