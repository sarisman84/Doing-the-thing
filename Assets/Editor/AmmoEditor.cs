using Interactivity.Destructable_Objects;
using Interactivity.Pickup;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AmmoCrate))]
    public class AmmoEditor : UnityEditor.Editor
    {
    }
}