using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Shadee.Dialogues
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        protected Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();


        private void Awake() 
        {
#if UNITY_EDITOR

#endif

            OnValidate();
        }

        private void OnValidate() 
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach(string childID in parentNode.children)
            {
                if(nodeLookup.ContainsKey(childID))
                    yield return nodeLookup[childID];
            }
        }

        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            if(parent != null)
            {
                parent.children.Add(newNode.name);
            }
            nodes.Add(newNode);
            // AssetDatabase.AddObjectToAsset(newNode, this);
            OnValidate();
        }
    
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.children.Remove(nodeToDelete.name);
            }
        }

        public void OnBeforeSerialize()
        {
            if(nodes.Count == 0)
            {
                CreateNode(null);
            }
            
            if(AssetDatabase.GetAssetPath(this) != String.Empty)
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if(AssetDatabase.GetAssetPath(node) == String.Empty)
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
