using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelMove : MonoBehaviour
{

    [SerializeField] PolygonCollider2D mapBoundary;
    CinemachineConfiner confiner;

    [SerializeField] Direction direction;
    [SerializeField] float additivePas = 4f;
    enum Direction { Up, Down , Left, Right}

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner.m_BoundingShape2D = mapBoundary;
            UpdatePlayerPosition(collision.gameObject);
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPas = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPas.y += additivePas;
                break;
            case Direction.Down:
                newPas.y -= additivePas;
                break;
            case Direction.Left:
                newPas.x += additivePas;
                break;
            case Direction.Right:
                newPas.x -= additivePas;
                break;
        }

        player.transform.position = newPas;
    }

}
