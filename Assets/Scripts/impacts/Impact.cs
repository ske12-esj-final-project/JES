using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    private ImpactTags impactTags;
    public Dictionary<string, ImpactStrategy> impactStrategies = new Dictionary<string, ImpactStrategy>();

    void Start()
    {
        GameObject global = GameObject.Find("Global");
        impactTags = global.GetComponent<ImpactTags>();
        impactStrategies.Add(impactTags.metalImpactStaticTag, new ImpactMetalStaticStrategy());
        impactStrategies.Add(impactTags.metalImpactTag, new ImpactMetalStrategy());
        impactStrategies.Add(impactTags.woodImpactStaticTag, new ImpactWoodStaticStrategy());
        impactStrategies.Add(impactTags.woodImpactTag, new ImpactWoodStrategy());
        impactStrategies.Add(impactTags.concreteImpactStaticTag, new ImpactConcreteStaticStrategy());
        impactStrategies.Add(impactTags.concreteImpactTag, new ImpactConcreteStrategy());
        impactStrategies.Add(impactTags.dirtImpactStaticTag, new ImpactDirtStaticStrategy());
        impactStrategies.Add(impactTags.dirtImpactTag, new ImpactDirtStrategy());
        impactStrategies.Add(impactTags.enemyImpactStaticTag, new ImpactEnemyStaticStrategy());
        impactStrategies.Add(impactTags.enemyImpactTag, new ImpactEnemyStrategy());
    }
}
