using UnityEngine;
using System;

public class Mono : MonoBehaviour
{
  public Render render;

  public AnimationCurve curve;
  // public LineRenderer line;
  public Vector3 a, b;
  public float t;
  public GameObject thingy, ballPrefab;
  public Vector3 target;
  public Vector3[] appendage;
  public bool kinematic;
  
  void Start()
  {
    render.Start(this);
  }

  void Update()
  {
    Vector3 mousePos = Input.mousePosition;
    mousePos.z = 10;
    target = Camera.main.ScreenToWorldPoint(mousePos);
    target.z = 0;
    if (kinematic)
    {
        InverseKinematics(appendage, target);
    }

    thingy.transform.position = Vector3.LerpUnclamped(a, b, curve.Evaluate(t));
    // line.SetPosition(0, a);
    // line.SetPosition(1, b);
    render.Update();
  }

  const int maxIterations = 100;
  const float minAcceptableDst = 0.01f;
  public void InverseKinematics(Vector3[] points, Vector3 target) {
    Vector3 origin = points[0];
    float[] segmentLengths = new float[points.Length - 1];
    for (int i = 0; i < segmentLengths.Length; i++) {
        segmentLengths[i] = (points[i + 1] - points[i]).magnitude;
    }

    for (int iteration = 0; iteration < maxIterations; iteration++) {
        bool startingFromTarget = iteration % 2 == 0;
        // Reverse arrays to alternate between forward and backward passes
        System.Array.Reverse(points);
        System.Array.Reverse(segmentLengths);
        points[0] = (startingFromTarget) ? target : origin;

        // Constrain lengths
        for (int i = 1; i < points.Length; i++) {
            Vector3 dir = (points[i] - points[i - 1]).normalized;
            points[i] = points[i - 1] + dir * segmentLengths[i - 1];
        }

        // Terminate if close enough to target
        float dstToTarget = (points[points.Length - 1] - target).magnitude;
        if (!startingFromTarget && dstToTarget <= minAcceptableDst) {
            return;
        }
    }
  }
}

[Serializable]
public class Render
{
  Mono mono;

  Mesh[] meshes;
  Material[] materials;

  public void Start(Mono mono)
  {
    this.mono = mono;

    meshes = Resources.LoadAll<Mesh>("Meshes/");
    materials = Resources.LoadAll<Material>("Materials/");
  }

  public void Update()
  {
    DrawMesh("icosphere", "default", mono.a, Quaternion.identity, 0.5f);
    DrawMesh("icosphere", "default", mono.b, Quaternion.identity, 0.5f);

    for (int i = 0; i < mono.appendage.Length; i++)
    {
        Vector3 point = mono.appendage[i];
        DrawMesh("icosphere", "default", point, Quaternion.identity, 0.1f);
    }

    DrawMesh("icosphere", "default", mono.target, Quaternion.identity, 0.2f);
  }

  Matrix4x4 m4 = new Matrix4x4();
  void DrawMesh(string mesh, string mat, Vector3 pos, Quaternion rot, float scale)
  {
    m4.SetTRS(pos, rot, Vector3.one * scale);
    Graphics.DrawMesh(Mesh(mesh), m4, Mat(mat), 0);
  }

  public Material Mat(string name)
  {
    for (int i = 0; i < materials.Length; i++)
    {
      if (name.ToLower() == materials[i].name.ToLower())
      {
        return materials[i];
      }
    }
    Debug.LogWarning("Material not found: " + name);
    return null;
  }

  public Mesh Mesh(string name)
  {
    for (int i = 0; i < meshes.Length; i++)
    {
      if (meshes[i].name.ToLower() == name.ToLower())
      {
        return meshes[i];
      }
    }
    Debug.LogWarning("Mesh not found: " + name);
    return null;
  }
}