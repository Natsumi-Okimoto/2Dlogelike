using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                        //���x�����J�n����O�ɑҋ@���鎞�ԁi�b�P�ʁj�B
    public float turnDelay = 0.1f;                            //�e�v���C���[�̃^�[���Ԃ̒x���B


    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    
    public bool playersTurn = true;

    public Text levelText;
    public GameObject levelImage;
    public int level = 1;

    private List<Enemy> enemies;                            //�ړ��R�}���h�𔭍s���邽�߂Ɏg�p����邷�ׂĂ̓G���j�b�g�̃��X�g�B
    private bool enemiesMoving;                                //enemy�̃^�[�����`�F�b�N
    private bool doingSetup;
    // Start is called before the first frame update
    void Awake()
    {
        //�C���X�^���X�̊m�F
        if (instance == null)
        {
            //�Ȃ��ꍇ�A�C���X�^���X�ɐݒ肷��
            instance = this;
        }
        //�C���X�^���X�����݂��邪�A����ł͂Ȃ��ꍇ
        else if (instance != this)
        {
            //�j�󂵂܂��B ����ɂ��A�V���O���g���p�^�[�����K�p����܂��B
            //�܂�AGameManager�̃C���X�^���X��1�������݂ł��܂���B
            Destroy(gameObject);
        }

        //�V�[���������[�h����Ƃ��ɂ��ꂪ�j������Ȃ��悤�ɐݒ肵�܂�
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //�V�[�����ǂݍ��܂�邽�тɌĂяo�����R�[���o�b�N��o�^���܂�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    //�e���x���̃Q�[�������������܂��B
    public void InitGame()
    {
        //doingSetup��true�̏ꍇ�A�v���[���[�͈ړ��ł��܂���B�^�C�g���J�[�h���A�b�v���Ă���Ԃ̓v���[���[���ړ����Ȃ��悤�ɂ��܂��B
        doingSetup = true;

        //���O�Ō������āA�摜Levelimage�ւ̎Q�Ƃ��擾���܂��B
        levelImage = GameObject.Find("Levelimage");

        //���O�Ō�������GetComponent���Ăяo�����Ƃɂ��A�e�L�X�gLevelText�̃e�L�X�g�R���|�[�l���g�ւ̎Q�Ƃ��擾���܂��B
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //levelText�̃e�L�X�g�𕶎���uDay�v�ɐݒ肵�A���݂̃��x���ԍ���ǉ����܂��B
        levelText.text = "Day " + level;

        //�Z�b�g�A�b�v����levelImage���Q�[���{�[�h�̃A�N�e�B�u�ȃu���b�L���O�v���[���[�̃r���[�ɐݒ肵�܂��B
        levelImage.SetActive(true);

        //levelStartDelay��b�P�ʂŒx��������HideLevelImage�֐����Ăяo���܂��B
        Invoke("HideLevelImage", levelStartDelay);

        //���X�g���̓G�I�u�W�F�N�g�����ׂăN���A���āA���̃��x���ɔ����܂��B
        enemies.Clear();

        //BoardManager�X�N���v�g��SetupScene�֐����Ăяo���A���݂̃��x���ԍ���n���܂��B
        boardScript.SetupScene(level);

    }

    //���x���ԂŎg�p����鍕���摜���\���ɂ��܂�
    public void HideLevelImage()
    {
        //levelImage gameObject�𖳌��ɂ��܂��B
        levelImage.SetActive(false);

        //�v���[���[���Ăшړ��ł���悤�ɁAdoingSetup��false�ɐݒ肵�܂�
        doingSetup = false;
    }
    //�X�V�̓t���[�����ƂɌĂяo����܂��B
    void Update()
    {
        //playersTurn�܂���enemiesMoving�܂���doingSetup������true�łȂ����Ƃ��m�F���Ă��������B
        if (playersTurn || enemiesMoving || doingSetup)

            //�����̂����ꂩ��true�̏ꍇ�͖߂�AMoveEnemies���J�n���Ȃ��ł��������B
            return;

        //�G�l�~�[�̍s���֐����s
        StartCoroutine(MoveEnemies());
    }

    //������Ăяo���āA�n���ꂽ�G��G�I�u�W�F�N�g�̃��X�g�ɒǉ����܂��B
    public void AddEnemyToList(Enemy script)
    {
        //List�ɃG�l�~�[��ǉ�����
        enemies.Add(script);
    }


    //�v���C���[���t�[�h�|�C���g0�ɓ��B����ƁAGameOver���Ăяo����܂�
    public void GameOver()
    {
        //Set levelText to display number of levels passed and game over message
        levelText.text = "After " + level + " days, you starved.";

        //Enable black background image gameObject.
        levelImage.SetActive(true);

        //����GameManager�𖳌��ɂ��܂��B
        enabled = false;
    }

    //�G�����Ԃɓ������R���[�`���B
    IEnumerator MoveEnemies()
    {
        //enemiesMoving��true�ł����A�v���C���[�͈ړ��ł��܂���B
        enemiesMoving = true;

        //turnDelay�b�ҋ@���܂��B�f�t�H���g��.1�i100�~���b�j�ł��B
        yield return new WaitForSeconds(turnDelay);

        //�X�|�[�����ꂽ�G�����Ȃ��ꍇ�i��1���x����IE�j�F
        if (enemies.Count == 0)
        {
            //�ړ��̊Ԃ�turnDelay�b�ҋ@���A�����Ȃ��Ƃ��Ɉړ�����G�ɂ���Ĉ����N�������x����u�������܂��B
            yield return new WaitForSeconds(turnDelay);
        }

        //�G�I�u�W�F�N�g�̃��X�g�����[�v���܂��B
        for (int i = 0; i < enemies.Count; i++)
        {
            //�G���X�g�̃C���f�b�N�Xi�ɂ���G��MoveEnemy�֐����Ăяo���܂��B
            enemies[i].MoveEnemy();

            //�G��moveTime��҂��Ă���A���̓G���ړ����܂��B
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        //�G�̈ړ�������������AplayersTurn��true�ɐݒ肵�āA�v���[���[���ړ��ł���悤�ɂ��܂��B
        playersTurn = true;

        //�G�̈ړ�������������AenemiesMoving��false�ɐݒ肵�܂��B
        enemiesMoving = false;
    }
}


