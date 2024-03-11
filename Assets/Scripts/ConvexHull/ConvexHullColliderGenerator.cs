using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ux {

    public class ConvexHullColliderGenerator : MonoBehaviour {
        public Mesh sourceMesh;
        public GameObject targetObject;
        public bool DestroyExistingColliders;

        MeshCollider createdCollider;

        void Awake() {
            if (sourceMesh == null) {
                sourceMesh = GetComponent<MeshFilter>()?.sharedMesh;
                if (sourceMesh == null) {
                    Debug.LogWarning("No source mesh.");
                    return;
                }
            }
            if (targetObject == null) {
                targetObject = gameObject;
            }
            UpdateCollider();
        }

        void UpdateCollider() {
            if (createdCollider == null) {
                if (DestroyExistingColliders) {
                    foreach (var c in GetComponents<Collider>().ToArray()){
                        Destroy(c);
                    }
                }
                createdCollider = targetObject.AddComponent<MeshCollider>();
                createdCollider.convex = true;
            }
            createdCollider.sharedMesh = ConvexHull.Generate(sourceMesh.vertices);
        }
    }

}