using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class AILearningManager : MonoBehaviour
{
    public Agent[] agents;
    public GameObject agent;

    public GameObject[] Spowns;
    public GameObject[] Walls;
    private GameObject parent;
    public int SpownIndex;
    private int[] ransu;
    private int ChangeWallIndex;
    private GameObject Wall_created;
    private AIScript[] aiScript;
    // private float pastTime;
    private int agents_Destroyed = 0;
    private BoxCollider boxCollider;
    // Start is called before the first frame update
    void Awake() {
        aiScript = new AIScript[agents.Length];
        for (int i = 0; i < agents.Length; i++)
        {
            aiScript[i] = agents[i].GetComponent<AIScript>();
        }
        DistributeAgents();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false; 

    }
    void Start()
    {
        parent = transform.parent.gameObject; 
        Wall_created = Instantiate(Walls[Random.Range(0,Walls.Length)],parent.transform.position, Quaternion.identity);
        // pastTime = 0;
    }
    // void Update() {
    //     pastTime += 0.0004f;
    // }

       
    public void CreateStagePrefab()
    {
        ChangeWallIndex++;
        if (ChangeWallIndex%10 == 0)
        {
            Destroy(Wall_created);
            ChangeWallIndex = 0;
            Wall_created = Instantiate(Walls[Random.Range(0,Walls.Length)],parent.transform.position, Quaternion.identity);
        }
    }

    public void DistributeAgents()
    {
        ransu = NODupuricationRandom(Spowns.Length,agents.Length);
        for (int i = 0; i < agents.Length; i++)
        {
            Vector3 pos = Spowns[ransu[i]].transform.position;
            pos.y = 0.02f;
            aiScript[i].pos = pos;
        }
    }


    private int[] NODupuricationRandom(int ArrayNum,int count)
    {
        List<int> numbers = new List<int>();

        for (int i = 0; i < ArrayNum; i++) 
        {
            numbers.Add(i);
        }
        int[] ransu = new int[count];
        while (--count >= 0) 
        {
            int index = UnityEngine.Random.Range(0, numbers.Count);
            ransu[count] = numbers[index];
            numbers.RemoveAt(index);
        }
        return ransu;
            
    }

    public void EndEpisode(GameObject agent_Lose,GameObject agent_Win)
    {
        if (agent_Lose  == agent_Win)
        {
            for (int i = 0; i < agents.Length; i++)
            {
                if (agents[i].gameObject == agent_Lose)
                {
                    agents[i].AddReward(-1.2f);
                    agents[i].gameObject.SetActive(false);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < agents.Length; i++)
            {
                if (agents[i].gameObject == agent_Lose)
                {
                    agents[i].AddReward(-0.9f);
                    agents[i].gameObject.SetActive(false);
                }
                if (agents[i].gameObject == agent_Win)
                {
                    agents[i].AddReward(1.1f);
                }
            }
        }
        agents_Destroyed++;
        if (agents_Destroyed+1 == agents.Length)
        {
            boxCollider.enabled = true;
            Invoke("FalseCollider", 1);
            for (int i = 0; i < agents.Length; i++)
            {
                agents[i].gameObject.SetActive(true);
                agents[i].EndEpisode();
            }
            DistributeAgents();
            CreateStagePrefab();
            agents_Destroyed = 0;
        }
        // pastTime = 0;
    }

    void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag =="Gun")
            {
                Destroy(other.gameObject);
            }
        }
    void FalseCollider(){
        boxCollider.enabled = false;
    }
    


}
