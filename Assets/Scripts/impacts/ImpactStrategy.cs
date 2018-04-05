using UnityEngine;

public abstract class ImpactStrategy {
	public abstract void makeImpact(Prefabs prefabs, RaycastHit hit);
}