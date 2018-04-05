using UnityEngine;

public class ImpactGasTankStrategy : ImpactStrategy {

	public override void makeImpact(Prefabs prefabs, RaycastHit hit) {
		//Toggle the explode bool on the explosive barrel object
		hit.transform.gameObject.GetComponent<GasTankScript> ().isHit = true;
		//Spawn metal impact on surface of the gas tank
		Object.Instantiate (prefabs.metalImpactPrefab, hit.point, 
			Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
	}
}