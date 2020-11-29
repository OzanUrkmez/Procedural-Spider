using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class IK_Controller : MonoBehaviour
{

    [SerializeField]
    private int _chainLength;

    [SerializeField]
    private Transform _targetTransform;




    protected float[] _boneLengths;
    protected float _completeLength;
    protected Transform[] _bones;
    protected Vector3[] _positions;



    #region Visuals


    [SerializeField]
    private bool _drawGizmos;

    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            var current = this.transform;
            for(int i = 0; i < _chainLength && current != null && current.parent != null;i++)
            {
                var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
                Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position),
                    new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));

                Handles.color = Color.green;
                Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
                current = current.parent;
            }
        }    
    }

    #endregion

}
