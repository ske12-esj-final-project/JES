using UnityEngine;

public class ImpactDirtStaticStrategy : ImpactStrategy {

	public override void makeImpact(Prefabs prefabs, RaycastHit hit) {
		Object.Instantiate (prefabs.dirtImpactStaticPrefab, hit.point, 
			Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
	}
}