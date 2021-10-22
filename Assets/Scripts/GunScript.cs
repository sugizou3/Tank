using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float speed; //プレイヤーの動くスピード
    public int collisionNum = 2;
    public PlayerScript playerScript;
    private Rigidbody rb;
    private Vector3 Player_pos; //プレイヤーのポジション
    void Start()
    {
        Player_pos = transform.position; //最初の時点での玉のポジションを取得
        rb = GetComponent<Rigidbody>(); //プレイヤーのRigidbodyを取得
        rb.velocity = transform.forward * speed;

    }

    void FixedUpdate()
    {

        Vector3 diff = transform.position - Player_pos; //プレイヤーがどの方向に進んでいるかがわかるように、初期位置と現在地の座標差分を取得
        // transform.rotation = Quaternion.LookRotation(diff); 
        if (diff.sqrMagnitude > 0.0001f) //ベクトルの長さが0.01fより大きい場合にプレイヤーの向きを変える処理を入れる(0では入れないので）
        {
            transform.rotation = Quaternion.LookRotation(diff);  //ベクトルの情報をQuaternion.LookRotationに引き渡し回転量を取得しプレイヤーを回転させる
        }
        if (rb.velocity.sqrMagnitude < speed*speed)
        {
            rb.AddForce(transform.forward * speed);
        }

        Player_pos = transform.position; //プレイヤーの位置を更新
        if (collisionNum <=0)
        {
            playerScript.availableGunNum++;
            Destroy(this.gameObject);
        }


    }

    void OnCollisionExit(Collision other) 
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Compter" )
        {
            playerScript.availableGunNum++;
            Destroy(this.gameObject);
        }
        collisionNum--;
    }

}