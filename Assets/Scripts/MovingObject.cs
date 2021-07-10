using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;            //オブジェクトの移動にかかる時間（秒単位）
    public LayerMask blockingLayer;            //衝突がチェックされるレイヤー


    private BoxCollider2D boxCollider;         //このオブジェクトにアタッチされた、BoxCollider2Dの入れ物を用意
    private Rigidbody2D rb2D;                //このオブジェクトにアタッチされた、Rigidbody2Dの入れ物を用意
    private float inverseMoveTime;            //動きをより効率的にするために使用
    private bool isMoving;                    //動けるかどうか
                                              // Start is called before the first frame update
    protected virtual void Start()
    {
        //このオブジェクトのBoxCollider2Dへのコンポーネント参照を取得します
        boxCollider = GetComponent<BoxCollider2D>();

        //このオブジェクトのRigidbody2Dへのコンポーネント参照を取得します
        rb2D = GetComponent<Rigidbody2D>();

        //移動時間の逆数を保存することで、除算ではなく乗算で使用できるため、より効率的です。
        inverseMoveTime = 1f / moveTime;
    }

    //移動できる場合はtrueを、移動できない場合はfalseを返します。
    // Moveはx方向、y方向、およびRaycastHit2Dのパラメーターを取り、衝突をチェックします。
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        //オブジェクトの開始位置を保存します。(現在位置)
        Vector2 start = transform.position;

        //Moveを呼び出すときに渡される方向パラメーターに基づいて終了位置を計算します。（移動後の位置）
        Vector2 end = start + new Vector2(xDir, yDir);

        //boxColliderを無効にして、ラインキャストがこのオブジェクト自身のコライダーに当たらないようにします。
        boxCollider.enabled = false;

        //始点から終点までラインをキャストして、blockingLayerの衝突をチェックします。(ここで自分のオブジェクトとの接触判定が出ないようにfalseしている)
        hit = Physics2D.Linecast(start, end, blockingLayer);

        //ラインキャスト後にboxColliderを再度有効にします
        boxCollider.enabled = true;

        //何かがヒットしたかどうかを確認します
        if (hit.transform == null && !isMoving)
        {
            //何もヒットしなかった場合は、Vector2エンドを宛先として渡してSmoothMovementコルーチンを開始します。
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        //何かがヒットした場合は、falseを返し、行動はできない
        return false;
    }


    //ユニットを今のスペースから次のスペースに移動するためのコルーチン。endを使用して移動先を指定します。
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true;

        //現在の位置と終了パラメーターの差の2乗の大きさに基づいて、移動する残りの距離を計算します。
        //計算量が少ないため、等級の代わりに平方等級使用。(sqrMagnitudeは返り値をベクトルの二乗にする)
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //残りの移動距離がイプシロン(ほぼゼロ)より大きい間
        while (sqrRemainingDistance > float.Epsilon)
        {
            //newPostionに、移動途中の位置を設定(現在位置、目的位置、呼び出されるごと(1フレーム)に移動する距離)
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //アタッチされたRigidbody2DでMovePositionを呼び出し、それを計算された位置に移動します。
            rb2D.MovePosition(newPostion);

            //移動後の残り距離を再計算します
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //ループを終了するためにsqrRemainingDistanceがゼロに近づくまで戻り、ループする
            yield return null;
        }
        //移動距離がイプシロンより小さくなった時、終了地点まで移動する
        rb2D.MovePosition(end);

        //移動判定をfalseに変更する
        isMoving = false;
    }


    //仮想キーワード(virtual)は、オーバーライドキーワードを使用してクラスを継承することでAttemptMoveをオーバーライドできることを意味します。
    //AttemptMoveは、ジェネリックパラメーターTを取り、ブロックされた場合(移動できない)にユニットが操作するコンポーネントのタイプを指定します。
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        //hitを宣言
        RaycastHit2D hit;

        //移動が成功した場合はcanMoveをtrueに設定し、失敗した場合はfalseに設定します。
        bool canMove = Move(xDir, yDir, out hit);

        //ラインキャストの影響を受けていないか確認する
        if (hit.transform == null)
        {
            //何もヒットしなかった場合、これ以上AttemptMove関数のコードを実行しません。
            return;
        }


        //ヒットしたオブジェクトに接続されているタイプTのコンポーネントへのコンポーネント参照を取得します
        //敵にはプレイヤーと壁、プレイヤーには壁と敵）
        T hitComponent = hit.transform.GetComponent<T>();

        //canMoveがfalseで、hitComponentがnullに等しくない場合、つまり、MovingObjectがブロックされ、相互作用できる何かにヒットしたことを意味します
        if (!canMove && hitComponent != null)
        {
            //OnCantMove関数を呼び出し、パラメータとしてhitComponentを渡します。
            OnCantMove(hitComponent);
        }
    }
    //OnCantMoveは、継承するクラスの関数によってオーバーライドされます。
    //障害物にぶつかり移動できない時に呼び出す
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}


