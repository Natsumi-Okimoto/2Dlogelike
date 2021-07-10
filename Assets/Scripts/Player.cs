using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;        //���x�����Ďn������܂ł̕b�P�ʂ̒x�����ԁB(�X�e�[�W�̂���)
    public int pointsPerFood = 10;                //�t�[�h�I�u�W�F�N�g���E���Ƃ��Ƀv���[���[�̃t�[�h�|�C���g�ɒǉ�����|�C���g�̐��B
    public int wallDamage = 1;                    //�v���C���[���ǂ��������Ƃ��ɕǂɗ^����_���[�W�B
    public Text foodText;

    private Animator animator;                    //�v���[���[�̃A�j���[�^�[�R���|�[�l���g�ւ̎Q�Ƃ��i�[���邽�߂Ɏg�p����܂��B
    private int food;                       //���x����(���̃X�e�[�W��)�Ƀv���C���[�̃t�[�h�|�C���g�̍��v��ۑ����邽�߂Ɏg�p����܂��B
                                            // Start is called before the first frame update
    protected override void Start()
    {
        //�v���[���[�̃A�j���[�^�[�R���|�[�l���g�ւ̃R���|�[�l���g�Q�Ƃ��擾����
        animator = GetComponent<Animator>();


        //���x���Ԃ�GameManager.instance�ɕۑ�����Ă��錻�݂̃t�[�h�|�C���g�̍��v���擾���܂��B
        food = GameManager.instance.playerFoodPoints;

        foodText.text = "Food:" + food;

        //MovingObject��{�N���X��Start�֐����Ăяo���܂��B
        base.Start();
    }
    //���̊֐��́A���삪�����܂��͔�A�N�e�B�u�ɂȂ����Ƃ��ɌĂяo����܂��B�i�G���A�ړ��̎��ɌĂяo�����j
    private void OnDisable()
    {
        //Player�I�u�W�F�N�g�������ɂȂ��Ă���ꍇ�́A���݂̃��[�J���t�[�h�̍��v��GameManager�ɕۑ����āA���̃��x���ōă��[�h�ł���悤�ɂ��܂��B
        GameManager.instance.playerFoodPoints = food;
    }

    // Update is called once per frame
    private void Update()
    {
        //�v���C���[�̔ԂłȂ��ꍇ�A�֐����I�����܂��B
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        int horizontal = 0;      //�����ړ��������i�[���邽�߂Ɏg�p����܂�
        int vertical = 0;        //�����ړ��������i�[���邽�߂Ɏg�p����܂��B


        //���̓}�l�[�W���[������͂��擾���A�����Ɋۂ߁A�����ɕۑ�����x���̈ړ�������ݒ肵�܂�
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //���̓}�l�[�W���[������͂��擾���A�����Ɋۂ߁A�����ɕۑ�����y���̈ړ�������ݒ肵�܂�
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //�����Ɉړ����邩�ǂ������m�F���A�ړ�����ꍇ�͐����Ƀ[���ɐݒ肵�܂��B(�Y���h�~)
        if (horizontal != 0)
        {
            vertical = 0;
        }
        //�����܂��͐����Ƀ[���ȊO�̒l�����邩�ǂ������m�F���܂�
        if (horizontal != 0 || vertical != 0)
        {
            //�W�F�l���b�N�p�����[�^�[Wall��n����AttemptMove���Ăяo���܂��B
            //����́A�v���C���[���ǂɑ��������ꍇ�v���C���[��wall�N���X�𑀍삷�邽�߂ł��B
            //�v���[���[���ړ�����������w�肷��p�����[�^�[�Ƃ��āA���������Ɛ��������ɓn���܂��B
            AttemptMove<Wall>(horizontal, vertical);
        }
    }
    // AttemptMove�́A��{�N���XMovingObject��AttemptMove�֐����I�[�o�[���C�h���܂�
    // AttemptMove�̓W�F�l���b�N�p�����[�^�[T���󂯎��܂��B����́A�v���[���[�̏ꍇ��Wall�^�C�v�ł���Ax�����y direc�̐������󂯎��܂�
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //�v���C���[���ړ����邽�тɁA�t�[�h�|�C���g�̍��v���猸�Z���܂��B
        food--;
        foodText.text = "Food:" + food;

        //��{�N���X��AttemptMove���\�b�h���Ăяo���A�R���|�[�l���gT�i���̏ꍇ��Wall�j�ƈړ�����x�����y������n���܂��B
        base.AttemptMove<T>(xDir, yDir);

        //�q�b�g�ɂ��AMove�ōs��ꂽLinecast�̌��ʂ��Q�Ƃł��܂��B
        RaycastHit2D hit;

        //�v���C���[���ړ����ăt�[�h�|�C���g���������̂ŁA�Q�[�����I���������ǂ������m�F���܂��B
        CheckIfGameOver();

        //�v���[���[�̃^�[�����I������̂ŁAGameManager��playersTurn�u�[���l��false�ɐݒ肵�܂��B
        GameManager.instance.playersTurn = false;
    }


    //OnCantMove�́AMovingObject�̒��ۊ֐�OnCantMove���I�[�o�[���C�h���܂��B
    //����́A�v���[���[�̏ꍇ�̓v���[���[���U�����Ĕj��ł���ǂł����ʓI�ȃp�����[�^�[T�����܂��B
    protected override void OnCantMove<T>(T component)
    {
        //hitWall���A�p�����[�^�[�Ƃ��ēn���ꂽ�R���|�[�l���g�Ɠ������Ȃ�悤�ɐݒ肵�܂��B
        Wall hitWall = component as Wall;

        //�U�����Ă���ǂ�DamageWall�֐����Ăяo���܂��B
        hitWall.DamageWall(wallDamage);

        //�v���[���[�̍U���A�j���[�V�������Đ�����ɂ́A�v���[���[�̃A�j���[�V�����R���g���[���[�̍U���g���K�[��ݒ肵�܂��B
        animator.SetTrigger("chop");
    }

    //Player�ƃg���K�[���Ԃ�������
    //OnTriggerEnter2D�́A�g���K�[�ݒ肵���I�u�W�F�N�g�ƂԂ���ƌĂяo�����
    private void OnTriggerEnter2D(Collider2D other)
    {
        //�Փ˂����g���K�[�̃^�O��Exit�ł��邩�m�F���Ă��������B
        if (other.tag == "Exit")
        {
            //1�b��Ɏ��̃��x���i�X�e�[�W�j���J�n���邽�߂ɁARestart�֐����Ăяo���܂��B
            Invoke("Restart", restartLevelDelay);

            //���x�����I������̂ŁA�v���[���[�I�u�W�F�N�g�𖳌��ɂ��܂��B
            enabled = false;
        }

        //�Փ˂����g���K�[�̃^�O��Food�ł��邩�m�F���Ă��������B
        else if (other.tag == "Food")
        {
            //�v���C���[�̌��݂̃t�[�h�̍��v��pointsPerFood��ǉ����܂��B
            food += pointsPerFood;

            //�H�ו����E�������ɉ��Z���ĕ\��
            foodText.text = "+" + pointsPerFood + "Food:" + food;

            //�A�C�e���擾��A��\��
            other.gameObject.SetActive(false);
        }
    }

    //Restart�͌Ăяo���ꂽ�Ƃ��ɃV�[���������[�h���܂��B
    private void Restart()
    {

        //�Ō�Ƀ��[�h���ꂽ�V�[�������[�h���܂��B���̏ꍇ��Main�A�Q�[�����̗B��̃V�[���ł��B �����āA������u�V���O���v���[�h�Ń��[�h���āA�����̂��̂�u�������܂�
        //���݂̃V�[���̂��ׂẴV�[���I�u�W�F�N�g�����[�h���܂���B
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

    }


    //LoseFood�́A�G���v���C���[���U�������Ƃ��ɌĂяo����܂��B
    //�����|�C���g�̐����w�肷��p�����[�^�[�������Ƃ�܂��B
    public void LoseFood(int loss)
    {
        //�v���[���[�A�j���[�^�[�̃g���K�[��ݒ肵�āAplayerHit�A�j���[�V�����ɑJ�ڂ��܂��B
        animator.SetTrigger("hit");

        //�v���C���[�̍��v���玸��ꂽ�t�[�h�|�C���g�����������܂��B
        food -= loss;

        foodText.text = "-" + loss + "Food:" + food;

        //�Q�[�����I���������ǂ������m�F���܂�
        CheckIfGameOver();
    }


    //CheckIfGameOver�́A�v���[���[���t�[�h�|�C���g�𒴂��Ă��邩�ǂ������`�F�b�N���A����Ȃ��ꍇ�̓Q�[�����I�����܂��B
    private void CheckIfGameOver()
    {
        //�t�[�h�|�C���g�̎c�肪0���Ⴂ�A�܂��͓����ꍇ
        if (food <= 0)
        {
            //GameManager��GameOver�֐����Ăяo���܂��B
            GameManager.instance.GameOver();
        }
    }
}
