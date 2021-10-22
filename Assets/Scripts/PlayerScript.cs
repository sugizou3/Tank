using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] Player_ScriptableObject player_sb;

    private Vector3 Player_pos; //プレイヤーのポジション
    private float x; //x方向のImputの値
    private float z; //z方向のInputの値
    private Rigidbody rb;
    private Transform SpownObj;
    public int availableGunNum = 3;

    void Start()
    {
        Player_pos = GetComponent<Transform>().position; //最初の時点でのプレイヤーのポジションを取得
        rb = GetComponent<Rigidbody>(); //プレイヤーのRigidbodyを取得
        SpownObj = transform.GetChild(0);
        availableGunNum = player_sb.GunNum;
    }

    void Update()
    {

        x = Input.GetAxis("Horizontal"); //x方向のInputの値を取得
        z = Input.GetAxis("Vertical"); //z方向のInputの値を取得
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
        if (Input.GetKeyDown(KeyCode.Space) && availableGunNum >0)
        {
            CreateGun();
            --availableGunNum;
        }


    }

    private void CreateGun()
    {
        GameObject Gun = Instantiate(player_sb.GunPrefab,SpownObj.transform.position, SpownObj.transform.rotation) as GameObject;
        GunScript gunScript = Gun.GetComponent<GunScript>();
        gunScript.speed = player_sb.GunSpeed * 0.1f;
        gunScript.collisionNum = player_sb.GunBounceNum;
        gunScript.playerScript = this;
    }
    public void HitGun()
    {

    }
    public void DestroyEnemy()
    {

    }
}
