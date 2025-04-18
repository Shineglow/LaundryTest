using System.Collections.Generic;
using UnityEngine;

namespace LaundryTest.UI
{
    public class UISystem : MonoBehaviour
    {
        [SerializeField] private List<Transform> layers;
        
        private IUIModel _activeModel;

        private void Awake()
        {
            layers ??= new List<Transform>((int)EUILayer.Count);
            
            if (layers == null || layers.Count <= (int)EUILayer.Count)
            {
                for(int i = 0; i < (int)EUILayer.Count; i++)
                {
                    if (layers.Count <= i)
                    {
                        layers.Add(default);
                    }
                    if (layers[i] != null)
                    {
                        continue;
                    }
                    var layer = new GameObject(((EUILayer)i).ToString());
                    layer.transform.SetParent(transform);
                    layers[i] = layer.transform;
                }
            }
            else
            {
                Debug.LogError("There are more layer objects than layer names or the layer name enumeration is not filled in correctly.");
            }
        }

        public void SetUIModel(IUIModel uiModel, EUILayer layer = default)
        {
            if (_activeModel != null)
                _activeModel.IsActive = false;
            _activeModel = uiModel;
            uiModel.IsActive = true;
        }
    }

    public enum EUILayer
    {
        Default,
        DefaultOverlay,
        GameMenu,
        GameMenuOverlay,
        Debug, 
        
        // add new layers above this line
        
        Count,
    }
}