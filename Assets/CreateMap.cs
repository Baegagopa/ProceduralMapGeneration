using UnityEngine;
using System.Collections;

public class CreateMap : MonoBehaviour
{
    enum GroundType
    {
        WATER = 0,
        GRASSLAND = 1,
    };

    Vector3[] vertexes;
    int[] triangles;
    Color[] colors;
    GroundType[] vertexesGroundType;

    // 맵배경
    public Transform mapBackgroundTr;
    public GameObject vertexPrefab;
    ItemPool vertex = null;

    // 맵을 분할할 크기
    public int numOf_X = 2;
    public int numOf_Y = 2;

    // Vertex 위치 보정
    [Range(0f, 0.5f)]
    public float AreaRreviseRate = 0.1f; // 보정수치

    // 맵의 넓이
    float mapAreaWidth;
    float mapAreaheight;

    // 맵의 좌측 상단기준 좌표
    float mapArea_X;
    float mapArea_Y;

    // 1칸당 크기 (mapAreaWidth / numOf_X)
    float mapArea_PieceSize_X;
    float mapArea_PieceSize_Y;

    void Awake()
    {
        
//        vertex = gameObject.AddComponent<ItemPool>();
//        vertex.CreateItemPool(vertexPrefab, numOf_X * numOf_Y);     // 버텍스 오브젝트 풀

        vertexes = new Vector3[numOf_X * numOf_Y];
        triangles = new int[(numOf_X - 1) * (numOf_Y - 1) * 6];
        colors = new Color[numOf_X * numOf_Y];
        vertexesGroundType = new GroundType[numOf_X * numOf_Y];

        mapAreaWidth = mapBackgroundTr.localScale.x;
        mapAreaheight = mapBackgroundTr.localScale.y;

        mapArea_PieceSize_X = mapAreaWidth / numOf_X;
        mapArea_PieceSize_Y = mapAreaheight / numOf_Y;

        mapArea_X = mapBackgroundTr.position.x - (mapAreaWidth * 0.5f);
        mapArea_Y = mapBackgroundTr.position.y + (mapAreaheight * 0.5f);


    }

    public void MakeLand()
    {
        MakeRadial();

        int y, x, triangleCnt=0;
        // 점을 뿌려줌 피봇은 좌측 상단
        for (y = 0; y < numOf_Y; y++)
        {
            for (x = 0; x < numOf_X; x++)
            {
                vertexes[(y * numOf_X) + x].x = mapArea_X + Random.Range((mapArea_PieceSize_X * x) + (mapArea_PieceSize_X * AreaRreviseRate),
                                                                         (mapArea_PieceSize_X * (x + 1)) - (mapArea_PieceSize_X * AreaRreviseRate));
                vertexes[(y * numOf_X) + x].y = mapArea_Y - Random.Range((mapArea_PieceSize_Y * y) + (mapArea_PieceSize_Y * AreaRreviseRate),
                                                                         (mapArea_PieceSize_Y * (y + 1)) - (mapArea_PieceSize_Y * AreaRreviseRate));
            }
        }

        // 테두리
        // 좌측 상단
        vertexes[0].x = mapArea_X;
        vertexes[0].y = mapArea_Y;

        // 우측 상단
        vertexes[numOf_X - 1].x = mapArea_X + mapAreaWidth;
        vertexes[numOf_Y - 1].y = mapArea_Y;

        // 좌측 하단
        vertexes[(numOf_Y - 1) * numOf_X].x = mapArea_X;
        vertexes[(numOf_Y - 1) * numOf_X].y = mapArea_Y - mapAreaheight;

        // 우측 상단
        vertexes[vertexes.Length - 1].x = mapArea_X + mapAreaWidth;
        vertexes[vertexes.Length - 1].y = mapArea_Y - mapAreaheight;

        // 상하단 위치 정렬
        for (x = 1; x < numOf_X - 1; x++)
        {
            vertexes[x].x = mapArea_X + (mapArea_PieceSize_X * x) + (mapArea_PieceSize_X * 0.5f);
            vertexes[x].y = mapArea_Y;
            vertexes[((numOf_Y - 1) * numOf_X) + x].x = mapArea_X + (mapArea_PieceSize_X * x) + (mapArea_PieceSize_X * 0.5f);
            vertexes[((numOf_Y - 1) * numOf_X) + x].y = mapArea_Y - mapAreaheight;
        }

        // 좌우측 위치 정렬
        for (y = 1; y < numOf_Y - 1; y++)
        {
            vertexes[y * numOf_X].x = mapArea_X;
            vertexes[y * numOf_X].y = mapArea_Y - (mapArea_PieceSize_Y * y) - (mapArea_PieceSize_Y * 0.5f);
            vertexes[(y * numOf_X) + numOf_X - 1].x = mapArea_X + mapAreaWidth;
            vertexes[(y * numOf_X) + numOf_X - 1].y = mapArea_Y - (mapArea_PieceSize_Y * y) - (mapArea_PieceSize_Y * 0.5f);
        }
        

        // 점을 그려줌
        for (y = 0; y < numOf_Y; y++)
        {
            for (x = 0; x < numOf_X; x++)
            {

                
                if (Island(vertexes[(y * numOf_X) + x]))
                {
                    colors[(y * numOf_X) + x] = new Color(0.2f, 1f, 0.2f, 1f);
                    vertexesGroundType[(y * numOf_X) + x] = GroundType.GRASSLAND;
                    
                }
                else
                {
                    colors[(y * numOf_X) + x] = new Color(0.4f, 0.4f, 0.9f, 1f);
                    vertexesGroundType[(y * numOf_X) + x] = GroundType.WATER;
                }

                // vertex.UseItem().transform.position = new Vector3(vertexes[(y * numOf_X) + x].x, vertexes[(y * numOf_X) + x].y, -0.5f);                     // 버텍스 출력
            }
        }

        /*
        // 테두리 바다로 만들어줌
        // 좌측 상단
        colors[0] = new Color(0.4f, 0.4f, 0.9f, 1f);
        vertexesGroundType[0] = GroundType.WATER;

        // 우측 상단
        colors[numOf_X - 1] = new Color(0.4f, 0.4f, 0.9f, 1f);
        vertexesGroundType[numOf_X - 1] = GroundType.WATER;

        // 좌측 하단
        colors[(numOf_Y - 1) * numOf_X] = new Color(0.4f, 0.4f, 0.9f, 1f);
        vertexesGroundType[(numOf_Y - 1) * numOf_X] = GroundType.WATER;

        // 우측 상단
        colors[vertexes.Length - 1] = new Color(0.4f, 0.4f, 0.9f, 1f);
        vertexesGroundType[vertexes.Length - 1] = GroundType.WATER;

        // 상하단 위치 정렬
        for (x = 1; x < numOf_X - 1; x++)
        {

            colors[x] = new Color(0.4f, 0.4f, 0.9f, 1f);
            vertexesGroundType[x] = GroundType.WATER;

            colors[x + numOf_Y] = new Color(0.4f, 0.4f, 0.9f, 1f);
            colors[x + numOf_Y*2] = new Color(0.4f, 0.4f, 0.9f, 1f);

            colors[((numOf_Y - 1) * numOf_X) + x] = new Color(0.4f, 0.4f, 0.9f, 1f);
            vertexesGroundType[((numOf_Y - 1) * numOf_X) + x] = GroundType.WATER;
        }

        // 좌우측 위치 정렬
        for (y = 1; y < numOf_Y - 1; y++)
        {

            colors[y * numOf_X] = new Color(0.4f, 0.4f, 0.9f, 1f);
            vertexesGroundType[y * numOf_X] = GroundType.WATER;
            colors[(y * numOf_X) + numOf_X - 1] = new Color(0.4f, 0.4f, 0.9f, 1f);
            vertexesGroundType[(y * numOf_X) + numOf_X - 1] = GroundType.WATER;
        }
        */


        for (y = 0; y < numOf_Y - 1; y++)
        {
            for (x = 0; x < numOf_X - 1; x++)
            {
                // 세 점 사이의 각도
                // 배열이 아래와 같이 존재한다고 가정 했을 때
                //  0   1  ....
                // 10  11  ....
                // 점 1, 0, 10이 있을 때, 점 0을 사잇값일 때 각도는 아래와 같이 구한다 
                // Vector3.Angle(vertexes[0] - vertexes[1], vertexes[0] - vertexes[10])

                // 좌측 상단, 우측 하단 두 각의 합이 180이 넘지 않는다면
                if (Vector3.Angle(vertexes[(y * numOf_X) + x] - vertexes[(y * numOf_X) + x + 1],
                                  vertexes[(y * numOf_X) + x] - vertexes[(y * numOf_X) + x + numOf_X])
                  + Vector3.Angle(vertexes[(y * numOf_X) + x + numOf_X + 1] - vertexes[(y * numOf_X) + x + 1],
                                  vertexes[(y * numOf_X) + x + numOf_X + 1] - vertexes[(y * numOf_X) + x + numOf_X]) < 180)
                {
                    triangles[triangleCnt * 3] = (y * numOf_X) + x + 1;
                    triangles[triangleCnt * 3 + 1] = (y * numOf_X) + x + numOf_X;
                    triangles[triangleCnt * 3 + 2] = (y * numOf_X) + x;
                    triangleCnt++;

                    triangles[triangleCnt * 3] = (y * numOf_X) + x + 1;
                    triangles[triangleCnt * 3 + 1] = (y * numOf_X) + x + numOf_X + 1;
                    triangles[triangleCnt * 3 + 2] = (y * numOf_X) + x + numOf_X;
                    triangleCnt++;

                    
                }
                // 좌측 상단, 우측 하단 두 각의 합이 180이 넘으면
                else
                {
                    triangles[triangleCnt * 3] = (y * numOf_X) + x;
                    triangles[triangleCnt * 3 + 1] = (y * numOf_X) + x + numOf_X + 1;
                    triangles[triangleCnt * 3 + 2] = (y * numOf_X) + x + numOf_X;
                    triangleCnt++;

                    triangles[triangleCnt * 3] = (y * numOf_X) + x;
                    triangles[triangleCnt * 3 + 1] = (y * numOf_X) + x + 1;
                    triangles[triangleCnt * 3 + 2] = (y * numOf_X) + x + numOf_X + 1;
                    triangleCnt++;


                }
            }


        }

        MeshRenderer mr = GetComponent<MeshRenderer>();
        Mesh m = new Mesh();
        m.vertices = vertexes;
        m.triangles = triangles;
        m.colors = colors;

        
        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = m;

    }

    public float val1 = 0.5f;
    public float val2 = 0.25f;
    public float val3 = 0.15f;
    public float val4 = 0.1f;
    public float scale = 1f;
    public float refVal = 0.5f;
    public float factorX = 1;
    public float factorY = 1;

    void MakeRadial()
    {
//        factorX = Random.Range(1, 100);
//        factorY = Random.Range(1, 100);
    }

    float xCoord, yCoord, sample;
    bool Island(Vector2 q)
    {
        float xCoord;
        float yCoord;
        float sample = 0;

        xCoord = factorX + q.x / numOf_X * 5 * scale;
        yCoord = factorY + q.y / numOf_Y * 5 * scale;
        sample = Mathf.PerlinNoise(xCoord, yCoord) * val1;


        xCoord = factorX + q.x / numOf_X * 10 * scale;
        yCoord = factorY + q.y / numOf_Y * 10 * scale;
        sample += Mathf.PerlinNoise(xCoord, yCoord) * val2;

        xCoord = factorX + q.x / numOf_X * 20 * scale;
        yCoord = factorY + q.y / numOf_Y * 20 * scale;
        sample += Mathf.PerlinNoise(xCoord, yCoord) * val3;

        xCoord = factorX + q.x / numOf_X * 40 * scale;
        yCoord = factorY + q.y / numOf_Y * 40 * scale;
        sample += Mathf.PerlinNoise(xCoord, yCoord) * val4;

        return sample >= refVal ? true : false;
    }

    /*
    public float ISLAND_FACTOR = 1.07f;  // 1.0 means no small islands; 2.0 leads to a lot
    public float val3 = 0.01f;

    int bumps;                 // 굴곡의 량
    float startAngle;
    float dipAngle;
    float dipWidth;
    // 방사형
    void MakeRadial()
    {
        bumps = Random.Range(1, 5);
      
        startAngle = Random.Range(0f, 2f * Mathf.PI);
        dipAngle = Random.Range(0f, 2f * Mathf.PI);
        dipWidth = Random.Range(0.0f, 0.0f);

        Debug.Log(bumps);
        Debug.Log(startAngle * Mathf.Rad2Deg);
        Debug.Log(dipAngle * Mathf.Rad2Deg);
        Debug.Log(dipWidth);
        var i = 10 + "10";
        Debug.Log("i'm a lasagna hog".Split());

    }
    // 땅 구분
    bool Inside(Vector2 q)
    {

        float angle = Mathf.Atan2(q.y, q.x);
        float length = 0.01f * (Mathf.Max(Mathf.Abs(q.x) , Mathf.Abs(q.y)) + q.magnitude);
       
        float r1 = 0.5f + 0.10f * Mathf.Cos(startAngle + bumps * angle + Mathf.Cos((bumps + 3) * angle));
        float r2 = 0.7f - 0.20f * Mathf.Sin(startAngle + bumps * angle - Mathf.Sin((bumps + 2) * angle));
       
       
        if (Mathf.Abs(angle - dipAngle) < dipWidth
         || Mathf.Abs(angle - dipAngle + 2 * Mathf.PI) < dipWidth
         || Mathf.Abs(angle - dipAngle - 2 * Mathf.PI) < dipWidth)
        {
            r1 = r2 = 0.0f;
        }
        //      중심 || (섬을 나누기 위함 && 테두리는 바다로 만들기 위함)
        return (length < r1 || (length > r1 * ISLAND_FACTOR && length < r2));
        
    }
    */
    /*
    // 완성

    // 방사형
    void MakeRadial()
    {
        bumps = Random.Range(1, 5);
        startAngle = Random.Range(0f, 2f * Mathf.PI);
        dipAngle = Random.Range(0f, 2f * Mathf.PI);
        dipWidth = Random.Range(0.2f, 0.7f);
    }

    bool Inside(Vector2 q)
    {
        float angle = Mathf.Atan2(q.y, q.x);
        float length = val3 * (Mathf.Max(Mathf.Abs(q.x) * val1, Mathf.Abs(q.y) * val2) + Mathf.Sqrt(q.x * q.x + q.y * q.y));
    
        float r1 = 0.5f + 0.10f * Mathf.Sin(startAngle + bumps * angle + Mathf.Cos((bumps + 3) * angle));
        float r2 = 0.7f - 0.20f * Mathf.Sin(startAngle + bumps * angle - Mathf.Sin((bumps + 2) * angle));
        if (Mathf.Abs(angle - dipAngle) < dipWidth 
            || Mathf.Abs(angle - dipAngle + 2 * Mathf.PI) < dipWidth 
            || Mathf.Abs(angle - dipAngle - 2 * Mathf.PI) < dipWidth)
        {
            r1 = r2 = 0.2f;
        }
        //      중심 || (섬을 나누기 위함 && 테두리는 바다로 만들기 위함)
        return (length < r1 || (length > r1 * ISLAND_FACTOR && length < r2));
    }
    */

    // 원본
    //bool inside(Vector2 q)
    //{
    //    float angle = Mathf.Atan2(q.y, q.x);
    //    float length = 0.01f * (Mathf.Max(Mathf.Abs(q.x), Mathf.Abs(q.y)) + Mathf.Sqrt(q.x * q.x + q.y * q.y));
    //
    //    float r1 = 0.5f + 0.40f * Mathf.Sin(startAngle + bumps * angle + Mathf.Cos((bumps + 3) * angle)); // 0.4를 수정해보자 진짜 섬같음
    //    float r2 = 0.7f - 0.20f * Mathf.Sin(startAngle + bumps * angle - Mathf.Sin((bumps + 2) * angle));
    //    if (Mathf.Abs(angle - dipAngle) < dipWidth || Mathf.Abs(angle - dipAngle + 2 * Mathf.PI) < dipWidth || Mathf.Abs(angle - dipAngle - 2 * Mathf.PI) < dipWidth)
    //    {
    //        r1 = r2 = 0.2f;
    //    }
    //    //      중심 || (섬을 나누기 위함 && 테두리는 바다로 만들기 위함)
    //    return (length < r1 || (length > r1 * ISLAND_FACTOR && length < r2));
    //}


}