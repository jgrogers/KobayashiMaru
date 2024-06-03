using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ConstructMe : MonoBehaviour
{
    public class Triangle {
        public int[] vertices = new int[3];
        public Triangle(int a, int b, int c) {
            vertices[0] = a; vertices[1] = b; vertices[2] = c;
        }
        public int a() {
            return vertices[0];
        }
        public int b() {
            return vertices[1];
        }public int c() {
            return vertices[2];
        }public string getString() {
            return vertices[0].ToString() + ", " + vertices[1].ToString() + ", " + vertices[2].ToString();
        }
    }
    public class WedgeType {
        public GameObject wedge;
        public Vector3 initial_position;
        public float StartTime;
        public bool isCollapsed;
        public WedgeType(GameObject go) {
            initial_position = go.transform.position;
            wedge = go;
            isCollapsed = false;
        }
    }
    [SerializeField] Material material;
    [SerializeField] float collapsingSpeed = 4f;
    // Start is called before the first frame update
    private List<WedgeType> wedges = new List<WedgeType>();
    [SerializeField] GameObject BlueGlow;
    [SerializeField] TMP_Text prompt;
    [SerializeField] TMP_Text critical;
    private List<int> NeedsCollapsing = new List<int> ();
    int Collapsing = -1;
    bool blueGlowStarted = false;
    float blueGlowStartTime;
    private Color initialColor;
    void Start()
    {
        BlueGlow.transform.localScale = new Vector3(0,0,0);
        initialColor = material.color;
        prompt.enabled = false;
        critical.enabled = false;
        Vector3[] vertices = new Vector3[13];
        vertices[0] = new Vector3(-0.26286500f, 0.0000000f, 0.42532500f);
        vertices[1] = new Vector3(0.26286500f, 0.0000000f, 0.42532500f);
        vertices[2] = new Vector3(-0.26286500f, 0.0000000f, -0.42532500f);
        vertices[3] = new Vector3(0.26286500f, 0.0000000f, -0.42532500f);
        vertices[4] = new Vector3(0.0000000f, 0.42532500f, 0.26286500f);
        vertices[5] = new Vector3(0.0000000f, 0.42532500f, -0.26286500f);
        vertices[6] = new Vector3(0.0000000f, -0.42532500f, 0.26286500f);
        vertices[7] = new Vector3(0.0000000f, -0.42532500f, -0.26286500f);
        vertices[8] = new Vector3(0.42532500f, 0.26286500f, 0.0000000f);
        vertices[9] = new Vector3(-0.42532500f, 0.26286500f, 0.0000000f);
        vertices[10] = new Vector3(0.42532500f, -0.26286500f, 0.0000000f);
        vertices[11] = new Vector3(-0.42532500f, -0.26286500f, 0.0000000f);
        vertices[12] = new Vector3(0,0,0);
        
        List<Triangle> triangles = new List<Triangle>();
        triangles.Add(new Triangle(1, 0, 6));
        triangles.Add(new Triangle(0, 1, 4));
        triangles.Add(new Triangle(5, 4, 8));
        triangles.Add(new Triangle(8, 1, 10));
        triangles.Add(new Triangle(10, 3, 8));
        triangles.Add(new Triangle(5, 3, 2));
        triangles.Add(new Triangle(2, 3, 7));
        triangles.Add(new Triangle(11, 2, 7));
        triangles.Add(new Triangle(0, 9, 11));
        triangles.Add(new Triangle(11, 9, 2));
        triangles.Add(new Triangle(2, 9, 5));
        triangles.Add(new Triangle(9, 4, 5));
        triangles.Add(new Triangle(0, 4, 9));
        triangles.Add(new Triangle(3, 5, 8));
        triangles.Add(new Triangle(1, 8, 4));
        triangles.Add(new Triangle(6, 10, 1));
        triangles.Add(new Triangle(6, 0, 11));
        triangles.Add(new Triangle(7, 6, 11));
        triangles.Add(new Triangle(3, 10, 7));
        triangles.Add(new Triangle(6, 7, 10));

        foreach (Triangle triangle in triangles) {
            GameObject wedge = new GameObject("Ico");
            wedge.AddComponent<MeshRenderer>();

            wedge.AddComponent<MeshFilter>();
            Mesh wedgeMesh = new Mesh();
            int[] wedgeTris = new int [4*3];
            wedgeTris[0] = triangle.a();
            wedgeTris[1] = triangle.b();
            wedgeTris[2] = triangle.c();
            wedgeTris[3] = 12;
            wedgeTris[4] = triangle.b();
            wedgeTris[5] = triangle.a();
            wedgeTris[6] = 12;
            wedgeTris[7] = triangle.c();
            wedgeTris[8] = triangle.b();
            wedgeTris[9] = 12;
            wedgeTris[10] = triangle.a();
            wedgeTris[11] = triangle.c();
            wedgeMesh.vertices = vertices;
            wedgeMesh.triangles = wedgeTris;
            wedgeMesh.RecalculateNormals();
            wedge.GetComponent<MeshFilter>().mesh = wedgeMesh;
            wedge.transform.position += getAveragePoint(wedgeMesh) * 5.0f;
            wedge.GetComponent<MeshRenderer>().material = material;
            
            wedges.Add(new WedgeType(wedge));
        }
        for (int i = 0; i < wedges.Count; i++) {
            NeedsCollapsing.Add(i);
        }
    }
    public Vector3 getAveragePoint(Mesh mesh) {
        Vector3 result = new Vector3(0,0,0);
        foreach (int ind in mesh.triangles) {
            result += mesh.vertices[ind];
        }
        result /= mesh.triangles.Length;
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        if (NeedsCollapsing.Count == 0) {
            // Do blue glow effect
        }
        if (Collapsing == -1 && NeedsCollapsing.Count > 0) {
            Collapsing = Random.Range(0, NeedsCollapsing.Count);
            wedges[NeedsCollapsing[Collapsing]].StartTime = Time.time;
        }
        if (Collapsing != -1) {
            WedgeType wedge = wedges[NeedsCollapsing[Collapsing]];
            float prop =(Time.time - wedge.StartTime)*collapsingSpeed; 
            if(prop >= 1.0f) {
                prop = 1.0f;
                NeedsCollapsing.RemoveAt(Collapsing);
                Collapsing = -1;
            }
            wedge.wedge.transform.position = Vector3.Lerp(wedge.initial_position, new Vector3(0,0,0), prop);
        } else {
            if (!blueGlowStarted) {
                blueGlowStarted =true;
                blueGlowStartTime = Time.time;
            }
            float prop =Time.time - blueGlowStartTime ;
            if (prop > 1.0f) prompt.enabled = true;
            if (prop > 1.3f) {
                critical.enabled = true;
                Camera.main.GetComponent<camera_rotate_lock>().enabled = false;
            }
            if (prop > 2.5f) {
                SceneManager.LoadScene(1);
            }
            if (prop > 1.0f) prop = 1.0f;
            BlueGlow.transform.localScale = Vector3.Lerp(new Vector3(0,0,0), new Vector3(6f,6f,6f), Mathf.Sqrt(prop));
            foreach (WedgeType wedge in wedges) {
                wedge.wedge.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.Lerp(initialColor, Color.red*10.0f, prop));

            }
        }
        
    }
}
