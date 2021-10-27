using UnityEngine;
using System;

public class Mono : MonoBehaviour
{
    public Render render;

    public AnimationCurve curve;
    // public LineRenderer line;
    public Vector3 a,b;
    public float t;
    public GameObject thingy;
    public GameObject ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
        render.Start(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Instantiate(ballPrefab);
        }

        if(Input.GetKey(KeyCode.A)&& t>0)
        {
            t -= Time.deltaTime/1;
        }
        if(Input.GetKey(KeyCode.D)&& t<1)
        {
            t += Time.deltaTime/1;
        }
        thingy.transform.position = Vector3.LerpUnclamped(a,b,curve.Evaluate(t));

        // BagelOpera -> int bagelOpera = 0;
        // if (*insert what you want to do*); -> for (int i = 0; i < 10; i++)
        // def *function name* -> void NameFunction(int variable) { //code }
        


        

        // line.SetPosition(0, a);
        // line.SetPosition(1, b);
        render.Update();
    }
}

[Serializable]
public class Render
{
    Mono mono;

    public Mesh[] meshes;
    public Material[] materials;

    public void Start(Mono mono)
    {
        this.mono = mono;

        // meshes = Resources.LoadAll<Mesh>("Meshes/");
        materials = Resources.LoadAll<Material>("Materials/");
    }

    public void Update()
    {
        DrawMesh("Sphere", "Default", mono.b, Quaternion.identity, 1 / 2);
    }

    Matrix4x4 m4 = new Matrix4x4();
    void DrawMesh(string mesh, string mat, Vector3 pos, Quaternion rot, float scale)
    {
        m4.SetTRS(pos, rot, Vector3.one * scale);
        Graphics.DrawMesh(GetMesh(mesh), m4, GetMat(mat), 0);
    }

    public Mesh GetMesh(string name)
    {
        for (int i = 0; i < meshes.Length; i++)
        {
            if (meshes[i].name.ToLower() == name.ToLower())
            {
                // Debug.Log(meshes[i].name);
                return meshes[i];
            }
        }
        Debug.LogWarning("Mesh not found: " + name);
        return null;
    }

    public Material GetMat(string name)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            if (name.ToLower() == materials[i].name.ToLower())
            {
                // Debug.Log(materials[i].name);
                return materials[i];
            }
        }
        Debug.LogWarning("Material not found: " + name);
        return null;
    }
}