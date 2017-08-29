using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class PoolManager<T> : Singleton<PoolManager<T>> where T:MonoBehaviour {

	[SerializeField]
	List<Pool<T>> poolList = new List<Pool<T>>();
	[SerializeField]
	Transform organizeTransform;

	GameObject prefab;

	void Awake(){
		InitializePool();
	}

	void InitializePool(){
		foreach(T objectType in Resources.FindObjectsOfTypeAll<T>()){
			poolList.Add(new Pool<T>(objectType.gameObject,organizeTransform));
		}
	}



	public T RequestInstance(GameObject prefab){

		this.prefab = prefab;
		// Finds the correct pool
		Pool<T> pool = poolList.Find(FindPool);
		// Gets the first avaliable instance
		T instance = pool.pool.Find(T => T.gameObject.activeSelf == false);
		// If none is avaliable, instantiate another five and pick the first one
		if(instance == null){	instance = pool.Instantiate(5);	}
		// takes the Enemy out of the pool
		instance.gameObject.SetActive(true);
		return instance;
	}

	[System.Serializable]
	public class Pool<U>{

		public GameObject prefab;
		public List<U> pool = new List<U>();
		public Transform holderTransform;

		public Pool(GameObject prefab, Transform parent){
			this.prefab = prefab;
			GameObject holder = new GameObject(prefab.name);
			holder.transform.parent = parent;
			holderTransform = holder.transform;
			Instantiate(5);
		}

		public U Instantiate(int quantity){
			U instance = default(U);
			for(int i=0; i<quantity; i++){
				GameObject newInstance = GameObject.Instantiate(prefab);
				newInstance.SetActive(false);
				newInstance.transform.parent = holderTransform;
				pool.Add(newInstance.GetComponent<U>());
				if(instance == null){	instance = pool[pool.Count-1];	}
			}
			return instance;
		}
	}

	private bool FindPool(Pool<T> pool){
		if(pool.prefab == prefab){	return true;	}
		else{	return false;	}
	}
}

