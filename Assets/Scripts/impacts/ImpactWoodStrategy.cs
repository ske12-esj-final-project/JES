using UnityEngine;

public class ImpactWoodStrategy : ImpactStrategy {

	public override void makeImpact(Prefabs prefabs, RaycastHit hit) {
		Object.Instantiate (prefabs.woodImpactPrefab, hit.point, 
			Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
	}
}