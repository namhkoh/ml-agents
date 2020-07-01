using System.Collections.Generic;
using UnityEngine;

namespace Unity.MLAgents.Extensions.Sensors
{

    /// <summary>
    /// Utility class to track a hierarchy of RigidBodies. These are assumed to have a root node,
    /// and child nodes are connect to their parents via Joints.
    /// </summary>
    public class RigidBodyPoseExtractor : PoseExtractor
    {
        Rigidbody[] m_Bodies;

        /// <summary>
        /// Initialize given a root RigidBody.
        /// </summary>
        /// <param name="rootBody"></param>
        public RigidBodyPoseExtractor(Rigidbody rootBody)
        {
            if (rootBody == null)
            {
                return;
            }
            var rbs = rootBody.GetComponentsInChildren <Rigidbody>();
            var bodyToIndex = new Dictionary<Rigidbody, int>(rbs.Length);
            var parentIndices = new int[rbs.Length];

            if (rbs[0] != rootBody)
            {
                Debug.Log("Expected root body at index 0");
                return;
            }

            for (var i = 0; i < rbs.Length; i++)
            {
                bodyToIndex[rbs[i]] = i;
            }

            var joints = rootBody.GetComponentsInChildren <Joint>();


            foreach (var j in joints)
            {
                var parent = j.connectedBody;
                var child = j.GetComponent<Rigidbody>();

                var parentIndex = bodyToIndex[parent];
                var childIndex = bodyToIndex[child];
                parentIndices[childIndex] = parentIndex;
            }

            m_Bodies = rbs;
            SetParentIndices(parentIndices);
        }




//        public override void UpdateModelSpacePoses()
//        {
//            base.UpdateModelSpacePoses();
//            var numBodies = m_Bodies?.Length ?? 0;
//            if (numBodies == 0)
//            {
//                return;
//            }
//
//            var worldPose = GetPoseAt(0);
//            var worldToModel = worldPose.Inverse();
//            var rootBody = m_Bodies[0];
//
//            for (var i = 0; i < numBodies; i++)
//            {
//                var body = GetBodyAt(i);
//                var relativeVelocity = body.velocity - rootBody.velocity;
//                m_ModelSpaceLinearVelocities[i] = worldToModel.Multiply(relativeVelocity);
//            }
//        }
//
//        public override void UpdateLocalSpacePoses()
//        {
//            base.UpdateLocalSpacePoses();
//
//            var numBodies = m_Bodies?.Length ?? 0;
//            if (numBodies == 0)
//            {
//                return;
//            }
//
//            for (var i = 0; i < numBodies; i++)
//            {
//                var parentIndex = GetParentIndex(i);
//                if (i != -1)
//                {
//                    var parentTransform = GetPoseAt(parentIndex);
//                    var parentBody = GetBodyAt(parentIndex);
//                    var currentBody = GetBodyAt(i);
//                    // This is slightly inefficient, since for a body with multiple children, we'll end up inverting
//                    // the transform multiple times. Might be able to trade space for perf here.
//                    var invParent = parentTransform.Inverse();
//                    m_LocalSpaceLinearVelocities[i] = invParent.Multiply(currentBody.velocity - parentBody.velocity);
//                }
//                else
//                {
//                    m_LocalSpaceLinearVelocities[i] = Vector3.zero;
//                }
//            }
//        }

        protected Rigidbody GetBodyAt(int index)
        {
            return m_Bodies[index];
        }

        protected override Vector3 GetLinearVelocityAt(int index)
        {
            return GetBodyAt(index).velocity;
        }

        /// <summary>
        /// Get the pose of the i'th RigidBody.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Pose GetPoseAt(int index)
        {
            var body = m_Bodies[index];
            return new Pose { rotation = body.rotation, position = body.position };
        }
    }

}
