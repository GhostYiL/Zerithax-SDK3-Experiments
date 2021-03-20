
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax.MagicCircle
{
    public class MagicModifier : UdonSharpBehaviour
    {
        [SerializeField] private int modifier;

        public int GetModifier()
        {
            return modifier;
        }

        public MagicModifier[] GetSiblingModifiers()
        {
            return transform.parent.GetComponentsInChildren<MagicModifier>();
        }
    }
}