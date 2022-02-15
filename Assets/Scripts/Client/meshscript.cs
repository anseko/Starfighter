using UnityEngine;

namespace Client
{
    public class mapgrid1 : MonoBehaviour
    {
        private void Awake()
        {
            var meshFilter = gameObject.GetComponent<MeshFilter>();
            var mesh = new Mesh();

            var width = 128;
            var height = 128;

            mesh.vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(width, 0, 0),
                new Vector3(0, height, 0),
                new Vector3(width, height, 0)
            };

            mesh.triangles = new int[6]
            {
                0, 2, 1,    // lower left triangle
                2, 3, 1     // upper right triangle
            };

            mesh.normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };

            mesh.uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            meshFilter.mesh = mesh;

            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Custom/gridshader2"));
            meshRenderer.material.color = new Color(.0f, .1176469f, 1.0f, .866f);
        }
    }
}