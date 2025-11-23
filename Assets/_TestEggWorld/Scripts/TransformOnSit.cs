using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 椅子に座ったら箱化/球体化などする
/// </summary>
public class TransformOnSit : UdonSharpBehaviour
{
    [Header("SetReplacementCamera")]
    //シェーダを適応するカメラ
    public Camera replacementCamera;
    //適応するシェーダ
    public Shader transformShader;

    [Header("ChairObject")]
    //椅子オブジェクト　椅子の位置を認識するためだが，TF後は非表示にする
    [SerializeField] private GameObject chairObject;

    [Header("SittingPositionChange")]
    public Transform slideTarget; // Stationに対して上下スライドさせるオブジェクト
    public float slideSpeed = 0.05f;
    public float maxOffset = 0.5f;
    public float minOffset = -0.2f;
    private Vector3 onSitPosition;

    [HideInInspector] public Animator animator;
    public GameObject chairRoot;

    [HideInInspector] public VRC.SDKBase.VRCStation freezeStation; //インタラクションで座る用のステーション

    //isSeatedはVRCStationのSeatedとは別物
    [HideInInspector] public bool isSeated = false;
    private float currentOffset = 0f;

    public void Start()
    {
        freezeStation = GetComponent<VRC.SDKBase.VRCStation>();

        // Shader を取得
        if (transformShader == null)
        {
            Debug.LogError("Boxify Shader not found!");
            return;
        }

        // AnimatorとChairRootを取得
        //それぞれ取得できなくてもこのスクリプト自体は動くのでエラーにはしない
        //これらは継承先で使うことを想定している
        animator = GetComponent<Animator>();
        chairRoot = this.transform.root.gameObject;

        // ReplacementShader を Camera に設定
        replacementCamera.SetReplacementShader(transformShader, "");
    }

    //椅子にすわったらカメラオン
    public override void OnStationEntered(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        replacementCamera.GetComponent<Camera>().enabled = true;
        isSeated = true;
        currentOffset = 0f;
        onSitPosition = transform.position;
        if (chairObject != null)
            chairObject.SetActive(false);
    }

    //椅子から離れたらカメラオフ
    public override void OnStationExited(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        replacementCamera.GetComponent<Camera>().enabled = false;
        isSeated = false;
        currentOffset = 0f;
        if (chairObject != null)
            chairObject.SetActive(true);

        if (slideTarget != null)
            slideTarget.position = onSitPosition; // 元の位置に戻す
    }

    //移動キー入力を受け付ける　デフォはこれだけどオーバーライドできるようにする
    public virtual void Update()
    {
        if (!isSeated || slideTarget == null) return;

        float move = 0f;
        if (Input.GetKey(KeyCode.E)) move = slideSpeed;
        if (Input.GetKey(KeyCode.Q)) move = -slideSpeed;

        if (move != 0f)
        {
            currentOffset = Mathf.Clamp(currentOffset + move, minOffset, maxOffset);
            Vector3 pos = slideTarget.localPosition;
            pos.y = currentOffset;
            slideTarget.localPosition = pos;
        }
    }

}
