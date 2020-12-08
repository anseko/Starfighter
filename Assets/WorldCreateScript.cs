using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreateScript : MonoBehaviour
{
    public GameObject asteroid_1;
    public GameObject asteroid_2;
    private GameObject asteroid_type;
    private GameObject asteroid;
    private int asteroidShow;
    private Vector3 playerPosition;
    public static int[,] nums = { { -10, 0, 60, 1, 0 },
                                  { 10, 1, 50, 2, 0 },
                                  { 20, 2, 30, 1, 0 }
                                  /*{ -10, 3, 60, 1, 0 },
                                  { 10, 4, 50, 2, 0 },
                                  { 20, 5, 30, 1, 0 },
                                  { -10, 6, 60, 1, 0 },
                                  { 10, 7, 50, 2, 0 },
                                  { 20, 8, 30, 1, 0 },
                                  { -10, 9, 60, 1, 0 },
                                  { 10, 10, 50, 2, 0 },
                                  { 20, 11, 30, 1, 0 },
                                  { -10, 12, 60, 1, 0 },
                                  { 10, 13, 50, 2, 0 },
                                  { 20, 14, 30, 1, 0 },
                                  { -10, 15, 60, 1, 0 },
                                  { 10, 16, 50, 2, 0 },
                                  { -10, 17, 60, 1, 0 },
                                  { 10, 18, 50, 2, 0 },
                                  { 20, 19, 30, 1, 0 },
                                  { -10, 20, 60, 1, 0 },
                                  { 10, 21, 50, 2, 0 },
                                  { 20, 22, 30, 1, 0 },
                                  { -10, 23, 60, 1, 0 },
                                  { 10, 24, 50, 2, 0 },
                                  { 20, 25, 30, 1, 0 },
                                  { 20, 26, 30, 1, 0 },
                                  { -10, 27, 60, 1, 0 },
                                  { 10, 28, 50, 2, 0 },
                                  { 20, 29, 30, 1, 0 },
                                  { -10, 30, 60, 1, 0 },
                                  { 10, 31, 50, 2, 0 },
                                  { 20, 32, 30, 1, 0 },
                                  { -10, 33, 60, 1, 0 },
                                  { 10, 34, 50, 2, 0 },
                                  { 20, 35, 30, 1, 0 },
                                  { -10, 36, 60, 1, 0 },
                                  { 10, 37, 50, 2, 0 },
                                  { 20, 38, 30, 1, 0 },
                                  { -10, 39, 60, 1, 0 },
                                  { 10, 40, 50, 2, 0 },
                                  { 20, 41, 30, 1, 0 },
                                  { -10, 42, 60, 1, 0 },
                                  { 10, 43, 50, 2, 0 },
                                  { -10, 44, 60, 1, 0 },
                                  { 10, 45, 50, 2, 0 },
                                  { 20, 46, 30, 1, 0 },
                                  { -10, 47, 60, 1, 0 },
                                  { 10, 48, 50, 2, 0 },
                                  { 20, 49, 30, 1, 0 },
                                  { -10, 50, 60, 1, 0 },
                                  { 10, 51, 50, 2, 0 },
                                  { 20, 52, 30, 1, 0 },
                                  { 20, 53, 30, 1, 0 } */};
    static int rows = nums.GetUpperBound(0) + 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = GameObject.Find("Player").transform.position;
        for (int i = 0; i < rows; i++)
        {
            Vector3 asteroidPosition = new Vector3(nums[i, 0], nums[i, 1], nums[i, 2]);
            if (nums[i, 3] == 1) asteroid_type = asteroid_1;
            if (nums[i, 3] == 2) asteroid_type = asteroid_2;
            asteroidShow = nums[i, 4];
            float deltaX = Math.Abs(asteroidPosition[0] - playerPosition[0]);
            float deltaZ = Math.Abs(asteroidPosition[2] - playerPosition[2]);
            asteroidShow = nums[i, 4];
            if (deltaX < 45 && deltaZ < 45 && asteroidShow == 0)
            {
                asteroidShow = nums[i, 4] = 1;
                asteroid = Instantiate(asteroid_type, asteroidPosition, Quaternion.identity);
                asteroid.GetComponent<AsteroidScript>().val = i;
            }
            /*else if (deltaX > 45 || deltaZ > 45)
            {
                asteroidShow = nums[i, 4] = 0;
                Destroy(GameObject);
            }*/

        }
    }
}
