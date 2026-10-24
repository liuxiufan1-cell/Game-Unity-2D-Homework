using UnityEngine;

public class AnimetionControllSample : MonoBehaviour
{

    private Animator _animator;

    public string _boolName = new string("switch");
    public bool _switchBool;
    public string _intName = new string("switchint");
    public int _switchInt;

    private int _hashBoolName;
    private int _hashIntName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();

        // 文字列からパラメータID（ハッシュ値）を取得
        _hashBoolName = Animator.StringToHash(_boolName);
        _hashIntName = Animator.StringToHash(_intName);
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool(_hashBoolName, _switchBool);
        _animator.SetInteger(_hashIntName, _switchInt);
    //    _animator.SetBool(_boolName, _switchBool);
    //    _animator.SetInteger(_intName, _switchInt);
    }
}
