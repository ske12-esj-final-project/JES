using UnityEngine;

public class ImpactExplosiveBarrelStrategy : ImpactStrategy {

	public override void makeImpact(Prefabs prefabs, RaycastHit hit) {
		//Toggle the explode bool on the explosive barrel object
		hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
		//Spawn metal impact on surface of the barrel
		Object.Instantiate (prefabs.metalImpactPrefab, hit.point, 
			Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
	}
}