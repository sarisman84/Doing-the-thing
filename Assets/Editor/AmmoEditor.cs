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
        public override void OnInspectorGUI()
        {
            AmmoCrate _ref = target as AmmoCrate;


            _ref.ammoType = EditorGUILayout.TextField(new GUIContent("Weapon to supply ammo to:"), _ref.ammoType);
            _ref.amountOfAmmo = EditorGUILayout.IntField(new GUIContent("Amount of ammo to spawn when broken:"),
                _ref.amountOfAmmo);

        }
    }
}