using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentPool : MonoBehaviour {


  public static FragmentPool current;

  public GameObject pooledFragment;
  public List<GameObject> pooledFragmentContainer;
  public int maxPoolSize = 500;
  int currentPoolLocation = 0;
  // Use this for initialization

  void Awake()
  {
    current = this;
    pooledFragmentContainer = new List<GameObject>();
  }
 
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void GenerateFragments()
  {
    for (int i = 0; i < maxPoolSize; i++)
    {
      GameObject obj = (GameObject)Instantiate(pooledFragment);
      obj.SetActive(false);
      pooledFragmentContainer.Add(obj);
    }
  }

  public GameObject GetPooledFragments(int fragmentCount)
  {
    for (int i = currentPoolLocation; i < (currentPoolLocation + fragmentCount); i++)
    {
      if (!pooledFragmentContainer[i].activeInHierarchy)
      {
        return pooledFragmentContainer[i];

      }
      currentPoolLocation += 1;
    }
    return null;
  }
 }
