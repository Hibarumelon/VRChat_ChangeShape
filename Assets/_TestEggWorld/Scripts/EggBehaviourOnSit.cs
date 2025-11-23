
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class EggBehaviourOnSit : TransformOnSit
{
    [Header("RigidBody")]
    public Rigidbody chairRb;
    public float forcePower = 10f;

    private float speed = 2.0f;
    
    public override void Update()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //エディタ用
        if (Input.GetKey(KeyCode.LeftArrow)) h = -1;
        if (Input.GetKey(KeyCode.RightArrow)) h = 1;
        if (Input.GetKey(KeyCode.UpArrow)) v = 1;
        if (Input.GetKey(KeyCode.DownArrow)) v = -1;

        Vector3 dir = new Vector3(h, 0, v);
        if (dir.sqrMagnitude > 0.01f && isSeated)
        {
            // 回転（移動方向に向く）
            chairRoot.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

            // 前進移動
            chairRoot.transform.position += dir.normalized * speed * Time.deltaTime;

            // ぴょんぴょんアニメ再生
            animator.SetBool("IsHopping", true);
        }
        else
        {
            animator.SetBool("IsHopping", false);
        }
    }

    //インタラクションしたときに身体を固定化
    public override void Interact()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player != null)
        {
            freezeStation.UseStation(player);
        }
    }
}
