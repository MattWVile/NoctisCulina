using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PolygonColliderFiller : MonoBehaviour
{
    public Color fillColor = new Color(1, 0, 0, 0.3f); // Semi-transparent red

    private PolygonCollider2D polyCollider;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        // Use a simple unlit transparent material
        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = fillColor;
        meshRenderer.material = mat;

        meshRenderer.sortingLayerName = "Player"; // Make sure this layer exists in your project
        meshRenderer.sortingOrder = 1; // Adjust as needed
    }

    void Update()
    {
        meshFilter.mesh = CreateMeshFromCollider(polyCollider);
        meshRenderer.enabled = polyCollider.enabled;
    }

    Mesh CreateMeshFromCollider(PolygonCollider2D collider)
    {
        Vector2[] points = collider.points;
        Vector3[] vertices = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            vertices[i] = points[i];

        // Triangulate the polygon (Unity does not provide a built-in method for this)
        int[] triangles = Triangulate(points);

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // Simple ear clipping triangulation for convex polygons
    int[] Triangulate(Vector2[] points)
    {
        if (points.Length < 3) return new int[0];
        int[] triangles = new int[(points.Length - 2) * 3];
        for (int i = 0; i < points.Length - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        return triangles;
    }
}