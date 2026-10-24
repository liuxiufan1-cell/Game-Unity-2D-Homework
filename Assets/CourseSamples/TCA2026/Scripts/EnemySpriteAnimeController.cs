using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemySpriteAnimeController : MonoBehaviour
{
    public EnemyController2D _enemyController2D;
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

}
