using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class AIScript : Agent
{
    public AILearningManager AILM;
    [SerializeField] Player_ScriptableObject player_sb;

    private Vector3 Player_pos; //プレイヤーのポジション
    private float x = 0; //x方向のImputの値
    private float z = 0; //z方向のInputの値
    private Rigidbody rb;
    private Transform SpownObj;
    public AIGunScript gunScript;
    public int availableGunNum = 3;
    public Vector3 pos;
    private float SPEED = 0.02f * 3f;

    private float emitInterval = -0.2f; // availableGunNumに掛け算することで条件分岐を減らした　玉の発射間隔

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>(); //プレイヤーのRigidbodyを取得
        SpownObj = transform.GetChild(0);
        availableGunNum = player_sb.GunNum;
    }


    public override void OnEpisodeBegin()
    {
        // pos.y = 0.02f;
        transform.position = pos;
        availableGunNum = player_sb.GunNum;
        AILM.SpownIndex +=1;
        emitInterval = -2.0f;
    }


    public override void OnActionReceived(float[] vectorAction)
    {
        var h = vectorAction[0];
        var v = vectorAction[1];
        x = KeySteer(x,h);
        z = KeySteer(z,v);
        var firing = vectorAction[2];

        
        
        var InputVec = new Vector3(x, 0, z);
        if ((x*x+z*z)>1.0f)
        {
            InputVec = InputVec.normalized; 
        }
        InputVec *= player_sb.speed * 0.1f;
        
        rb.velocity = InputVec; //プレイヤーのRigidbodyに対してInputにspeedを掛けた値で更新し移動

        Vector3 diff = transform.position - Player_pos; //プレイヤーがどの方向に進んでいるかがわかるように、初期位置と現在地の座標差分を取得

        if (diff.sqrMagnitude > 0.0001f) //ベクトルの長さが0.01fより大きい場合にプレイヤーの向きを変える処理を入れる(0では入れないので）
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(diff),0.5f);  //ベクトルの情報をQuaternion.LookRotationに引き渡し回転量を取得しプレイヤーを回転させる
        }



        Player_pos = transform.position; //プレイヤーの位置を更新

        emitInterval += Time.deltaTime;
        if (firing == 1 && availableGunNum * emitInterval >0 )
        {
            CreateGun();
            --availableGunNum;
            emitInterval = -0.2f;
        }
    }
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0]=0;
        actionsOut[1]=0;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            actionsOut[0] = 1f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            actionsOut[0] = 2f;
        }
        //rotate
        if (Input.GetKey(KeyCode.DownArrow))
        {
            actionsOut[1] = 1f;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            actionsOut[1] = 2f;
        }
        actionsOut[2] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
    }

    private void CreateGun()
    {
        GameObject Gun = Instantiate(player_sb.GunPrefab,SpownObj.transform.position, SpownObj.transform.rotation) as GameObject;
        gunScript = Gun.GetComponent<AIGunScript>();
        gunScript.speed = player_sb.GunSpeed * 0.1f;
        gunScript.collisionNum = player_sb.GunBounceNum;
        gunScript.playerScript = this;
        gunScript.gameObj = this.gameObject;
    }

    

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Gun")
        {
            AIGunScript gunScript = other.gameObject.GetComponent<AIGunScript>();
            gunScript.HitEnemy();
            AILM.EndEpisode(this.gameObject,gunScript.gameObj);
            Destroy(other.gameObject);
        }
        
    }

    public void OnCollisionStay(Collision other) {
        if (other.gameObject.tag == "Wall")
        {
            AddReward(-0.0005f);
        }
    }

    float KeySteer(float keySteer,float i)
	{
		if (i == 2)
		{
			keySteer += SPEED;
		}
		if (i == 1)
		{
			keySteer -= SPEED;
		}
		if (i == 0)
		{
			if (0 < keySteer)
			{
				keySteer -= SPEED;
			}
			else if (keySteer < 0)
			{
				keySteer += SPEED;
			}
		}
		if (-SPEED < keySteer && keySteer < SPEED)
			keySteer = 0;

		if (keySteer < -1)
		{
			keySteer = -1;
		}
		else if (1 < keySteer)
		{
			keySteer = 1;
		}

		return keySteer;
	}

    
}

