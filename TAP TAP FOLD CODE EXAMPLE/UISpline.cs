using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UISpline : Graphic // UGUI basic class
{
    [Header("UI Spline")]
    public int resolution = 10;
    public float handleStrength = 1.0f;
    public Sprite splineSprite;

    [Range(0f,1f)] 
    public float handlePosition = 1.0f;
    public float width = 1f;
    public AnimationCurve widthMapping;
    public Vector2 textureScale = new Vector2(1f, 1f);

    [Header("Pointer")]
    public bool enablePointer;
    public Image pointerImg;
    [SerializeField]private Vector3 start;
    [SerializeField]private Vector3 end;
    private Vector3 handle;
    private bool resetHandle = false;
    private float singleSegementTextureUV;

    
    

    public Vector3 startOffset;
    public Vector3 endOffest;

    public override Texture mainTexture => splineSprite.texture;

    protected override void OnPopulateMesh(VertexHelper vh) // to populate the mesh 
    {
        vh.Clear();
        MakeSpline(vh);
       // material.color = color;
    }

    public void UpdateUISpline(Vector3 start, Vector3 end)
    {
        if(start == this.start && end == this.end) return;
        //if(start != null)
        //Debug.Log(start);
        this.start = start;
        //if(end != null)
        this.end = end;

        //通过起始点和结束点计算出中间的控制点
        Vector3 vec = this.end - this.start;
        Vector3 mid = this.start + vec * handlePosition;
        Vector3 normal = new Vector3(-vec.y, vec.x, 0).normalized;

        if(this.end.x < this.start.x)
        {
            normal = -normal;
        }

        Vector3 temp = mid + handleStrength * Mathf.Sqrt(vec.sqrMagnitude) * normal;

        if (resetHandle)
        {
            resetHandle = false;
            handle = mid;
        }

        handle = Vector3.Lerp(handle, temp, Time.deltaTime * 20f);

        if (enablePointer)
        {
            pointerImg.gameObject.SetActive(true);
            pointerImg.transform.localPosition = this.end + transform.position;
            pointerImg.transform.up = (this.end - temp).normalized;
        }

        SetVerticesDirty();
    }

    private void MakeSpline(VertexHelper vh)
    {
        var points = CalculateBezierPoints(start, end, handle, resolution);
        singleSegementTextureUV = Vector3.Distance(start,end) / Screen.height;

        for (int i = 0; i< points.Count - 1; i++)
        {
            bool isLast = i == points.Count - 2;
            float newWidth = widthMapping.Evaluate(i / (float)(points.Count - 1)) * width;
            AddSegment(vh, i, points[i], points[i + 1], points[i + (isLast ? 1 : 2)], newWidth, isLast);
        }
    }

    private void AddVert(VertexHelper vh, Vector3 position, Vector4 uv)
    {
        UIVertex uIVertex = UIVertex.simpleVert;
        uIVertex.color = color;
        uIVertex.position = position;
        uIVertex.uv0 = uv;
        vh.AddVert(uIVertex);
    }
    

    // 添加一个线段
    private void AddSegment(VertexHelper vh, int i, Vector3 start, Vector3 end, Vector3 next, float width, bool isLast)
    {
        //根据起始点和结束点计算出四个顶点
        //为了衔接后续的线段，使用结束点与下一个点的方向来计算法线
        Vector3 direction1 = (end - start).normalized;
        Vector3 direction2 = (next - end).normalized;

        Vector3 normal1 = new(-direction1.y, direction1.x, 0);
        Vector3 normal2 = new(-direction2.y, direction2.x, 0);

        if(isLast)
        {
            normal2 = normal1;
        }

        Vector3 p1 = start + normal1 * width /2;
        Vector3 p2 = start - normal1 * width /2;
        Vector3 p3 = end + normal2 * width /2;
        Vector3 p4 = end - normal2 * width /2;

        //添加顶点
        AddVert(vh, p1, new Vector4(i * singleSegementTextureUV * textureScale.x, 1f));
        AddVert(vh, p2, new Vector4(i * singleSegementTextureUV * textureScale.x, 0f));
        AddVert(vh, p3, new Vector4((i + 1) * singleSegementTextureUV * textureScale.x, 1f));
        AddVert(vh, p4, new Vector4((i + 1) * singleSegementTextureUV * textureScale.x, 0f));

        //添加三角形
        int startIndex = vh.currentVertCount - 4;
        vh.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
        vh.AddTriangle(startIndex, startIndex + 3, startIndex + 1);

    }

    private List<Vector3> CalculateBezierPoints(Vector3 start, Vector3 end, Vector3 handle, int resolution) // Calculate Bezier curve for spline trail 计算二次贝塞尔曲线上的点
    {
        if(resolution<1) resolution = 1;
        List<Vector3> points = new();

        for(int i=0; i<= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = (1 - t) * ( 1 - t ) * start + 2 * (1 - t) * t * handle + t * t * end;
            points.Add(point);
        }
        //Debug.Log(points[0]);
        return points;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        resetHandle = true;
        //Debug.Log(Screen.width);

        
    }

    public List<Vector3> GetStartAndEndPoint()
    {
        List<Vector3> temp = new();
        temp.Add(start);
        temp.Add(end);
        return temp;
    }
}
