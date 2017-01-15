﻿/* A MonoBehaviour that allows the visualization of a heightmap. Any changes to the height map component
 will be immediately updated by the visualizer, allowing for a rapid exploration of how the properties of a given
 height map affect the final result. 
 
  This takes an arbitrary HeightMapModule, which means it will work for custom modules as well. An alternative
 would have been to write an editor script that offers a preview window, but that doesn't play as well
 with inheritance: each custom module would require its own custom editor script to override the default
 behaviour to draw nothing.
 
  Note that this script will destroy itself if it's used in a live build (i.e. it's editor-only)*/

using UnityEngine;
using CaveGeneration.MeshGeneration;

namespace CaveGeneration.Modules
{
    [ExecuteInEditMode]
    public sealed class HeightMapVisualizer : MonoBehaviour
    {
        [SerializeField] HeightMapModule heightMapModule;
        [SerializeField] Material material;
        [SerializeField] int size;
        [SerializeField] int scale;

        Mesh mesh;

        const int MIN_SIZE = 10;
        const int MAX_SIZE = 200;
        const int DEFAULT_SIZE = 75;

        const int MIN_SCALE = 1;
        const int DEFAULT_SCALE = 1;

        Mesh CreateMesh()
        {
            var wallGrid = new WallGrid(new byte[size, size], Vector3.zero, scale);
            IHeightMap heightMap = heightMapModule.GetHeightMap();
            return MeshBuilder.BuildFloor(wallGrid, heightMap).CreateMesh();
        }

        void Reset()
        {
            size = DEFAULT_SIZE;
            scale = DEFAULT_SCALE;
        }

        void Update()
        {
            if (CanDraw()) 
            {
                Graphics.DrawMesh(mesh, Matrix4x4.identity, material, 0);
            }
        }

        bool CanDraw()
        {
            return !Application.isPlaying && heightMapModule != null && material != null && mesh != null;
        }

        void OnValidate()
        {
            size = Mathf.Clamp(size, MIN_SIZE, MAX_SIZE);
            scale = Mathf.Max(scale, MIN_SCALE);
            mesh = CreateMesh();
        }

        // This destroys the script if it's accidentally left in an actual build, where it does nothing
        // but waste resources.
        void OnEnable() 
        {
            #if !UNITY_EDITOR
                Destroy(this);
            #endif
        }
    } 
}