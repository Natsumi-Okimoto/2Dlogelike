using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                        //レベルを開始する前に待機する時間（秒単位）。
    public float turnDelay = 0.1f;                            //各プレイヤーのターン間の遅延。


    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    
    public bool playersTurn = true;

    public Text levelText;
    public GameObject levelImage;
    public int level = 1;

    private List<Enemy> enemies;                            //移動コマンドを発行するために使用されるすべての敵ユニットのリスト。
    private bool enemiesMoving;                                //enemyのターンかチェック
    private bool doingSetup;
    // Start is called before the first frame update
    void Awake()
    {
        //インスタンスの確認
        if (instance == null)
        {
            //ない場合、インスタンスに設定する
            instance = this;
        }
        //インスタンスが存在するが、これではない場合
        else if (instance != this)
        {
            //破壊します。 これにより、シングルトンパターンが適用されます。
            //つまり、GameManagerのインスタンスは1つしか存在できません。
            Destroy(gameObject);
        }

        //シーンをリロードするときにこれが破棄されないように設定します
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //シーンが読み込まれるたびに呼び出されるコールバックを登録します
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    //各レベルのゲームを初期化します。
    public void InitGame()
    {
        //doingSetupがtrueの場合、プレーヤーは移動できません。タイトルカードがアップしている間はプレーヤーが移動しないようにします。
        doingSetup = true;

        //名前で検索して、画像Levelimageへの参照を取得します。
        levelImage = GameObject.Find("Levelimage");

        //名前で検索してGetComponentを呼び出すことにより、テキストLevelTextのテキストコンポーネントへの参照を取得します。
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //levelTextのテキストを文字列「Day」に設定し、現在のレベル番号を追加します。
        levelText.text = "Day " + level;

        //セットアップ中にlevelImageをゲームボードのアクティブなブロッキングプレーヤーのビューに設定します。
        levelImage.SetActive(true);

        //levelStartDelayを秒単位で遅延させてHideLevelImage関数を呼び出します。
        Invoke("HideLevelImage", levelStartDelay);

        //リスト内の敵オブジェクトをすべてクリアして、次のレベルに備えます。
        enemies.Clear();

        //BoardManagerスクリプトのSetupScene関数を呼び出し、現在のレベル番号を渡します。
        boardScript.SetupScene(level);

    }

    //レベル間で使用される黒い画像を非表示にします
    public void HideLevelImage()
    {
        //levelImage gameObjectを無効にします。
        levelImage.SetActive(false);

        //プレーヤーが再び移動できるように、doingSetupをfalseに設定します
        doingSetup = false;
    }
    //更新はフレームごとに呼び出されます。
    void Update()
    {
        //playersTurnまたはenemiesMovingまたはdoingSetupが現在trueでないことを確認してください。
        if (playersTurn || enemiesMoving || doingSetup)

            //これらのいずれかがtrueの場合は戻り、MoveEnemiesを開始しないでください。
            return;

        //エネミーの行動関数実行
        StartCoroutine(MoveEnemies());
    }

    //これを呼び出して、渡された敵を敵オブジェクトのリストに追加します。
    public void AddEnemyToList(Enemy script)
    {
        //Listにエネミーを追加する
        enemies.Add(script);
    }


    //プレイヤーがフードポイント0に到達すると、GameOverが呼び出されます
    public void GameOver()
    {
        //Set levelText to display number of levels passed and game over message
        levelText.text = "After " + level + " days, you starved.";

        //Enable black background image gameObject.
        levelImage.SetActive(true);

        //このGameManagerを無効にします。
        enabled = false;
    }

    //敵を順番に動かすコルーチン。
    IEnumerator MoveEnemies()
    {
        //enemiesMovingはtrueですが、プレイヤーは移動できません。
        enemiesMoving = true;

        //turnDelay秒待機します。デフォルトは.1（100ミリ秒）です。
        yield return new WaitForSeconds(turnDelay);

        //スポーンされた敵がいない場合（第1レベルのIE）：
        if (enemies.Count == 0)
        {
            //移動の間にturnDelay秒待機し、何もないときに移動する敵によって引き起こされる遅延を置き換えます。
            yield return new WaitForSeconds(turnDelay);
        }

        //敵オブジェクトのリストをループします。
        for (int i = 0; i < enemies.Count; i++)
        {
            //敵リストのインデックスiにある敵のMoveEnemy関数を呼び出します。
            enemies[i].MoveEnemy();

            //敵のmoveTimeを待ってから、次の敵を移動します。
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        //敵の移動が完了したら、playersTurnをtrueに設定して、プレーヤーが移動できるようにします。
        playersTurn = true;

        //敵の移動が完了したら、enemiesMovingをfalseに設定します。
        enemiesMoving = false;
    }
}


