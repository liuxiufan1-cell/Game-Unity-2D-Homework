using UnityEngine;

public class BGScroll : MonoBehaviour
{
    [SerializeField, Header("スクロール速度率"), Range(0, 1)]
    private float scrollPer;

    public GameObject _camera;
    private Vector3 startPosition;
    private Vector3 scrollPosition = Vector3.zero;


    private void Start()
    {
        startPosition = this.transform.position;
    }

    void Update()
    {
        ScrollMove();
    }

    private void ScrollMove()
    {
        float dist = _camera.transform.position.x * scrollPer;

        scrollPosition = startPosition;
        scrollPosition.x += dist;
        this.transform.position = scrollPosition;
    }
}
