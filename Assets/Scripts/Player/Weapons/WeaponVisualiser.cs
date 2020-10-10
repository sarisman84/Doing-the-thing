using System.Collections.Generic;
using UnityEngine;

namespace Player.Weapons
{
    public class WeaponVisualiser : MonoBehaviour
    {
        Dictionary<string, GameObject> _knownWeaponModels = new Dictionary<string, GameObject>();

        private string _selectedModel;

        private void ChangeWeaponModel(string weaponModelName)
        {
            GameObject model;

            if (!_knownWeaponModels.TryGetValue(weaponModelName, out model))
                return;
            foreach (var keyPair in _knownWeaponModels)
            {
                keyPair.Value.SetActive(false);
            }

            model.SetActive(true);
            _selectedModel = weaponModelName;
        }

        public Transform WeaponBarrel
        {
            get
            {
                if (_knownWeaponModels[_selectedModel].Equals(null)) return null;
                return _knownWeaponModels[_selectedModel].transform.GetChild(0);
            }
        }


        public void SetWeaponModel(string weaponModelName, GameObject weaponModel)
        {
            if (_knownWeaponModels.ContainsKey(weaponModelName))
            {
                ChangeWeaponModel(weaponModelName);
                return;
            }
            if (_knownWeaponModels.ContainsValue(weaponModel)) return;


            weaponModel.transform.SetParent(transform);
            weaponModel.transform.localPosition = Vector3.zero;
            weaponModel.SetActive(false);
            _knownWeaponModels.Add(weaponModelName, weaponModel);

            ChangeWeaponModel(weaponModelName);
        }
    }
}