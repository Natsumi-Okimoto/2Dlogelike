using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;                             //攻撃時にプレイヤーから差し引くフードポイントの量。


    private Animator animator;                            //敵のAnimatorコンポーネントへの参照を格納するAnimator型の変数。
    private Transform target;                            //各ターンに移動しようとする目的object
    private bool skipMove;                                //敵がターンをスキップするか、このターンを移動するかどうかを決定するブール値。
                                                          // Start is called before the first frame update
    protected override void Start()
    {
        // Enemyオブジェクトのリストに追加して、この敵をGameManagerのインスタンスに登録します。
        //これにより、GameManagerが移動コマンドを発行できるようになります。
        GameManager.instance.AddEnemyToList(this);

        //添付されたAnimatorコンポーネントへの参照を取得して保存します。
        animator = GetComponent<Animator>();

        //タグを使用してPlayer GameObjectを見つけ、transformを保存します。
        target = GameObject.FindGameObjectWithTag("Player").transform;

        //スタート関数を抽象クラスから呼ぶ
        base.Start();
    }

    //Enemyがターンをスキップするために必要な機能を含めるには、MovingObjectのAttemptMove関数をオーバーライドします。
    //基本的なAttemptMove関数の動作の詳細については、MovingObjectのコメントを参照してください。
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //skipMoveがtrueかどうかを確認し、trueの場合はfalseに設定して、このターンをスキップします。
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        //MovingObjectからAttemptMove関数を呼び出します。
        base.AttemptMove<T>(xDir, yDir);

        //Enemyが移動したので、skipMoveをtrueに設定して次の移動をスキップします。
        skipMove = true;
    }
    //MoveEnemyは毎ターンGameMangerによって呼び出され、各敵にプレイヤーに向かって移動するように指示します。
    public void MoveEnemy()
    {
        // X軸とY軸の移動方向の変数を宣言します。これらの範囲は-1から1です。
        //これらの値により、基本的な方向（上、下、左、右）を選択できます。
        int xDir = 0;
        int yDir = 0;

        //x軸がイプシロン(ほぼ)の方が大きい場合
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            //ターゲット（プレーヤー）の位置のy座標がこの敵の位置のy座標より大きい場合は、y方向1（上に移動）を設定します。 そうでない場合は、-1に設定します（下に移動します）。
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        //y軸が同じ場合
        else
        {
            //ターゲットのx位置が敵のx位置より大きいかどうかを確認します。そうであれば、x方向を1（右に移動）に設定し、そうでなければ-1（左に移動）に設定します。
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }


        //エネミーは移動していて、プレーヤーに遭遇する可能性があるため、AttemptMove関数を呼び出してジェネリックパラメーターPlayerを渡します。
        AttemptMove<Player>(xDir, yDir);
    }


    // OnCantMoveは、Enemyがプレーヤーが占有するスペースに移動しようとすると呼び出され、MovingObjectのOnCantMove関数をオーバーライドします
    //また、遭遇すると予想されるコンポーネント、この場合はPlayerに渡すために使用する汎用パラメーターTを受け取ります
    protected override void OnCantMove<T>(T component)
    {
        //hitPlayerを宣言し、遭遇したコンポーネントと等しくなるように設定します。
        Player hitPlayer = component as Player;

        //hitPlayerのLoseFood関数を呼び出して、減算するフードポイントの量であるplayerDamageを渡します。
        hitPlayer.LoseFood(playerDamage);

        //アニメータの攻撃トリガーを設定して、敵の攻撃アニメーションをトリガーします。
        animator.SetTrigger("EnemyAttack");

    }

}
