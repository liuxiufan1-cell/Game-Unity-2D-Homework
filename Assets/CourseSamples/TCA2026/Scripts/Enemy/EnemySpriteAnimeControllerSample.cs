using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemySpriteAnimeControllerSample : MonoBehaviour
{
    public EnemyController2DSample _enemyController2D;
    private Animator _animator;

    private int _animeNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _animeNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        AnimeChange();
        ChaceAnimationEnd();
    }

    private void AnimeChange()
    {
        //アニメーション指定が変化したかチェック。
        if (_animeNumber != _enemyController2D!.animeNumber)
        {
            _animeNumber = _enemyController2D!.animeNumber;
            switch (_animeNumber)
            {
                case 0:
                case 1:
                case 2:
                    _animator.SetInteger("AnimeMode", _animeNumber);
                    break;
            }
        }
    }

    private void ChaceAnimationEnd()
    {
        // 現在再生中のアニメーション情報を取得
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // normalizedTimeが1以上（アニメーション終了）かつ、指定したステート名の場合
        if (stateInfo.normalizedTime >= 1.0f && stateInfo.IsName("enemyDeath"))
        {
            Debug.Log("敵のデスアニメーションが終了しました");

            _enemyController2D.OnDeath();
        }
    }


}
